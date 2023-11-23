import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent {
  public items: PaginatedResponse<BasicItemResponse> = {
    castEntities: [],
    total: 0,
    page: 1,
    limit: 10,
  };
  public isLoading: boolean = true;
  title: string = '';
  minPrice: number = 0;
  maxPrice: number = 0;
  categoryQuery: string = '';
  selectedCategoryNames: string[] = [];
  imageSrc: string = '';

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) {}

  private setQueryParameter(
    paramName: string,
    queryParams: ParamMap,
    setter: (value: string) => void
  ): void {
    if (queryParams.has(paramName)) {
      const value = queryParams.get(paramName);
      if (value != null) {
        setter(value);
      }
    }
  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.isLoading = true;

      this.setQueryParameter('page', queryParams, (value) => {
        this.items.page = parseInt(value);
      });
      this.setQueryParameter('title', queryParams, (value) => {
        this.title = value;
      });
      this.setQueryParameter('minPrice', queryParams, (value) => {
        this.minPrice = Number(value);
      });
      this.setQueryParameter('maxPrice', queryParams, (value) => {
        this.maxPrice = Number(value);
      });

      if (queryParams.has('category')) {
        const categoryNames = queryParams.getAll('category');
        this.categoryQuery = categoryNames
          .map((categoryName) => `categories=${categoryName}`)
          .join('&');
      }

      this.fetchItems();
    });
  }

  private makeApiSearchCall(): void {
    this.http
      .get(
        `${ItemEndpoints.search}?page=${this.items.page}&limit=${
          this.items.limit
        }${this.title && `&title=${this.title}`}${
          this.minPrice !== null &&
          this.maxPrice !== null &&
          `&minPrice=${this.minPrice}&maxPrice=${this.maxPrice}`
        }&${this.categoryQuery}`
      )
      .subscribe({
        next: (response: PaginatedResponse<BasicItemResponse> | any) => {
          this.items = response;
          this.isLoading = false;
          console.log(this.items);
          this.http
            .get(
              `${ItemEndpoints.getImage(this.items.castEntities[5].imageGuid)}`,
              {
                responseType: 'blob',
              }
            )
            .subscribe((imageData) => {
              const blob = new Blob([imageData], { type: 'image/jpeg' });
              const imageUrl = URL.createObjectURL(blob);
              this.imageSrc = imageUrl;
            });
        },
        error: (error) => {
          console.error(error);
          this.isLoading = false;
          alert('An error occured while searching for items');
        },
      });
  }

  private fetchItems(): void {
    if (environment.production) {
      this.makeApiSearchCall();
    } else {
      setTimeout(() => {
        this.makeApiSearchCall();
      }, environment.timeout);
    }
  }

  public showFiltersDialog() {
    this.dialog.open(FiltersDialogComponent, {
      width: '50vw',
      height: '60vh',
      autoFocus: false,
      restoreFocus: false,
    });
  }

  public onSearchSubmit(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: this.items.page, title: this.title },
      queryParamsHandling: 'merge',
    });
  }

  public onPageChange(event: PageEvent): void {
    this.isLoading = true;

    if (this.paginatorTop && this.paginatorBottom) {
      this.paginatorTop.pageIndex = event.pageIndex;
      this.paginatorBottom.pageIndex = event.pageIndex;
    }

    const selectedPage = event.pageIndex + 1;

    if (event.pageSize != this.items.limit) {
      this.items.limit = event.pageSize;
      this.items.page = selectedPage;
      this.fetchItems();
      return;
    }

    this.items.page = selectedPage;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: selectedPage },
      queryParamsHandling: 'merge',
    });
  }
}

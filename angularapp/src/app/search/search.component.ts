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
import { MatSnackBar } from '@angular/material/snack-bar';
import { ItemComponent } from './item/item.component';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent {
  public items: PaginatedResponse<BasicItemResponse> = {
    castEntities: [],
    total: 0,
    page: 1,
    limit: 10,
  };
  isLoading: boolean = true;
  title: string = '';
  minPrice: number = 0;
  maxPrice: number = 0;
  categoryQuery: string = '';
  selectedCategoryNames: string[] = [];
  images: { src: string; isLoading: boolean }[] = [];

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private filtersDialog: MatDialog,
    private itemDialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  private setQueryParameter(
    paramName: string,
    queryParams: ParamMap,
    setter: (value: string) => void,
    defaultValue: any
  ): void {
    if (queryParams.has(paramName)) {
      const value = queryParams.get(paramName);
      if (value != null) {
        setter(value);
        return;
      }
    }

    setter(defaultValue);
  }

  private openNoItemsFoundSnackBar(): void {
    this.snackBar.open('No Items found', 'close', {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 1500,
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
          this.images = new Array(this.items.limit).fill({
            src: '',
            isLoading: true,
          });

          if (this.items.castEntities.length === 0) {
            this.openNoItemsFoundSnackBar();
          }

          this.items.castEntities.map((item, i) => {
            this.http
              .get(`${ItemEndpoints.getImage(item.imageGuid)}`, {
                responseType: 'blob',
              })
              .subscribe({
                next: (imageData: Blob) => {
                  setTimeout(() => {
                    const blob = new Blob([imageData], {
                      type: 'image/jpeg',
                    });
                    const imageUrl = URL.createObjectURL(blob);
                    this.images[i] = {
                      src: imageUrl,
                      isLoading: false,
                    };
                  }, environment.timeout);
                },
                error: (error) => {
                  console.error(error);
                },
              });
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

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.isLoading = true;

      this.setQueryParameter(
        'page',
        queryParams,
        (value) => {
          this.items.page = parseInt(value);
        },
        1
      );
      this.setQueryParameter(
        'title',
        queryParams,
        (value) => {
          this.title = value;
        },
        ''
      );
      this.setQueryParameter(
        'minPrice',
        queryParams,
        (value) => {
          this.minPrice = Number(value);
        },
        0
      );
      this.setQueryParameter(
        'maxPrice',
        queryParams,
        (value) => {
          this.maxPrice = Number(value);
        },
        0
      );

      if (queryParams.has('category')) {
        const categoryNames = queryParams.getAll('category');
        this.categoryQuery = categoryNames
          .map((categoryName) => `categories=${categoryName}`)
          .join('&');
      }

      this.fetchItems();
    });
  }

  public showFiltersDialog(): void {
    this.filtersDialog.open(FiltersDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
    });
  }

  public clearFilters(): void {
    let queryParams = { ...this.route.snapshot.queryParams };

    delete queryParams['minPrice'];
    delete queryParams['maxPrice'];
    delete queryParams['category'];

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
    });
  }

  public removeTitle(): void {
    this.title = '';

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { title: '' },
      queryParamsHandling: 'merge',
    });
  }

  public showItemDialog(itemId: number): void {
    this.itemDialog.open(ItemComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        itemId: itemId,
      },
    });
  }

  public onSearchSubmit(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: 1, title: this.title },
      queryParamsHandling: 'merge',
    });
  }

  public onPageChange(event: PageEvent): void {
    this.isLoading = true;

    if (event.pageSize != this.items.limit) {
      this.items.limit = event.pageSize;

      if (this.paginatorTop && this.paginatorBottom) {
        this.paginatorTop.pageIndex = this.items.page - 1;
        this.paginatorBottom.pageIndex = this.items.page - 1;
      }
      this.fetchItems();
      return;
    }

    const selectedPage = event.pageIndex + 1;

    if (this.paginatorTop && this.paginatorBottom) {
      this.paginatorTop.pageIndex = event.pageIndex;
      this.paginatorBottom.pageIndex = event.pageIndex;
    }

    this.items.page = selectedPage;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: selectedPage },
      queryParamsHandling: 'merge',
    });
  }

  public getNumberArray(limit: number): number[] {
    return Array.from({ length: limit }, (_, i) => i);
  }
}

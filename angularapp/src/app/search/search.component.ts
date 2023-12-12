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
import { DateTimeFormatService } from '../shared/date-time-format.service';

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
  images: { src: string; isLoading: boolean; itemId: number }[] = [];

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private filtersDialog: MatDialog,
    private itemDialog: MatDialog,
    private snackBar: MatSnackBar,
    private formatter: DateTimeFormatService
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.isLoading = true;

      // Reset to default values
      this.items.page = 1;
      this.title = '';
      this.minPrice = 0;
      this.maxPrice = 0;
      this.categoryQuery = '';

      // Set the new values if they exist
      this.setQueryParameter('page', queryParams, (value) => {
        this.items.page = Number(value);
      });

      if (this.items.page == 1) {
        this.removeUrlParameters(['page']);
      }

      if (this.paginatorTop && this.paginatorBottom) {
        console.log('changed');

        this.paginatorTop.pageIndex = this.items.page - 1;
        this.paginatorBottom.pageIndex = this.items.page - 1;
      }

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

  private fetchItems(): void {
    if (environment.production) {
      this.makeApiSearchCall();
    } else {
      setTimeout(() => {
        this.makeApiSearchCall();
      }, environment.timeout);
    }
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
            itemId: -1,
          });

          if (this.items.castEntities.length === 0) {
            this.openNoItemsFoundSnackBar();
          }

          this.items.castEntities.map((item, i) => {
            item.started = this.formatter.convertOnlyToDate(item.started);
            item.ends = this.formatter.convertOnlyToDate(item.ends);
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
                      itemId: item.itemId,
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

  public showFiltersDialog(): void {
    this.filtersDialog.open(FiltersDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
    });
  }

  public showItemDialog(itemIdx: number): void {
    this.itemDialog.open(ItemComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        item: { ...this.items.castEntities[itemIdx] },
        image: this.images[itemIdx],
      },
    });
  }

  public clearFilters(): void {
    this.removeUrlParameters(['minPrice', 'maxPrice', 'category']);
  }

  public removeTitle(): void {
    this.removeUrlParameters(['title']);
  }

  public onSearchSubmit(): void {
    this.addUrlParameters([
      { name: 'page', value: '1' },
      { name: 'title', value: this.title },
    ]);
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

    this.items.page = selectedPage;

    this.addUrlParameters([{ name: 'page', value: selectedPage.toString() }]);
  }

  public getNumberArray(limit: number): number[] {
    return Array.from({ length: limit }, (_, i) => i);
  }

  private setQueryParameter(
    paramName: string,
    queryParams: ParamMap,
    setter: (value: string) => void
  ): void {
    if (queryParams.has(paramName)) {
      const value = queryParams.get(paramName);
      if (value != null) {
        setter(value);
        return;
      }
    }
  }

  private openNoItemsFoundSnackBar(): void {
    this.snackBar.open('No Items found', 'close', {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 2500,
      panelClass: ['error-snackbar'],
    });
  }

  private addUrlParameters(params: { name: string; value: string }[]): void {
    let queryParams: Record<string, string> = {};

    params.forEach((param) => {
      queryParams[param.name] = param.value;
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
      queryParamsHandling: 'merge',
    });
  }

  private removeUrlParameters(paramNames: string[]): void {
    let queryParams = { ...this.route.snapshot.queryParams };

    paramNames.forEach((paramName) => {
      delete queryParams[paramName];
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
    });
  }
}

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
import { __param } from 'tslib';

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
  searchTerm: string = '';
  title: string = '';
  minPrice: number = 0;
  maxPrice: number = 0;
  categoryQuery: string = '';
  selectedCategoryNames: string[] = [];
  priceRange: { valueFrom: number; valueTo: number } = {} as {
    valueFrom: number;
    valueTo: number;
  };

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) {}

  // private getQueryParam(
  //   queryParams: ParamMap,
  //   param: string,
  //   type: string
  // ): void {
  //   if (queryParams.has(param)) {
  //     const parameter = queryParams.get(param);

  //     if (parameter != null) {
  //       if (type == 'str') {

  //       } else if (type == 'number') {

  //       } else {
  //         console.error(`getQueryParam: unsupported type ${type}`);
  //       }
  //     }
  //   }
  // }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      let page = 1;

      if (queryParams.has('page')) {
        const pageParam = queryParams.get('page');

        if (pageParam != null) {
          this.items.page = page;
        }
      }

      if (queryParams.has('title')) {
        const title = queryParams.get('title');

        if (title != null) {
          this.title = title;
        }
      }

      if (queryParams.has('minPrice')) {
        const minPrice = queryParams.get('minPrice');

        if (minPrice != null) {
          this.minPrice = Number(minPrice);
        }
      }

      if (queryParams.has('maxPrice')) {
        const maxPrice = queryParams.get('maxPrice');

        if (maxPrice != null) {
          this.maxPrice = Number(maxPrice);
        }
      }

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
        }${this.selectedCategoryNames && `&${this.categoryQuery}`}`
      )
      .subscribe({
        next: (response: PaginatedResponse<BasicItemResponse> | any) => {
          this.items = response;
          this.isLoading = false;
          console.log(this.items);
        },
        error: (error) => console.error(error),
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
    this.dialog.open(FiltersDialogComponent, { width: '50vw', height: '60vh' });
  }

  public onSearchSubmit(): void {
    this.isLoading = true;
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

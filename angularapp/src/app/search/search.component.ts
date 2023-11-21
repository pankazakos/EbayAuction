import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { FiltersService } from './filters.service';
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

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      let page = 1;

      if (queryParams.has('page')) {
        const pageParam = queryParams.get('page');

        if (pageParam != null) {
          if (Number(pageParam) > 1) {
            page = Number(pageParam);
          }
        }
        this.items.page = page;
      }

      if (queryParams.has('title')) {
        const title = queryParams.get('title');

        if (title != null) {
          this.title = title;
        }

        console.log('title: ' + this.title);
      }

      if (queryParams.has('minPrice')) {
        const minPrice = queryParams.get('minPrice');

        if (minPrice != null) {
          this.minPrice = Number(minPrice);
        }

        console.log('Min price: ' + this.minPrice);
      }

      if (queryParams.has('maxPrice')) {
        const maxPrice = queryParams.get('maxPrice');

        if (maxPrice != null) {
          this.maxPrice = Number(maxPrice);
        }

        console.log('Max price: ' + this.maxPrice);
      }

      if (queryParams.has('category')) {
        const categoryNames = queryParams.getAll('category');
        this.categoryQuery = categoryNames
          .map((categoryName) => `categories=${categoryName}`)
          .join('&');
      }

      this.fetchItems({
        page: 1,
      });
    });
  }

  private makeApiSearchCall(): void {
    this.http
      .get(
        `${ItemEndpoints.search}?page=${this.items.page}&limit=${
          this.items.limit
        }${this.title && `&title=${this.title}`}${
          this.priceRange &&
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

  private fetchItems(params: { page: number }): void {
    if (params.page !== this.items.page) {
      this.items.page = params.page;
    }

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
      this.fetchItems({ page: selectedPage });
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

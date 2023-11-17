import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { environment } from 'src/environments/environment';

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

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router
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
      this.fetchItems(page);
    });
  }

  public makeApiSearchCall(): void {
    console.log(this.searchTerm);

    this.http
      .get(
        `${ItemEndpoints.search}?page=${this.items.page}&limit=${
          this.items.limit
        }${this.searchTerm && `&title=${this.searchTerm}`}`
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

  fetchItems(page: number): void {
    if (page !== this.items.page) {
      this.items.page = page;
    }

    console.log('page is: ' + this.items.page);

    if (environment.production) {
      this.makeApiSearchCall();
    } else {
      setTimeout(() => {
        this.makeApiSearchCall();
      }, environment.timeout);
    }
  }

  onPageChange(event: PageEvent): void {
    this.isLoading = true;

    if (this.paginatorTop && this.paginatorBottom) {
      this.paginatorTop.pageIndex = event.pageIndex;
      this.paginatorBottom.pageIndex = event.pageIndex;
    }

    const selectedPage = event.pageIndex + 1;

    if (event.pageSize != this.items.limit) {
      this.items.limit = event.pageSize;
      this.fetchItems(selectedPage);
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

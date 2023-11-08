import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { PageEvent } from '@angular/material/paginator';
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

  private makeApiSearchCall(): void {
    this.http
      .get(
        `${ItemEndpoints.all}?page=${this.items.page}&limit=${this.items.limit}`
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

    if (event.pageSize != this.items.limit) {
      this.items.limit = event.pageSize;
      const page = event.pageIndex == 0 ? 1 : event.pageIndex;
      this.fetchItems(page);
      return;
    }

    const selectedPage = event.pageIndex + 1;
    this.items.page = selectedPage;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: selectedPage },
      queryParamsHandling: 'merge',
    });
  }
}

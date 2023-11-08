import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
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
  public pageLength: number = 100;
  public isLoading: boolean = true;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchItems();
    this.pageLength = Math.floor(this.items.total / this.items.limit) + 1;
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

  fetchItems(): void {
    const pageParamStr = this.route.snapshot.queryParamMap.get('page');

    let pageParam = 1;
    if (pageParamStr != null) {
      const converted = Number(pageParamStr);
      if (converted > 1) {
        pageParam = converted;
      }
    }

    console.log('page is: ' + Number(pageParamStr));

    this.items.page = pageParam;

    if (environment.production) {
      this.makeApiSearchCall();
    } else {
      setTimeout(() => {
        this.makeApiSearchCall();
      }, environment.timeout);
    }

    if (this.items.page > 1) {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { page: this.items.page },
        queryParamsHandling: 'merge',
      });
    }
  }

  onPageChange(event: PageEvent): void {
    this.isLoading = true;
    this.items.limit = event.pageSize;
    this.items.page = event.pageIndex + 1;
    this.fetchItems();
  }
}

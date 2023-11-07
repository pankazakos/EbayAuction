import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { baseUrl } from '../shared/types';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { PageEvent } from '@angular/material/paginator';

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

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchItems(this.items.page, this.items.limit);
    this.pageLength = Math.floor(this.items.total / this.items.limit) + 1;
  }

  fetchItems(page: number, limit: number): void {
    this.http.get(`${baseUrl}/item?page=${page}&limit=${limit}`).subscribe({
      next: (response: PaginatedResponse<BasicItemResponse> | any) => {
        this.items = response;
        this.isLoading = false;
        console.log(this.items);
      },
      error: (error) => console.error(error),
    });
  }

  onPageChange(event: PageEvent): void {
    this.isLoading = true;
    this.items.limit = event.pageSize;
    this.items.page = event.pageIndex + 1;
    this.fetchItems(this.items.page, this.items.limit);
  }
}

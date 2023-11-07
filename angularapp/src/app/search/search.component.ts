import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { baseUrl } from '../shared/types';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { Observer } from 'rxjs';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent {
  public items: PaginatedResponse<BasicItemResponse>;

  constructor(private http: HttpClient) {
    this.items = {
      castEntities: [],
      total: 0,
      page: 1,
      limit: 10,
    };
  }

  ngOnInit(): void {
    this.http.get(`${baseUrl}api/Item`).subscribe({
      next: (response: PaginatedResponse<BasicItemResponse>) => {
        this.items = response;
        console.log(this.items);
      },
      error: (error) => console.error(error),
    } as Partial<Observer<any>>);
  }
}

import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { baseUrl } from '../shared/types';
import { LimitedInfoPublishedItemResponse } from '../shared/contracts/responses/item';
import { Observer } from 'rxjs';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent {
  public items: LimitedInfoPublishedItemResponse[];

  constructor(private http: HttpClient) {
    this.items = [];
  }

  ngOnInit(): void {
    this.http.get(`${baseUrl}api/Item`).subscribe({
      next: (response: LimitedInfoPublishedItemResponse[]) => {
        this.items = response as LimitedInfoPublishedItemResponse[];
        console.log(this.items);
      },
      error: (error) => console.error(error),
    } as Partial<Observer<any>>);
  }
}

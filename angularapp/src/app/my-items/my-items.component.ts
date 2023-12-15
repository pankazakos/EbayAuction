import { Component } from '@angular/core';
import {
  BasicItemResponse,
  PublishedItemResponse,
} from '../shared/contracts/responses/item';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';

@Component({
  selector: 'app-my-items',
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss'],
})
export class MyItemsComponent {
  headers: HttpHeaders;
  inactiveItems: BasicItemResponse[];
  publishedItems: PublishedItemResponse[];
  itemsWithBids: BasicItemResponse[];

  updatedInactive: boolean = false;
  updatedPublished: boolean = false;
  updatedBidden: boolean = false;

  constructor(private http: HttpClient) {
    this.inactiveItems = [];
    this.publishedItems = [];
    this.itemsWithBids = [];
    this.headers = new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('accessToken')}`
    );
  }

  ngOnInit(): void {
    this.setInactiveItems();
  }

  onTabChanged(event: MatTabChangeEvent): void {
    const selectedIndex = event.index;

    switch (selectedIndex) {
      case 0:
        this.setInactiveItems();
        break;
      case 1:
        this.setPublishedItems();
        break;
      case 2:
        this.setItemsWithBids();
        break;
      default:
        break;
    }
  }

  setInactiveItems(): void {
    if (this.updatedInactive) return;

    this.http.get(ItemEndpoints.inactive, { headers: this.headers }).subscribe({
      next: (response: BasicItemResponse[] | any) => {
        console.log(response);

        this.inactiveItems = response;
        this.updatedInactive = true;
      },
      error: (error: any) => {
        console.log(error);
      },
    });
  }

  setPublishedItems(): void {
    if (this.updatedPublished) return;

    this.http.get(ItemEndpoints.active, { headers: this.headers }).subscribe({
      next: (response: PublishedItemResponse[] | any) => {
        console.log(response);

        this.publishedItems = response;
        this.updatedPublished = true;
      },
      error: (error: any) => console.error(error),
    });
  }

  setItemsWithBids(): void {
    if (this.updatedBidden) return;

    this.http.get(ItemEndpoints.bidden, { headers: this.headers }).subscribe({
      next: (response: BasicItemResponse[] | any) => {
        console.log(response);

        this.itemsWithBids = response;
        this.updatedBidden = true;
      },
      error: (error: any) => console.error(error),
    });
  }

  addItem(): void {
    console.log('add item');
  }
}

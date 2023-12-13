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

  constructor(private http: HttpClient) {
    this.inactiveItems = [];
    this.publishedItems = [];
    this.itemsWithBids = [];
    this.headers = new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('accessToken')}`
    );
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
    this.http.get(ItemEndpoints.inactive, { headers: this.headers }).subscribe({
      next: (response: BasicItemResponse[]) => {
        this.inactiveItems = response;
      },
      error: (error: any) => {
        console.log(error);
      },
    } as Partial<any>);
  }

  setPublishedItems(): void {
    this.http.get(ItemEndpoints.active, { headers: this.headers }).subscribe({
      next: (response: PublishedItemResponse[]) => {
        this.publishedItems = response;
      },
      error: (error: any) => console.error(error),
    } as Partial<any>);
  }

  setItemsWithBids(): void {
    this.http.get(ItemEndpoints.bidden, { headers: this.headers }).subscribe({
      next: (response: BasicItemResponse[]) => {
        this.itemsWithBids = response;
      },
      error: (error: any) => console.error(error),
    } as Partial<any>);
  }

  addItem(): void {
    this.http.post(
      ItemEndpoints.create,
      // { body: {} },
      { headers: this.headers }
    );
  }
}

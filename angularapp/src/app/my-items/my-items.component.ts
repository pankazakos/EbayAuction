import { Component } from '@angular/core';
import {
  GenericItemResponse,
  PublishedItemResponse,
} from '../shared/contracts/responses/item';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { baseUrl } from '../shared/types';
import { MatTabChangeEvent } from '@angular/material/tabs';

@Component({
  selector: 'app-my-items',
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.css'],
})
export class MyItemsComponent {
  headers: HttpHeaders;
  inactiveItems: GenericItemResponse[];
  publishedItems: PublishedItemResponse[];
  itemsWithBids: GenericItemResponse[];

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
    this.http
      .get(`${baseUrl}api/item/inactive`, { headers: this.headers })
      .subscribe({
        next: (response: GenericItemResponse[]) => {
          this.inactiveItems = response;
        },
        error: (error: any) => {
          console.log(error);
        },
      } as Partial<any>);
  }

  setPublishedItems(): void {
    this.http
      .get(`${baseUrl}api/item/active`, { headers: this.headers })
      .subscribe({
        next: (response: PublishedItemResponse[]) => {
          this.publishedItems = response;
        },
        error: (error: any) => console.error(error),
      } as Partial<any>);
  }

  setItemsWithBids(): void {
    this.http
      .get(`${baseUrl}api/item/bidden`, { headers: this.headers })
      .subscribe({
        next: (response: GenericItemResponse[]) => {
          this.itemsWithBids = response;
        },
        error: (error: any) => console.error(error),
      } as Partial<any>);
  }

  addItem(): void {
    this.http.post(
      `${baseUrl}api/item`,
      // { body: {} },
      { headers: this.headers }
    );
  }
}

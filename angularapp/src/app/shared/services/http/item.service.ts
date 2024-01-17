import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BasicItemResponse } from '../../contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { ItemEndpoints } from '../../contracts/endpoints/ItemEndpoints';

@Injectable({
  providedIn: 'root',
})
export class ItemService {
  constructor(private http: HttpClient) {}

  getItem(itemId: number): Observable<BasicItemResponse> {
    return this.http.get<BasicItemResponse>(`${ItemEndpoints.getById(itemId)}`);
  }
}

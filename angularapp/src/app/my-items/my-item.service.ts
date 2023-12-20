import { Injectable } from '@angular/core';
import { AddItemRequest } from '../shared/contracts/requests/item';

@Injectable({
  providedIn: 'root',
})
export class MyItemService {
  addItemForm: AddItemRequest = {} as AddItemRequest;
  addItemImageFilename: string = '';
  addItemImageFile: File | null = null;

  constructor() {}
}

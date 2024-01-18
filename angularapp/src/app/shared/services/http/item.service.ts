import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  BasicItemResponse,
  ExtendedItemInfo,
} from '../../contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { ItemEndpoints } from '../../contracts/endpoints/ItemEndpoints';
import { DateTimeFormatService } from '../common/date-time-format.service';

@Injectable({
  providedIn: 'root',
})
export class ItemService {
  constructor(
    private http: HttpClient,
    private dateTimeFormatter: DateTimeFormatService
  ) {}

  getById(itemId: number): Observable<BasicItemResponse> {
    return this.http.get<BasicItemResponse>(`${ItemEndpoints.getById(itemId)}`);
  }

  getImage(imageGuid: string): Observable<Blob> {
    return this.http.get(`${ItemEndpoints.getImage(imageGuid)}`, {
      responseType: 'blob',
    });
  }

  handleObservable<T>(observable: Observable<T>): Promise<T> {
    return new Promise((resolve, reject) => {
      observable.subscribe({
        next: (response: T) => {
          resolve(response);
        },
        error: (error) => {
          console.error(error);
          reject();
        },
      });
    });
  }

  getBasicItemResponse(itemId: number): Promise<BasicItemResponse> {
    return this.handleObservable(this.getById(itemId));
  }

  async getExtendedItemResponse(itemId: number): Promise<ExtendedItemInfo> {
    let item: ExtendedItemInfo;

    return new Promise<ExtendedItemInfo>((resolve, reject) => {
      this.handleObservable(this.getById(itemId))
        .then((response) => {
          item = {
            ...response,
            image: { src: '', isLoading: false },
            auctionStarted: this.dateTimeFormatter.convertOnlyToDate(
              response.started
            ),
            auctionEnds: this.dateTimeFormatter.convertOnlyToDate(
              response.ends
            ),
          };

          this.handleObservable(this.getImage(item.imageGuid))
            .then((response: Blob) => {
              const blob = new Blob([response], {
                type: 'image/jpeg',
              });
              const imageUrl = URL.createObjectURL(blob);
              item.image = {
                src: imageUrl,
                isLoading: false,
              };
              resolve(item);
            })
            .catch((error) => {
              console.error(error);
              reject();
            });
        })
        .catch((error) => {
          console.error(error);
          reject();
        });
    });
  }
}

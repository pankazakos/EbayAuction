import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BidEndpoints } from '../../contracts/endpoints/BidEndpoints';
import { AuthService } from '../authentication/auth-service.service';
import {
  BasicBidResponse,
  ExtendedBidInfo,
} from '../../contracts/responses/bid';
import { Observable } from 'rxjs';

export type OrderType = 'ascending' | 'descending';
export type OrderByOption = 'time' | 'amount';

@Injectable({
  providedIn: 'root',
})
export class BidService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getUserBids(
    orderType: OrderType = 'ascending',
    orderByOption: OrderByOption = 'time'
  ): Observable<BasicBidResponse[]> {
    return this.http.get<BasicBidResponse[]>(
      `${BidEndpoints.myBids}?orderType=${orderType}&orderByOption=${orderByOption}`,
      {
        headers: this.authService.getHeaders(),
      }
    );
  }

  getFullInfoUserBids(
    orderType: OrderType = 'ascending',
    orderByOption: OrderByOption = 'time'
  ): Observable<ExtendedBidInfo[]> {
    return this.http.get<ExtendedBidInfo[]>(
      `${BidEndpoints.extendedMyBids}?orderType=${orderType}&orderByOption=${orderByOption}`,
      {
        headers: this.authService.getHeaders(),
      }
    );
  }
}

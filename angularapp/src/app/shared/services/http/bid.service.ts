import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BidEndpoints } from '../../contracts/endpoints/BidEndpoints';
import { AuthService } from '../authentication/auth-service.service';
import {
  BasicBidResponse,
  ExtendedBidInfo,
} from '../../contracts/responses/bid';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BidService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getUserBids(): Observable<BasicBidResponse[]> {
    return this.http.get<BasicBidResponse[]>(`${BidEndpoints.myBids}`, {
      headers: this.authService.getHeaders(),
    });
  }

  getFullInfoUserBids(): Observable<ExtendedBidInfo[]> {
    return this.http.get<ExtendedBidInfo[]>(`${BidEndpoints.extendedMyBids}`, {
      headers: this.authService.getHeaders(),
    });
  }
}

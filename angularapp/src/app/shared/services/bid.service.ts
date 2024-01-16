import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BidEndpoints } from '../contracts/endpoints/BidEndpoints';
import { AuthService } from './auth-service.service';
import { BasicBidResponse } from '../contracts/responses/bid';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BidService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getUsersBids(): Observable<BasicBidResponse[]> {
    return this.http.get<BasicBidResponse[]>(`${BidEndpoints.myBids}`, {
      headers: this.authService.getHeaders(),
    });
  }
}

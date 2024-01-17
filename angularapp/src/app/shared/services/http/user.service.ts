import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BasicUserResponse } from '../../contracts/responses/user';
import { UserEndpoints } from '../../contracts/endpoints/UserEndpoints';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}

  getUsername(userId: number): Observable<BasicUserResponse> {
    return this.http.get<BasicUserResponse>(`${UserEndpoints.IdToUsername(userId)}`);
  }
}

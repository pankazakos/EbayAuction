// src/app/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { UserRole } from '../types';
import { LoginUserResponse } from '../contracts/responses/user';
import { UserEndpoints } from '../contracts/endpoints/UserEndpoints';
import { UserCredentialsRequest } from '../contracts/requests/user';

export interface AuthData {
  username: string;
  role: UserRole;
  isLoggedIn: boolean;
}

const tokenKeyName = 'accessToken';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private headers: HttpHeaders = new HttpHeaders();

  private authDataSubject = new BehaviorSubject<AuthData>({
    username: '',
    role: UserRole.Anonymous,
    isLoggedIn: false,
  });
  authData$ = this.authDataSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  private readJwt(): string {
    const token = localStorage.getItem(tokenKeyName);
    if (token == null) {
      return '';
    }

    const decodedJWT = JSON.parse(window.atob(token.split('.')[1]));

    return decodedJWT;
  }

  getCurrentAuthData(): AuthData {
    return this.authDataSubject.getValue();
  }

  getHeaders(): HttpHeaders {
    return this.headers;
  }

  setAuthData(): void {
    const decodedJWT: any = this.readJwt();

    if (decodedJWT === '') {
      return;
    }

    const username = decodedJWT.username;
    const isSuperUser = decodedJWT.IsSuperUser;

    const expiryDate = decodedJWT.exp * 1000; // Convert to milliseconds

    const now = new Date().getTime();
    const timeout = expiryDate - now;

    if (timeout < 0) {
      alert('Your session has expired. Please login again');
      this.logoutUser();
      return;
    }

    setTimeout(() => {
      this.logoutUser();
    }, timeout);

    const newAuthData: AuthData = {
      username: username,
      role: isSuperUser ? UserRole.Admin : UserRole.User,
      isLoggedIn: true,
    };
    this.authDataSubject.next(newAuthData);

    this.headers.set(
      'Authorization',
      `Bearer ${localStorage.getItem(tokenKeyName)}`
    );
  }

  loginUser(username: string, password: string) {
    const credentials: UserCredentialsRequest = { username, password };
    this.http
      .post<LoginUserResponse>(UserEndpoints.login, credentials)
      .subscribe({
        next: (response) => {
          localStorage.setItem(tokenKeyName, response.accessToken);
          this.setAuthData();
          this.router.navigate(['/']);
        },
        error: (error) => {
          if (error.status === 404) {
            alert('invalid username');
          } else if (error.status === 400) {
            alert('incorrect password');
          } else {
            alert('unexpected error. Please try again later');
          }
        },
      });
  }

  logoutUser() {
    localStorage.removeItem(tokenKeyName);
    this.authDataSubject.next({
      username: '',
      role: UserRole.Anonymous,
      isLoggedIn: false,
    });
    this.router.navigate(['/']);
  }
}

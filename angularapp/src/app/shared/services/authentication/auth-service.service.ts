// src/app/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { UserRole } from '../../types';
import { LoginUserResponse } from '../../contracts/responses/user';
import { UserEndpoints } from '../../contracts/endpoints/UserEndpoints';
import { UserCredentialsRequest } from '../../contracts/requests/user';
import { AlertService } from '../common/alert.service';

export interface AuthData {
  username: string;
  role: UserRole;
  isLoggedIn: boolean;
}

interface DecodedJwt {
  username: string;
  IsSuperuser: string;
  exp: number;
  iss: string;
  aud: string;
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

  constructor(
    private http: HttpClient,
    private router: Router,
    private alertService: AlertService
  ) {}

  private decodeJwt(): DecodedJwt | null {
    const token = localStorage.getItem(tokenKeyName);
    if (token == null) {
      return null;
    }

    const decodedJWT = JSON.parse(window.atob(token.split('.')[1]));

    return decodedJWT;
  }

  private handleSessionTimeout(): void {
    this.alertService.error(
      'Your session has expired. Please login again',
      'Close'
    );
    this.logoutUser();
  }

  getCurrentAuthData(): AuthData {
    return this.authDataSubject.getValue();
  }

  getHeaders(): HttpHeaders {
    return this.headers;
  }

  setAuthData(): void {
    const decodedJWT: DecodedJwt | null = this.decodeJwt();

    if (decodedJWT == null) {
      return;
    }

    const username = decodedJWT.username;
    const isSuperuser = decodedJWT.IsSuperuser;

    const expiryDate = decodedJWT.exp * 1000; // Convert to milliseconds

    const now = new Date().getTime();
    const timeout = expiryDate - now;

    if (timeout < 0) {
      this.handleSessionTimeout();
      return;
    }

    setTimeout(() => {
      this.handleSessionTimeout();
    }, timeout);

    const newAuthData: AuthData = {
      username: username,
      role: isSuperuser === 'True' ? UserRole.Admin : UserRole.User,
      isLoggedIn: true,
    };
    this.authDataSubject.next(newAuthData);

    this.headers = this.headers.set(
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
            this.alertService.error('Invalid username', 'Close');
          } else if (error.status === 400) {
            this.alertService.error('Incorrect password', 'Close');
          } else {
            this.alertService.internalError();
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

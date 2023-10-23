// src/app/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserCredentialsRequest } from './contracts/requests/user';
import { LoginUserResponse } from './contracts/responses/other';
import { BehaviorSubject } from 'rxjs';

export interface AuthData {
  username: string;
  role: string;
  isLoggedIn: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient, private router: Router) {}

  private authDataSubject = new BehaviorSubject<AuthData>({
    username: '',
    role: '',
    isLoggedIn: false,
  });
  authData$ = this.authDataSubject.asObservable();

  LoginUser(username: string, password: string) {
    const credentials: UserCredentialsRequest = { username, password };
    this.http
      .post<LoginUserResponse>(
        'https://localhost:7068/api/User/login',
        credentials
      )
      .subscribe({
        next: (response) => {
          localStorage.setItem('accessToken', response.accessToken);
          this.authDataSubject.next({
            username: username,
            role: 'user',
            isLoggedIn: true,
          });
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
    localStorage.removeItem('accessToken');
    this.authDataSubject.next({ username: '', role: '', isLoggedIn: false });
  }
}

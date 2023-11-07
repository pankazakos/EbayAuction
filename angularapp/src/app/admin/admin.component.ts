import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { baseUrl } from '../shared/types';
import { DateTimeFormatService } from '../shared/date-time-format.service';
import { BasicUserResponse } from '../shared/contracts/responses/user';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
})
export class AdminComponent {
  users: BasicUserResponse[] = [];
  displayedColumns: string[] = [
    'id',
    'username',
    'firstName',
    'lastName',
    'lastLogin',
    'dateJoined',
    'email',
    'country',
    'location',
    'isSuperuser',
    'isActive',
    'delete',
  ];

  headers: HttpHeaders;

  constructor(
    private http: HttpClient,
    private dateTimeFormat: DateTimeFormatService
  ) {
    this.headers = new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('accessToken')}`
    );
  }

  ngOnInit(): void {
    this.http.get(`${baseUrl}/user/all`, { headers: this.headers }).subscribe({
      next: (response: BasicUserResponse[]) => {
        response.map((user) => {
          user.lastLogin =
            user.lastLogin !== null
              ? this.dateTimeFormat.formatDatetime(user.lastLogin.toString())
              : '';
        });
        this.users = response;
      },
      error: (error: any) => {
        console.error('API Error: ', error);
      },
    } as Partial<any>);
  }

  deleteUser(userId: number) {
    this.http
      .delete(`${baseUrl}/user/${userId}`, { headers: this.headers })
      .subscribe({
        next: () => {
          this.users = this.users.filter((user) => user.id != userId);
        },
        error: (error) => {
          console.error('API error: ', error);
        },
      });
  }
}

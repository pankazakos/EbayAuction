import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { DateTimeFormatService } from '../shared/date-time-format.service';
import { BasicUserResponse } from '../shared/contracts/responses/user';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { UserEndpoints } from '../shared/contracts/endpoints/UserEndpoints';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
})
export class AdminComponent {
  users: PaginatedResponse<BasicUserResponse>;
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
    this.users = {
      castEntities: [],
      total: 0,
      page: 1,
      limit: 10,
    };
  }

  ngOnInit(): void {
    this.http.get(UserEndpoints.all, { headers: this.headers }).subscribe({
      next: (response: PaginatedResponse<BasicUserResponse>) => {
        response.castEntities.map((user) => {
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
      .delete(UserEndpoints.getById(userId), { headers: this.headers })
      .subscribe({
        next: () => {
          this.users.castEntities = this.users.castEntities.filter(
            (user) => user.id != userId
          );
        },
        error: (error) => {
          console.error('API error: ', error);
        },
      });
  }
}

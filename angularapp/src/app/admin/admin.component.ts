import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { DateTimeFormatService } from '../shared/services/date-time-format.service';
import { BasicUserResponse } from '../shared/contracts/responses/user';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { UserEndpoints } from '../shared/contracts/endpoints/UserEndpoints';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmComponent } from '../shared/components/confirm/confirm.component';
import { AlertService } from '../shared/services/alert.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
})
export class AdminComponent {
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

  users: PaginatedResponse<BasicUserResponse> = {
    castEntities: [],
    total: 0,
    page: 1,
    limit: 10,
  };

  isLoading: boolean = true;

  headers: HttpHeaders = new HttpHeaders().set(
    'Authorization',
    `Bearer ${localStorage.getItem('accessToken')}`
  );

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private dateTimeFormat: DateTimeFormatService,
    private confirmDialog: MatDialog,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.users.page = 1;

    // set new value of page
    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.isLoading = true;

      if (queryParams.has('page')) {
        const value = queryParams.get('page');
        if (value != null) {
          this.users.page = Number(value);
        }
      }

      if (this.users.page == 1) {
        this.router.navigate([], {
          relativeTo: this.route,
        });
      }

      this.delayApiCall(this.fetchUsers.bind(this));
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.syncPaginators();
    });
  }

  private syncPaginators(): void {
    if (this.paginatorTop && this.paginatorBottom) {
      this.paginatorTop.pageIndex = this.users.page - 1;
      this.paginatorBottom.pageIndex = this.users.page - 1;
    }
  }

  private delayApiCall(apiCall: () => void): void {
    if (environment.production) {
      apiCall();
    } else {
      setTimeout(() => {
        apiCall();
      }, environment.timeout);
    }
  }

  private fetchUsers(): void {
    this.http
      .get(
        `${UserEndpoints.all}?page=${this.users.page}&limit=${this.users.limit}`,
        { headers: this.headers }
      )
      .subscribe({
        next: (response: PaginatedResponse<BasicUserResponse> | any) => {
          response.castEntities.map((user: BasicUserResponse) => {
            user.lastLogin =
              user.lastLogin !== null
                ? this.dateTimeFormat.formatDatetime(user.lastLogin.toString())
                : '';
            user.dateJoined =
              user.dateJoined !== null
                ? this.dateTimeFormat.formatDatetime(user.dateJoined.toString())
                : '';
          });
          this.users = response;
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('API Error: ', error);
        },
      });
  }

  onPageChange(event: PageEvent): void {
    this.isLoading = true;
    if (event.pageSize != this.users.limit) {
      this.users.limit = event.pageSize;
      this.syncPaginators();
      this.delayApiCall(this.fetchUsers.bind(this));
      return;
    }
    const selectedPage = event.pageIndex + 1;
    this.users.page = selectedPage;
    this.syncPaginators();

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: selectedPage },
      queryParamsHandling: 'merge',
    });
  }

  confirmDeleteUser(user: BasicUserResponse) {
    const confirmDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        question: `Are you sure you want to delete user ${user.userName} with id ${user.id}`,
      },
    });

    confirmDialogRef.afterClosed().subscribe((result) => {
      if (result == 'confirm') {
        this.deleteUser(user.id);
      }
    });
  }

  deleteUser(userId: number) {
    this.http
      .delete(UserEndpoints.getById(userId), { headers: this.headers })
      .subscribe({
        next: () => {
          this.users.castEntities = this.users.castEntities.filter(
            (user) => user.id != userId
          );
          this.alertService.success('Successfully deleted user', 'Ok');
        },
        error: (error) => {
          console.error('API error: ', error);
          if (error.status == 401 || error.status == 403) {
            this.alertService.error(
              'You are not authorized to delete users',
              'Close'
            );
          }
          this.alertService.error(
            'There was an error while attempting to delete user',
            'Close'
          );
        },
      });
  }
}

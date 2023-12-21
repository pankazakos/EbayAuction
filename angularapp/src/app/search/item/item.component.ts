import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Component, Inject, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserEndpoints } from 'src/app/shared/contracts/endpoints/UserEndpoints';
import { BasicItemResponse } from 'src/app/shared/contracts/responses/item';
import { IdToUsernameResponse } from 'src/app/shared/contracts/responses/user';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DateTimeFormatService } from 'src/app/shared/services/date-time-format.service';
import { BasicBidResponse } from 'src/app/shared/contracts/responses/bid';
import { AddBidRequest } from 'src/app/shared/contracts/requests/bid';
import { ConfirmBidDialogComponent } from './confirm-bid-dialog/confirm-bid-dialog.component';
import {
  AuthData,
  AuthService,
} from 'src/app/shared/services/auth-service.service';
import { BidEndpoints } from 'src/app/shared/contracts/endpoints/BidEndpoints';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';
import { AlertService } from 'src/app/shared/services/alert.service';

type loadingItem = { data: BasicItemResponse; isLoading: boolean };
type loadingImage = { src: string; isLoading: boolean };

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
})
export class ItemComponent {
  seller: IdToUsernameResponse = {} as IdToUsernameResponse;
  item: loadingItem = {} as loadingItem;
  image: loadingImage = {} as loadingImage;
  isItemInactive: boolean = false;
  headers: HttpHeaders;
  bid: BasicBidResponse = {} as BasicBidResponse;
  categories: BasicCategoryResponse[] = [];
  joinedCategories: string = '';

  auctionStarted: string = '';
  auctionEnds: string = '';

  authData: AuthData | null = null;

  @ViewChild('bidForm') bidForm!: NgForm;

  constructor(
    private http: HttpClient,
    private confirmBidDialog: MatDialog,
    private formatter: DateTimeFormatService,
    private authService: AuthService,
    private alertService: AlertService,
    @Inject(MAT_DIALOG_DATA) public dialogInputData: any
  ) {
    this.item.data = dialogInputData.item;
    this.image = dialogInputData.image;
    this.isItemInactive = dialogInputData.isItemInactive;
    this.headers = new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('accessToken')}`
    );
  }

  ngOnInit(): void {
    this.authService.authData$.subscribe(
      (authData) => (this.authData = authData)
    );

    this.auctionStarted = this.item.data.started
      ? this.formatter.formatDatetime(this.item.data.started.toString())
      : '';
    this.auctionEnds = this.item.data.ends
      ? this.formatter.formatDatetime(this.item.data.ends.toString())
      : '';

    this.http
      .get(UserEndpoints.IdToUsername(this.item.data.sellerId))
      .subscribe({
        next: (response: IdToUsernameResponse | any) => {
          this.seller = response;
        },
        error: (error) => {
          console.error(error);
        },
      });

    this.http.get(ItemEndpoints.categories(this.item.data.itemId)).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categories = response;
        this.joinedCategories = this.categories
          .map((category) => category.name)
          .join(', ');
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  private openInvalidBidAlert(): void {
    this.alertService.error('Invalid bid', 'Close');
  }

  placeBid(): void {
    const bid = this.bidForm.value as AddBidRequest;

    if (
      bid.amount == Number('') ||
      bid.amount < this.item.data.currently ||
      (bid.amount <= this.item.data.currently && this.item.data.numBids > 0)
    ) {
      this.openInvalidBidAlert();
      return;
    }

    this.confirmBid(bid);
  }

  private confirmBid(bid: AddBidRequest): void {
    const confirmDialogRef = this.confirmBidDialog.open(
      ConfirmBidDialogComponent,
      {
        autoFocus: false,
        disableClose: true,
        data: {
          itemName: this.item.data.name,
          bidAmount: this.bidForm.value.amount,
        },
      }
    );

    confirmDialogRef.afterClosed().subscribe((result) => {
      if (result === 'confirm') {
        this.http
          .post<AddBidRequest>(
            BidEndpoints.create,
            {
              itemId: this.item.data.itemId,
              amount: bid.amount,
            },
            {
              headers: this.headers,
            }
          )
          .subscribe({
            next: (response: BasicBidResponse | any) => {
              this.bid = response;
              this.item.data.currently = this.bid.amount;
              this.item.data.numBids += 1;
              this.alertService.success('Bid successful!', 'Ok');
              this.bidForm.reset();
            },
            error: (error: HttpErrorResponse) => {
              console.error(error);
              if (error.status === 400) {
                this.openInvalidBidAlert();
              } else if (error.status === 500) {
                this.alertService.internalError();
              }
            },
          });
      }
    });
  }

  isOwner(): boolean {
    return this.authData?.username === this.seller.username;
  }
}

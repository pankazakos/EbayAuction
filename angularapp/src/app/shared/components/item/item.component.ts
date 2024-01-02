import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { UserEndpoints } from 'src/app/shared/contracts/endpoints/UserEndpoints';
import { BasicItemResponse } from 'src/app/shared/contracts/responses/item';
import { IdToUsernameResponse } from 'src/app/shared/contracts/responses/user';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { DateTimeFormatService } from 'src/app/shared/services/date-time-format.service';
import { BasicBidResponse } from 'src/app/shared/contracts/responses/bid';
import { AddBidRequest } from 'src/app/shared/contracts/requests/bid';
import {
  AuthData,
  AuthService,
} from 'src/app/shared/services/auth-service.service';
import { BidEndpoints } from 'src/app/shared/contracts/endpoints/BidEndpoints';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';
import { AlertService } from 'src/app/shared/services/alert.service';
import { BidStepService } from 'src/app/shared/services/bid-step.service';
import { ConfirmComponent } from 'src/app/shared/components/confirm/confirm.component';
import {
  MatDatepicker,
  MatDatepickerInputEvent,
} from '@angular/material/datepicker';
import { DatePipe } from '@angular/common';
import { MyItemService } from 'src/app/my-items/services/my-item.service';

type loadingItem = { data: BasicItemResponse; isLoading: boolean };
type loadingImage = { src: string; isLoading: boolean };

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
})
export class ItemComponent implements AfterViewInit {
  seller: IdToUsernameResponse = {} as IdToUsernameResponse;
  item: loadingItem = {} as loadingItem;
  image: loadingImage = {} as loadingImage;
  headers: HttpHeaders;
  bid: BasicBidResponse = {} as BasicBidResponse;
  categories: BasicCategoryResponse[] = [];
  joinedCategories: string = '';

  auctionStarted: string = '';
  auctionEnds: string = '';

  authData: AuthData | null = null;
  bidStep: number = 0;

  isItemInactive: boolean = false;
  openCalendar: boolean = false;

  currentDate: Date = new Date();
  minDate: Date = this.currentDate;
  maxDate: Date = new Date(
    this.currentDate.getFullYear() + 1,
    this.currentDate.getMonth(),
    this.currentDate.getDate()
  );

  inputDate: Date = this.currentDate;
  inputTime: string = '23:59';
  selectedTime: string | null = null;

  @ViewChild('bidForm') bidForm!: NgForm;
  @ViewChild('picker') datepicker!: MatDatepicker<Date>;

  constructor(
    private http: HttpClient,
    private selfDialogRef: MatDialogRef<ItemComponent>,
    private confirmDialog: MatDialog,
    private formatter: DateTimeFormatService,
    private authService: AuthService,
    private alertService: AlertService,
    private bidStepService: BidStepService,
    private datePipe: DatePipe,
    private myItemService: MyItemService,
    @Inject(MAT_DIALOG_DATA) public dialogInputData: any
  ) {
    this.item.data = dialogInputData.item;
    this.image = dialogInputData.image;
    this.isItemInactive = dialogInputData.isItemInactive;
    this.openCalendar = dialogInputData.openCalendar;
    this.headers = new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('accessToken')}`
    );
    this.setExpiryDatetime();
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

    this.bidStep = this.bidStepService.getBidStep(this.item.data.currently);

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

  ngAfterViewInit(): void {
    if (this.isItemInactive && this.openCalendar) {
      setTimeout(() => {
        this.datepicker.open();
      }, 100);
    }
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
    const confirmDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      disableClose: true,
      data: {
        question: `Are you sure you want to place a bid of ${this.bidForm.value.amount} on ${this.item.data.name}?`,
      },
    });

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

  private setExpiryDatetime(): void {
    const formattedDate = this.datePipe.transform(this.inputDate, 'MM-dd-yyyy');
    this.myItemService.expiryDatetime = `${formattedDate}T${this.inputTime}`;
  }

  applyDateTime(): void {
    // Date is automatically updated from the datepicker.
    this.inputTime = this.selectedTime ? this.selectedTime : '23:59'; // Update input time
  }

  onSelectDate(event: MatDatepickerInputEvent<Date>): void {
    if (event.value) {
      const formattedDate = this.datePipe.transform(event.value, 'MM-dd-yyyy');
      this.inputDate = event.value;
      this.setExpiryDatetime();
    }
  }

  updateSelectedTime(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedTime = input.value;
  }

  confirmPublish(): void {
    const formattedDate = this.datePipe.transform(this.inputDate, 'yyyy-MM-dd');

    this.myItemService.expiryDatetime = `${formattedDate}T${this.inputTime}`;

    const confirmDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      restoreFocus: false,
      disableClose: true,
      data: {
        question: `Are you sure you want to start an auction for ${
          this.item.data.name
        } 
        that ends on ${this.myItemService.expiryDatetime.replace('T', ' ')}? 
        This action cannot be undone.`,
      },
    });

    confirmDialogRef.afterClosed().subscribe((result) => {
      if (result == 'confirm') {
        this.selfDialogRef.close('publish');
      }
    });
  }
}

import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { ItemComponent } from 'src/app/shared/components/item/item.component';
import { ExtendedBidInfo } from 'src/app/shared/contracts/responses/bid';
import { ExtendedItemInfo } from 'src/app/shared/contracts/responses/item';
import { DateTimeFormatService } from 'src/app/shared/services/common/date-time-format.service';
import {
  BidService,
  OrderByOption,
  OrderType,
} from 'src/app/shared/services/http/bid.service';
import { ItemService } from 'src/app/shared/services/http/item.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-bid-history',
  templateUrl: './bid-history.component.html',
  styleUrls: ['./bid-history.component.scss'],
})
export class BidHistoryComponent {
  displayedColumns: string[] = [
    'No',
    'Bid Amount',
    'DateTime',
    'Item Title',
    'Seller',
    'Auction Status',
    'Show Item',
  ];

  orderType: OrderType = 'ascending';
  orderByOption: OrderByOption = 'time';

  isLoading: boolean = true;
  userBids: ExtendedBidInfo[] = [];
  selectedItemToShow: ExtendedItemInfo = {} as ExtendedItemInfo;

  constructor(
    private bidService: BidService,
    private itemService: ItemService,
    private selectedItemDialog: MatDialog,
    private dateTimeFormatter: DateTimeFormatService
  ) {}

  ngOnInit(): void {}

  private processBidResponse(bids: ExtendedBidInfo[]): ExtendedBidInfo[] {
    bids.forEach((bid) => {
      bid.time = this.dateTimeFormatter.formatDatetimeUpToSeconds(bid.time);
    });
    return bids;
  }

  loadBidHistory(
    orderType: OrderType = 'ascending',
    orderByOption: OrderByOption = 'time'
  ): void {
    this.isLoading = true;

    setTimeout(() => {
      this.bidService.getFullInfoUserBids(orderType, orderByOption).subscribe({
        next: (bids: ExtendedBidInfo[]) => {
          console.log(bids);
          this.userBids = this.processBidResponse(bids);
          this.isLoading = false;
        },
        error: (error) => {
          console.log(error);
        },
      });
    }, environment.timeout);
  }

  toggleOrderType(): void {
    this.orderType =
      this.orderType === 'ascending' ? 'descending' : 'ascending';

    this.loadBidHistory(this.orderType, this.orderByOption);
  }

  onSelectOrderBy(event: MatSelectChange): void {
    this.orderByOption = event.value;

    console.log('order by: ' + this.orderByOption);

    this.loadBidHistory(this.orderType, this.orderByOption);
  }

  showItem(itemId: number): void {
    this.itemService
      .getExtendedItemResponse(itemId)
      .then((response: ExtendedItemInfo) => {
        this.selectedItemToShow = response;

        this.selectedItemDialog.open(ItemComponent, {
          autoFocus: false,
          restoreFocus: false,
          data: {
            item: this.selectedItemToShow,
            image: this.selectedItemToShow.image,
          },
        });
      });
  }
}

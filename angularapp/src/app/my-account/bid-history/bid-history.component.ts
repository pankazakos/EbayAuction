import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ItemComponent } from 'src/app/shared/components/item/item.component';
import { ExtendedBidInfo } from 'src/app/shared/contracts/responses/bid';
import { ExtendedItemInfo } from 'src/app/shared/contracts/responses/item';
import { DateTimeFormatService } from 'src/app/shared/services/common/date-time-format.service';
import { BidService } from 'src/app/shared/services/http/bid.service';
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
      bid.time = this.dateTimeFormatter.formatDatetime(bid.time);
    });
    return bids;
  }

  loadBidHistory(): void {
    setTimeout(() => {
      this.bidService.getFullInfoUserBids().subscribe({
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

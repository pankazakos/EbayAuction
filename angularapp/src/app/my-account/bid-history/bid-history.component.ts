import { Component } from '@angular/core';
import {
  BasicBidResponse,
  ExtendedBidInfo,
} from 'src/app/shared/contracts/responses/bid';
import { DateTimeFormatService } from 'src/app/shared/services/common/date-time-format.service';
import { BidService } from 'src/app/shared/services/http/bid.service';
import { UserService } from 'src/app/shared/services/http/user.service';
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

  constructor(
    private bidService: BidService,
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
}

import { Component } from '@angular/core';
import { BidService } from 'src/app/shared/services/http/bid.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-bid-history',
  templateUrl: './bid-history.component.html',
  styleUrls: ['./bid-history.component.scss'],
})
export class BidHistoryComponent {
  isLoading: boolean = true;

  constructor(private bidService: BidService) {}

  loadBidHistory(): void {
    setTimeout(() => {
      this.bidService.getUsersBids().subscribe({
        next: (data) => {
          console.log(data);
          this.isLoading = false;
        },
        error: (error) => {
          console.log(error);
        },
      });
    }, environment.timeout);
  }
}

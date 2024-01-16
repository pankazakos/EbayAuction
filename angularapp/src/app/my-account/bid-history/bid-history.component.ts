import { Component } from '@angular/core';
import { BidService } from 'src/app/shared/services/http/bid.service';

@Component({
  selector: 'app-bid-history',
  templateUrl: './bid-history.component.html',
  styleUrls: ['./bid-history.component.scss'],
})
export class BidHistoryComponent {
  constructor(private bidService: BidService) {}

  ngOnInit(): void {
    this.bidService.getUsersBids().subscribe({
      next: (data) => {
        console.log(data);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
}

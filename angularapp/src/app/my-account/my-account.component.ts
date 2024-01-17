import { Component, ViewChild } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { BidHistoryComponent } from './bid-history/bid-history.component';
import { PersonalInfoComponent } from './personal-info/personal-info.component';

@Component({
  selector: 'app-my-account',
  templateUrl: './my-account.component.html',
  styleUrls: ['./my-account.component.scss'],
})
export class MyAccountComponent {
  displayedColumns: string[] = [
    'Bid Amount',
    'DateTime',
    'Item Title',
    'Seller',
    'Show Item',
  ];

  @ViewChild(PersonalInfoComponent) personalInfoTab!: PersonalInfoComponent;
  @ViewChild(BidHistoryComponent) bidHistoryTab!: BidHistoryComponent;

  personalInfoFetchGuard: boolean = false;
  bidHistoryFetchGuard: boolean = false;

  constructor() {}

  onTabChange(event: MatTabChangeEvent): void {
    const selectedTabIndex = event.index;

    switch (selectedTabIndex) {
      case 0:
        if (!this.personalInfoFetchGuard && this.personalInfoTab) {
          this.personalInfoTab.loadPersonalInfo();
          this.personalInfoFetchGuard = true;
        }
        break;
      case 1:
        if (!this.bidHistoryFetchGuard && this.bidHistoryTab) {
          this.bidHistoryTab.loadBidHistory();
          this.bidHistoryFetchGuard = true;
        }
        break;
      default:
        break;
    }
  }
}

import { Injectable } from '@angular/core';
import { BidStep } from '../contracts/requests/bid';

@Injectable({
  providedIn: 'root',
})
export class BidStepService {
  private bidSteps: BidStep[] = [
    new BidStep(0.01, 0.99, 0.05),
    new BidStep(1.0, 4.99, 0.25),
    new BidStep(5.0, 24.99, 0.5),
    new BidStep(25.0, 99.99, 1.0),
    new BidStep(100.0, 249.99, 2.5),
    new BidStep(250.0, 499.99, 5.0),
    new BidStep(500.0, 999.99, 10.0),
    new BidStep(1000.0, 2499.99, 25.0),
    new BidStep(2500.0, 4999.99, 50.0),
    new BidStep(5000.0, Number.MAX_VALUE, 100.0),
  ];

  constructor() {}

  getBidStep(currentPrice: number): number {
    const range = this.bidSteps.find(
      (b) => currentPrice >= b.minPrice && currentPrice <= b.maxPrice
    );
    if (!range) {
      throw new Error(
        `No bid increment found for the current price of ${currentPrice}`
      );
    }
    return range.step;
  }
}

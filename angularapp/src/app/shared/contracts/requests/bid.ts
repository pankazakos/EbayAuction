export interface AddBidRequest {
  itemId: number;
  amount: number;
}

export class BidStep {
  constructor(
    public minPrice: number,
    public maxPrice: number,
    public step: number
  ) {}
}

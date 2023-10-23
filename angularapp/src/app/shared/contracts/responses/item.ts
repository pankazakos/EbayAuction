export interface AddItemResponse {
  itemId: number;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  active: boolean;
  description: string;
  sellerId: number;
}

export interface $1ublishedItemResponse {
  itemId: string;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  started: Date;
  ends: Date;
  active: boolean;
  description: string;
  sellerId: number;
}

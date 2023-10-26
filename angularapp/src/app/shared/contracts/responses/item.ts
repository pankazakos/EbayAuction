export interface GenericItemResponse {
  itemId: string;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  started: string;
  ends: string;
  active: boolean;
  description: string;
  sellerId: number;
}

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

export interface PublishedItemResponse {
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

export interface LimitedInfoPublishedItemResponse {
  itemId: string;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  sellerId: number;
}

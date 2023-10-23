export interface AddItemResponse {
  ItemId: number;
  Name: string;
  Currently: number;
  BuyPrice: number;
  FirstBid: number;
  NumBids: number;
  Active: boolean;
  Description: string;
  SellerId: number;
}

export interface PublishedItemResponse {
  ItemId: string;
  Name: string;
  Currently: number;
  BuyPrice: number;
  FirstBid: number;
  NumBids: number;
  Started: Date;
  Ends: Date;
  Active: boolean;
  Description: string;
  SellerId: number;
}

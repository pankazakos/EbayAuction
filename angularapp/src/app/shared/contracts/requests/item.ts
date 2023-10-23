export interface AddItemRequest {
  Name: string;
  CategoryIds: number[];
  FirstBid: number;
  Description: string;
}

export interface PublishItemRequest {
  Expiration: Date;
}

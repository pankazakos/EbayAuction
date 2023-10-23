export interface AddItemRequest {
  name: string;
  categoryIds: number[];
  firstBid: number;
  description: string;
}

export interface PublishItemRequest {
  expiration: Date;
}

export interface AddItemRequest {
  name: string;
  categoryIds: number[];
  firstBid: number;
  buyPrice?: number;
  description: string;
}

export interface PublishItemRequest {
  expiration: string;
}

import { IEntityResponse } from './IEntityResponse';

interface IBidResponse extends IEntityResponse {}

export interface AddBidResponse extends IBidResponse {
  bidId: number;
  time: Date;
  amount: number;
  itemId: number;
  bidderId: number;
}

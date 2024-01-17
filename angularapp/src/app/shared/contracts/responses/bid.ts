import { IEntityResponse } from './IEntityResponse';

interface IBidResponse extends IEntityResponse {}

export interface BasicBidResponse extends IBidResponse {
  bidId: number;
  time: string;
  amount: number;
  itemId: number;
  bidderId: number;
}

export interface ExtendedBidInfo extends BasicBidResponse {
  seller: string;
  itemTitle: string;
  auctionsStatus: 'active' | 'expired';
}

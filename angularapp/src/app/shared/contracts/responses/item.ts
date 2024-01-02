import { IEntityResponse } from './IEntityResponse';

interface IItemResponse extends IEntityResponse {}

export interface BasicItemResponse extends IItemResponse {
  itemId: number;
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
  imageGuid: string;
}

export interface ExtendedItemInfo extends BasicItemResponse {
  image: {
    src: string;
    isLoading: boolean;
  };
  auctionStarted: string;
  auctionEnds: string;
}

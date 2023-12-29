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

export interface AddItemResponse extends IItemResponse {
  itemId: number;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  active: boolean;
  description: string;
  sellerId: number;
  imageUrl: string;
}

export interface LimitedInfoPublishedItemResponse extends IItemResponse {
  itemId: string;
  name: string;
  currently: number;
  buyPrice: number;
  firstBid: number;
  numBids: number;
  sellerId: number;
}

import { baseUrl } from '../../types';

const bidBaseUrl = `${baseUrl}/bid`;

export const BidEndpoints: Readonly<{
  create: string;
  itemBids: (id: number) => string;
  myBids: string;
  extendedMyBids: string;
  lastBid: (itemId: number) => string;
}> = {
  create: `${bidBaseUrl}`,
  itemBids: (id: number) => `${bidBaseUrl}/${id}`,
  myBids: `${bidBaseUrl}/my-bids`,
  extendedMyBids: `${bidBaseUrl}/full/my-bids`,
  lastBid: (itemId: number) => `${bidBaseUrl}/last?itemId=${itemId}`,
};

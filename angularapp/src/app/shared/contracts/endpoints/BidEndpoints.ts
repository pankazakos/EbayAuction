import { baseUrl } from '../../types';

const bidBaseUrl = `${baseUrl}/bid`;

export const BidEndpoints: Readonly<{
  create: string;
  getItemBids: string;
}> = {
  create: `${bidBaseUrl}`,
  getItemBids: `${bidBaseUrl}/GetItemBids`,
};

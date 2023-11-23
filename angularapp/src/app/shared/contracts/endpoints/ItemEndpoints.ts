import { baseUrl } from '../../types';

const itemBaseUrl = `${baseUrl}/item`;

export const ItemEndpoints: Readonly<{
  create: string;
  search: string;
  myItems: string;
  inactive: string;
  active: string;
  bidden: string;
  getById: (id: number) => string;
  getImage: (guid: string) => string;
  activate: (id: number) => string;
  delete: (id: number) => string;
}> = {
  create: `${itemBaseUrl}`,
  search: `${itemBaseUrl}`,
  myItems: `${itemBaseUrl}/myitems`,
  inactive: `${itemBaseUrl}/inactive`,
  active: `${itemBaseUrl}/active`,
  bidden: `${itemBaseUrl}/bidden`,
  getById: (id: number) => `${itemBaseUrl}/${id}`,
  getImage: (guid: string) => `${itemBaseUrl}/${guid}`,
  activate: (id: number) => `${itemBaseUrl}/activate/${id}`,
  delete: (id: number) => `${itemBaseUrl}/${id}`,
};

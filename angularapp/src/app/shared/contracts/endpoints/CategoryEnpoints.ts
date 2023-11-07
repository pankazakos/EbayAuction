import { baseUrl } from '../../types';

const categoryBaseUrl = `${baseUrl}/category`;

export const CategoryEndpoints: Readonly<{
  getById: (id: number) => string;
  getAll: string;
  create: string;
  delete: (id: number) => string;
}> = {
  getById: (id: number) => `${categoryBaseUrl}/${id}`,
  getAll: `${categoryBaseUrl}/all`,
  create: `${categoryBaseUrl}`,
  delete: (id: number) => `${categoryBaseUrl}/${id}`,
};

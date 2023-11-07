import { IEntityResponse } from './IEntityResponse';

export interface PaginatedResponse<T extends IEntityResponse> {
  castEntities: T[];
  total: number;
  page: number;
  limit: number;
}

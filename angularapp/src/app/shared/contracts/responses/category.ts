import { IEntityResponse } from './IEntityResponse';

interface ICategoryResponse extends IEntityResponse {}

export interface BasicCategoryResponse extends ICategoryResponse {
  id: number;
  name: string;
}

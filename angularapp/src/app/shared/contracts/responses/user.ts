import { IEntityResponse } from './IEntityResponse';

interface IUserResponse extends IEntityResponse {}

export interface BasicUserResponse extends IUserResponse {
  id: number;
  userName: string;
  email: string;
  firtsName: string;
  lastName: string;
  lastLogin: string;
  dateJoined: string;
  country: string;
  location: string;
  isSuperuser: boolean;
  isActive: boolean;
}

export interface RegisterUserResponse extends IUserResponse {
  id: number;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  dateJoined: string;
  country: string;
  location: string;
  isSuperuser: boolean;
}

export interface LoginUserResponse extends IUserResponse {
  accessToken: string;
}

export interface IdToUsernameResponse extends IUserResponse {
  username: string;
}

export interface RegisterUserResponse {
  id: number;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  dateJoined: Date;
  country: string;
  location: string;
  isSuperuser: boolean;
}

export interface NoPasswordUserResponse {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  lastLogin: string;
  dateJoined: string;
  email: string;
  country: string;
  location: string;
  isSuperuser: boolean;
  isActive: boolean;
}

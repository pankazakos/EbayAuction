export interface RegiserUserResponse {
  Id: number;
  UserName: string;
  Email: string;
  FirstName: string;
  LastName: string;
  DateJoined: Date;
  Country: string;
  Location: string;
  IsSuperuser: boolean;
}

export interface NoPasswordUserResponse {
  Id: number;
  Username: string;
  FirstName: string;
  LastName: string;
  LastLogin: Date;
  DateJoined: Date;
  Email: string;
  Country: string;
  Location: string;
  IsSuperuser: boolean;
  IsActive: boolean;
}

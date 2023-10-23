export interface RegisterUserRequest {
  Username: string;
  Password: string;
  FirstName: string;
  LastName: string;
  Email: string;
  Country: string;
  Location: string;
}

export interface UserCredentialsRequest {
  Username: string;
  Password: string;
}

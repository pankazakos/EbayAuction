export interface RegisterUserRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  email: string;
  country: string;
  location: string;
}

export interface UserCredentialsRequest {
  username: string;
  password: string;
}

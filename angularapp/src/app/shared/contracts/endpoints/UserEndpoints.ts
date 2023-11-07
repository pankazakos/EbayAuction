import { baseUrl } from '../../types';

const userBaseUrl = `${baseUrl}/user`;

export const UserEndpoints: Readonly<{
  all: string;
  usernames: string;
  getById: (id: number) => string;
  getByUsername: (username: string) => string;
  delete: (id: number) => string;
  create: string;
  login: string;
}> = {
  all: `${userBaseUrl}/all`,
  usernames: `${userBaseUrl}/usernames`,
  getById: (id: number) => `${userBaseUrl}/${id}`,
  getByUsername: (username: string) => `${userBaseUrl}/${username}`,
  delete: (id: number) => `${userBaseUrl}/${id}`,
  create: `${userBaseUrl}`,
  login: `${userBaseUrl}/login`,
};

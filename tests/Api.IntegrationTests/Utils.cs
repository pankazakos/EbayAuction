using System.Net;
using contracts.Requests.User;
using Newtonsoft.Json;
using System.Text;
using contracts.Requests;
using contracts.Responses.User;

namespace Api.IntegrationTests
{
    public static class Utils
    {
        public const string BaseUrl = "https://localhost:7068/api";

        public static string AdminToken = string.Empty;

        public enum ContentType
        {
            Json,       // Represents "application/json"
            TextPlain   // Represents "text/plain"
        }

        public static StringContent ConvertRequestData(IAppRequest requestData, ContentType type)
        {
            var json = JsonConvert.SerializeObject(requestData);
            var data = new StringContent(json, Encoding.UTF8, type == ContentType.Json ? "application/json" : "text/plain");

            return data;
        }

        public static async Task<T?> ConvertResponseData<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<T>(responseString);

            return responseData;
        }

        public static async Task<string> LoginAdmin(HttpClient client)
        {
            if (AdminToken.Length != 0)
            {
                return AdminToken;
            }

            var adminCredentials = new LoginUserRequest
            {
                Username = "admin",
                Password = "admin"
            };

            var json = JsonConvert.SerializeObject(adminCredentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}/user/login", data);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString) ?? 
                               throw new InvalidOperationException("Couldn't get jwt of admin");
            
            AdminToken = responseBody.AccessToken;

            return responseBody.AccessToken;
        }

        public static async Task<LoginUserResponse> LoginUser(HttpClient client, LoginUserRequest credentials)
        {
            var loginBody = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync($"{BaseUrl}/user/login", loginBody);

            var responseString = await loginResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            return responseBody ?? new LoginUserResponse();
        }

        public static async Task<string> LoginOrCreateSimpleUser(HttpClient client)
        {
            var userCredentials = new LoginUserRequest
            {
                Username = "simpleUser",
                Password = "123"
            };

            var loginBody = new StringContent(JsonConvert.SerializeObject(userCredentials), Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync($"{BaseUrl}/user/login", loginBody);

            // Create user if not found
            if (loginResponse.StatusCode == HttpStatusCode.NotFound)
            {
                var userInfo = new RegisterUserRequest
                {
                    Username = "simpleUser",
                    Password = "123",
                    FirstName = "firstName",
                    LastName = "lastName",
                    Email = "simpleUser@test.com",
                    Country = "test country",
                    Location = "test location",
                };

                var registerBody = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, "application/json");

                var registerResponse = await client.PostAsync($"{BaseUrl}/user", registerBody);

                registerResponse.EnsureSuccessStatusCode();
            }

            loginResponse = await client.PostAsync($"{BaseUrl}/user/login", loginBody);

            var responseString = await loginResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            if (responseBody != null)
            {
                return responseBody.AccessToken;
            }

            throw new InvalidOperationException("Error while getting the jwt of simple user.");
        }
    }
}
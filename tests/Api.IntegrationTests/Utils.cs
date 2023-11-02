using contracts.Requests.User;
using Newtonsoft.Json;
using System.Text;
using contracts.Responses.User;

namespace Api.IntegrationTests
{
    public static class Utils
    {
        public const string BaseUrl = "https://localhost:7068/api/";

        public static string AdminToken = string.Empty;

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

            var response = await client.PostAsync($"{BaseUrl}user/login", data);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<LoginUserResponse>(responseString);

            if (responseBody is null)
            {
                throw new InvalidOperationException("Couldn't get jwt of admin");
            }

            AdminToken = responseBody.AccessToken;

            return responseBody.AccessToken;
        }
    }
}
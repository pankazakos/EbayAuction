using Contracts.Requests.User;
using Newtonsoft.Json;
using System.Text;
using Contracts.Responses.User;

namespace Api.Tests.Integration
{
    public static class Utils
    {
        public const string BaseUrl = "https://localhost:7068/api/";

        public static async Task<string> LoginAdmin(HttpClient client)
        {
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

            return responseBody.AccessToken;
        }
    }
}

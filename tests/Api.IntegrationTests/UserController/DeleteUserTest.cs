using System.Net;
using System.Net.Http.Headers;

namespace Api.IntegrationTests.UserController
{
    public class DeleteUserTest
    {
        private readonly HttpClient _client;

        public DeleteUserTest()
        {
            _client = new HttpClient();
            var adminJwt = Utils.LoginAdmin(_client).GetAwaiter().GetResult();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            const int invalidUserId = 9999;

            var response = await _client.DeleteAsync($"{Utils.BaseUrl}user/{invalidUserId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

using Microsoft.AspNetCore.Mvc.Testing;
using SnippetAdmin.Constants;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Account;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SnippetAdmin.IT.Controllers
{
    public class AccountControllerTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        private readonly HttpClient _httpClient;

        public AccountControllerTests(
            CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task LoginTest001()
        {
            // Act
            var loginModel = new LoginInputModel("admin", "admin2");
            var content = new StringContent(JsonSerializer.Serialize(loginModel),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                // etc.
            };
            var resultModle = JsonSerializer.Deserialize<CommonResult<LoginOutputModel>>(data, option);
            Assert.True(resultModle.IsSuccess);
            Assert.Equal(resultModle.Code, MessageConstant.ACCOUNT_INFO_0001.Item1);
            Assert.Equal(resultModle.Message, MessageConstant.ACCOUNT_INFO_0001.Item2);
        }

        [Fact]
        public async Task LoginTest002()
        {
            // Act
            var loginModel = new LoginInputModel("admin", "admin");
            var content = new StringContent(JsonSerializer.Serialize(loginModel),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                // etc.
            };
            var resultModle = JsonSerializer.Deserialize<CommonResult>(data, option);
            Assert.False(resultModle.IsSuccess);
            Assert.Equal(resultModle.Code, MessageConstant.ACCOUNT_ERROR_0001.Item1);
            Assert.Equal(resultModle.Message, MessageConstant.ACCOUNT_ERROR_0001.Item2);
        }
    }
}
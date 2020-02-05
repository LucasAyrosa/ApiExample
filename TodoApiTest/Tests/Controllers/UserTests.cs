using System;
using System.Net;
using System.Net.Http;
using System.Text;
using API.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ToDoAPI;
using TodoApiTest.Config;
using Xunit;

namespace TodoApiTest.Tests.Controllers
{
    public class UserTests : IClassFixture<TodoApplicationFactory<Startup>>
    {
        private readonly TodoApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public UserTests(TodoApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/User/")
            });
        }

        [Theory]
        [InlineData("lucas@exemplo.com", "S3gr3d0!")]
        [InlineData("user@email.com", "senha")]
        [InlineData("lucas@exemplo.com", null)]
        [InlineData(null, "Senha@1")]
        [InlineData(null, null)]
        public async void LoginUser_Fail(string email, string password)
        {
            //Given
            var user = new LoginUserDto
            {
                Email = email,
                Password = password
            };
            //When
            var response = await _client.PostAsync("login", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("lucas@example.com", "Senh@1")]
        [InlineData("ayrosa@email.com", "S3gr3d0!")]
        public async void LoginUser_Success(string email, string password)
        {
            //Given
            var user = new LoginUserDto
            {
                Email = email,
                Password = password
            };
            //When
            var response = await _client.PostAsync("login", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(String.IsNullOrEmpty(content));
        }

        [Fact]
        public async void NewUser_returnsCreated()
        {
            //Given
            var user = new RegisterUserDto
            {
                Email = "lucasayrosa@exemplo.com",
                Password = "aLg@12",
                ConfirmPassword = "aLg@12"
            };
            //When
            var response = await _client.PostAsync("new", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [InlineData("email@email.com", "senha")]
        [InlineData("teste@email.com", "senhaDificil")]
        [InlineData("teste", "Senh@1!")]
        [InlineData("teste@email.com", null)]
        [InlineData(null, "Senha@!2")]
        [InlineData(null, null)]
        public async void NewUser_returnsBadRequest(string email, string password)
        {
            //Given
            var user = new RegisterUserDto
            {
                Email = email,
                Password = password,
                ConfirmPassword = password
            };
            //When
            var response = await _client.PostAsync("new", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
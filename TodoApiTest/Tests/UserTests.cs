using System;
using System.Net;
using System.Net.Http;
using System.Text;
using API.Dto;
using Newtonsoft.Json;
using TodoApiTest.Config;
using Xunit;

namespace TodoApiTest.Tests
{
    public class UserTests : IClassFixture<TodoApplicationFactory<ToDoAPI.Startup>>
    {
        private readonly string BaseUri = "api/User";
        private readonly TodoApplicationFactory<ToDoAPI.Startup> _factory;
        private readonly HttpClient _client;
        public UserTests(TodoApplicationFactory<ToDoAPI.Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async void LoginUser_Fail()
        {
            //Given
            var user = new LoginUserDto();
            //When
            var response = await _client.PostAsync($"{BaseUri}/login", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void LoginUser_Success()
        {
            //Given
            var user = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Senh@1"
            };
            //When
            var response = await _client.PostAsync($"{BaseUri}/login", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            var content = await response.Content.ReadAsStringAsync();
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
            var response = await _client.PostAsync($"{BaseUri}/new", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void NewUser_returnsBadRequest()
        {
            //Given
            var user = new RegisterUserDto
            {
                Email = null,
                Password = "senha",
                ConfirmPassword = "senha"
            };
            //When
            var response = await _client.PostAsync($"{BaseUri}/new", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
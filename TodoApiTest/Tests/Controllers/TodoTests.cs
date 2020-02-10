using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using API.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ToDoAPI;
using TodoApiTest.Config;
using Xunit;

namespace TodoApiTest.Tests.Controllers
{
    public class TodoTests : IClassFixture<TodoApplicationFactory<Startup>>
    {
        private readonly TodoApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public TodoTests(TodoApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
        }

        #region GET

        [Fact]
        public async void Get_Unauthorized()
        {
            //Given

            //When
            var response = await _client.GetAsync("");
            //Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void GetId_Unauthorized()
        {
            //Given

            //When
            var response = await _client.GetAsync("1");
            var content = await response.Content.ReadAsStringAsync();
            //Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("comida", null, null, null, null)]
        [InlineData(null, true, null, null, null)]
        [InlineData(null, null, 2, 2, null)]
        [InlineData(null, null, null, null, "-name.iscomplete")]
        [InlineData("comida", false, 0, 0, "+name.iscomplete")]
        public async void Get_Ok_WithFilter(string name, bool? isComplete, int? limit, int? page, string sortBy)
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
               {
                   builder.ConfigureTestServices(services =>
                   {
                       services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                   });
               }).CreateClient(new WebApplicationFactoryClientOptions
               {
                   BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
               });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var queryString = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name)) queryString.Add("name", name);
            if (isComplete != null) queryString.Add("isComplete", isComplete.ToString());
            if (!string.IsNullOrEmpty(sortBy)) queryString.Add("sortBy", sortBy);
            if (page != null) queryString.Add("page", page.ToString());
            if (limit != null) queryString.Add("limit", limit.ToString());
            //When
            var response = await client.GetAsync(QueryHelpers.AddQueryString("", queryString));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TodoItemDto>>(content);
            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            if (name != null) Assert.All(result, ti => ti.Name.Contains(name));
            if (isComplete != null) Assert.All(result, ti => ti.IsComplete.CompareTo(isComplete));
            if (limit > 0) Assert.True(result.Count <= limit);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetId_Ok(long id)
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            //When
            var response = await client.GetAsync(id.ToString());
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TodoItemDto>(content);
            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async void GetId_NotFound()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            //When
            var response = await client.GetAsync("100");
            //Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region POST
        [Fact]
        public async void Post_Unauthorized()
        {
            //Given
            var todoItem = new TodoItemDto
            {
                Name = "Jogar bola",
                IsComplete = false
            };
            //When
            var response = await _client.PostAsync("", new StringContent(JsonConvert.SerializeObject(todoItem)));
            //Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("Jogar bola", true)]
        [InlineData("Preparar marmita", null)]
        public async void Post_Created(string name, bool isComplete)
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var todoItem = new TodoItemDto
            {
                Name = name,
                IsComplete = isComplete
            };
            //When
            var response = await client.PostAsync("", new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TodoItemDto>(content);
            //Then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(result.Id > 0);
            Assert.Equal(todoItem.Name, result.Name);
            Assert.Equal(todoItem.IsComplete, result.IsComplete);
        }

        [Fact]
        public async void Post_BadRequest()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                    });
                }).CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
                });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var todoItem = new TodoItemDto();
            //When
            var response = await client.PostAsync("", new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region PUT
        [Fact]
        public async void Put_Unauthorized()
        {
            //Given
            var todoItem = new TodoItemDto
            {
                Id = 2,
                Name = "Dar comida para os peixes",
                IsComplete = true,
            };
            //When
            var response = await _client.PutAsync(todoItem.Id.ToString(), new StringContent(JsonConvert.SerializeObject(todoItem)));
            //Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void Put_NoContent()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var todoItem = new TodoItemDto
            {
                Id = 2,
                Name = "Dar comida para os peixes",
                IsComplete = true,
            };
            //When
            var response = await client.PutAsync(todoItem.Id.ToString(), new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async void Put_BadRequest()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var todoItem = new TodoItemDto
            {
                Id = 2,
                Name = "Dar comida para os peixes",
                IsComplete = true,
            };
            //When
            var response = await client.PutAsync("3", new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async void Put_NotFound()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var todoItem = new TodoItemDto
            {
                Id = 100,
                Name = "Dar comida para os peixes",
                IsComplete = true
            };
            //When
            var response = await client.PutAsync(todoItem.Id.ToString(), new StringContent(JsonConvert.SerializeObject(todoItem), Encoding.UTF8, "application/json"));
            //Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region DELETE
        [Fact]
        public async void Delete_Unauthorized()
        {
            //Given

            //When
            var response = await _client.DeleteAsync("1");
            //Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async void Delete_Ok()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                    });
                }).CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
                });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            //When
            var response = await client.DeleteAsync("1");
            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Delete_NotFound()
        {
            //Given
            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                    });
                }).CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("http://localhost/api/v1/TodoItems/")
                });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            //When
            var response = await client.DeleteAsync("100");
            //Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }
}
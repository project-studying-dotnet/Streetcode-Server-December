using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Streetcode.WebApi.Middleware;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Org.BouncyCastle.Tls;
using Streetcode.WebApi.Extensions;
using FluentAssertions.Common;
using Streetcode.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Streetcode.XUnitTest.CustomMiddlewareTests
{
    public class GlobalExceptionHandlerMiddlewareTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public GlobalExceptionHandlerMiddlewareTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                })
                .Configure(app =>
                {
                    app.UseGlobalExceptionHandler();
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/test", async context =>
                        {
                            throw new Exception("Unexpected error");
                        });
                        endpoints.MapGet("/api/test/badrequest", async context =>
                        {
                            throw new BadRequestException();
                        });
                        endpoints.MapGet("/api/test/forbidden", async context =>
                        {
                            throw new ForbiddenAccessException();
                        });
                    });
                }));
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Middleware_ShouldReturn500_WhenExceptionIsThrown()
        {
            var response = await _client.GetAsync("/api/test");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal(500, problemDetails?.Status);
            Assert.Equal("An error occurred while processing your request.", problemDetails?.Title);
            Assert.Equal("Internal server error has occurred", problemDetails?.Detail);
        }

        [Fact]
        public async Task Middleware_ShouldReturn400_WhenBadRequestExceptionIsThrown()
        {
            var response = await _client.GetAsync("/api/test/badrequest");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal(400, problemDetails?.Status);
            Assert.Contains("The request is invalid.", problemDetails?.Detail);
        }

        [Fact]
        public async Task Middleware_ShouldReturn403_WhenForbiddenAccessExceptionIsThrown()
        {
            var response = await _client.GetAsync("/api/test/forbidden");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(problemDetails);
            Assert.Equal(403, problemDetails?.Status);
            Assert.Contains("Forbidden access", problemDetails?.Detail);
        }
    }
}

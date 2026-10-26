using EmployeeManagementAPI.Middleware;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xunit;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Tests
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenExceptionOccurs_Returns500ErrorResponse()
        {
            // Arrange
            RequestDelegate next = (context) =>
                throw new Exception("Test exception");

            var logger = new Logger<ExceptionMiddleware>(
                LoggerFactory.Create(builder => { }));

            var middleware = new ExceptionMiddleware(next, logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            context.TraceIdentifier = "test-trace-id";

            // Act
            await middleware.InvokeAsync(context);

            // Read the response body
            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // Assert
            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                context.Response.StatusCode);

            Assert.Equal("application/json", context.Response.ContentType);

            Assert.NotNull(errorResponse);
            Assert.Equal(500, errorResponse.StatusCode);
            Assert.Equal(
                "An unexpected error occurred.",
                errorResponse.Message);
            Assert.Equal("test-trace-id", errorResponse.TraceId);
        }


        [Fact]
        public async Task InvokeAsync_WhenExceptionOccurs_LogsError()
        {
            // Arrange: Create a mock logger
            var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
            mockLogger
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true);

            RequestDelegate next = (context) =>
                throw new Exception("Test exception");

            var middleware = new ExceptionMiddleware(
                next,
                mockLogger.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act: Invoke middleware and trigger an exception
            await middleware.InvokeAsync(context);

            // Assert: Verify that an error was logged
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) =>
                        state.ToString()!.Contains(
                            "An unhandled exception occurred.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task InvokeAsync_WhenExceptionOccurs_IncludesTraceIdInResponse()
        {
            // Arrange
            RequestDelegate next = (context) =>
                throw new Exception("Test exception");

            var logger = new Logger<ExceptionMiddleware>(
                LoggerFactory.Create(builder => { }));

            var middleware = new ExceptionMiddleware(next, logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.TraceIdentifier = "trace-test-123";

            // Act
            await middleware.InvokeAsync(context);

            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // Assert
            Assert.NotNull(errorResponse);
            Assert.Equal("trace-test-123", errorResponse.TraceId);
        }


        





    }
}
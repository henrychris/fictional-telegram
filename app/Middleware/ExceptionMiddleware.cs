using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using app.Errors;
using Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace app.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private readonly IWebHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ExceptionMiddleware: {ex.Message}\n{ex.StackTrace}\n\n\nINNER EXCEPTION\n\n" +
                    $"{ex.InnerException}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                ? new ErrorDetails(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ErrorDetails(context.Response.StatusCode, $"Internal Server Error.\n{ex.Message}");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
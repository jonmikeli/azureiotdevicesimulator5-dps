using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Net;
using System.Threading.Tasks;

namespace IoT.Simulator.API.DeviceManagement.API.Common.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is ArgumentNullException) code = HttpStatusCode.BadRequest;
            else if (exception is ArgumentException) code = HttpStatusCode.BadRequest;
            else if (exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;

            _logger.LogError($"GLOBAL ERROR HANDLER::HTTP:{code}::{exception.Message}");

            //Known issue for now in System.Text.Json
            //var result = JsonSerializer.Serialize<Exception>(exception, new JsonSerializerOptions { WriteIndented = true });

            //Newtonsoft.Json serializer (should be replaced once the known issue in System.Text.Json will be solved)
            var result = JsonConvert.SerializeObject(exception, Formatting.Indented);

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}

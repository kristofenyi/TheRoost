using Microsoft.Extensions.Localization;
using System.Net;
using TheRoost.API.Models.ErrorHandling;

namespace TheRoost.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _next = next;
            _logger = logger;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var thisError = new ErrorDetailForClient()
            {
                ErrorCode = context.Response.StatusCode,
                Message = _sharedResourceLocalizer["Internal Server Error, please get in touch with Admin and provide ErrorID"],
                ErrorID = Guid.NewGuid()
            };
            //Log Error
            _logger.Log(LogLevel.Error, "\n\nGUID:" + thisError.ErrorID + "\nException:" + exception + "\nRequest Method:" + context.Request.Method + "\n\n");
            //Send Error to Client
            await context.Response.WriteAsync(thisError.ToString());
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}

using System.Net;
using Newtonsoft.Json;

namespace Football.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex, context.RequestServices.GetService<IHostEnvironment>());
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment? env)
        {
            HttpStatusCode status;
            object? errors = null;

            switch (exception)
            {
                case FluentValidation.ValidationException validationEx:
                    status = HttpStatusCode.BadRequest;
                    errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    break;
                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    break;
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    break;
            }

            var response = new
            {
                errorCode = status.ToString(),
                message = exception.Message,
                details = errors,
                stackTrace = (env != null && (env.IsDevelopment() || env.IsEnvironment("Testing"))) ? exception.StackTrace : null
            };

            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(payload);
        }
    }
}
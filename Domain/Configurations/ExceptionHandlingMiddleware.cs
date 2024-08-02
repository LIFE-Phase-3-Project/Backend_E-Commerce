using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configurations
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate _requestDelegate;
        public readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        public Task HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex.Message);

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var errorMessage = ex.Message;

            if (ex is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                errorMessage = "Unauthorized access.";
            }
            else if (ex is ArgumentException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            } else if (ex is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
            } else if (ex is ValidationException) {              
                statusCode = (int)HttpStatusCode.BadRequest;
            }

            var errorResponse = new
            {
                Message = ex.Message,
                Code = "Error",
                StatusCode = statusCode
            };

            var costumResponse = JsonConvert.SerializeObject(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(costumResponse);
        }
    }
}

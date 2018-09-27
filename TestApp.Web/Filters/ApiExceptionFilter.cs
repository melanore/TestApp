using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestApp.Core.Exceptions;

namespace TestApp.Web.Filters
{
    public class ExceptionFilterFactory : IFilterFactory
    {
        public ExceptionFilterFactory(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        public IHostingEnvironment Environment { get; }
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<ApiExceptionFilter>>();
            return new ApiExceptionFilter(Environment.IsDevelopment(), logger);
        }

        private class ApiExceptionFilter : ExceptionFilterAttribute
        {
            public ApiExceptionFilter(bool isVerbose, ILogger<ApiExceptionFilter> logger)
            {
                IsVerbose = isVerbose;
                Logger = logger;
            }

            private bool IsVerbose { get; }
            private ILogger<ApiExceptionFilter> Logger { get; }

            public override void OnException(ExceptionContext context)
            {
                var (status, apiError) = HandleErrorResponse(context);
                context.Result = new JsonResult(apiError);
                context.HttpContext.Response.StatusCode = (int) status;
                base.OnException(context);
            }

            private (HttpStatusCode, ApiError) HandleErrorResponse(ExceptionContext context)
            {
                var serverError = $"A server error <{Guid.NewGuid()}> occurred.";
                const string unauthorizedAccess = "Unauthorized Access";

                var apiError = new ApiError(context.Exception?.GetBaseException()?.Message ?? context.Exception?.Message, IsVerbose ? context.Exception?.StackTrace : default);
                
                Logger.LogError($"{serverError}\nEx: {apiError.Message}.\nTrace: {apiError.Details}.");
                switch (context.Exception)
                {
                    case UnauthorizedAccessException _:
                        Logger.LogWarning($"{unauthorizedAccess}. Ip address: {{{context.HttpContext?.Connection?.RemoteIpAddress}}}");
                        return (HttpStatusCode.Unauthorized, new ApiError(unauthorizedAccess, default));
                    case ApiException ex:
                        ex.Details?.ForEach(apiError.Details.Add);
                        return (ex.StatusCode, apiError);
                    case NotImplementedException _:
                        return (HttpStatusCode.NotImplemented, apiError);
                    default:
                        return (HttpStatusCode.InternalServerError, apiError);
                }
            }
        }
    }
}
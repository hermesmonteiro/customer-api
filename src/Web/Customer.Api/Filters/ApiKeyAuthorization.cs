using System;
using System.Net;
using Customer.Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Customer.Api.Filters
{
    public class ApiKeyAuthorization : Attribute, IAuthorizationFilter
    {
        private const string BEARER_API_KEY_FORMAT = "Bearer {0}";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //if (!ProcessAuthorization(context))
            //    context.Result = new UnauthorizedResult();

            //By the moment it´s disabled the check of the header token (AuthTokenOptions).
            ProcessAuthorization(context);
        }

        private static bool ProcessAuthorization(AuthorizationFilterContext context)
        {
            var apiKeyFromHeader = context.HttpContext.Request.Headers[HttpRequestHeader.Authorization.ToString()].ToString();

            if (string.IsNullOrWhiteSpace(apiKeyFromHeader))
                return false;

            var optionToken = (IOptions<AuthTokenOptions>)context.HttpContext.RequestServices.GetService(typeof(IOptions<AuthTokenOptions>));

            //saving authorization token in routeData to be used for "launch-slots" endpoint. Casino External Api "get-game-url"
            context.RouteData.Values.Add("authorizationToken", apiKeyFromHeader);

            return apiKeyFromHeader == string.Format(BEARER_API_KEY_FORMAT, optionToken.Value.Token);
        }
    }
}
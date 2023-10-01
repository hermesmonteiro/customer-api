using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api
{
    public class ApiControllerBase : ControllerBase
    {
        protected IActionResult GetBadRequestResponse(string message)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400",
                Title = message,
                Detail = string.Empty
            });
        }

        protected IActionResult GetNotFoundResponse(string message)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "https://httpstatuses.com/404",
                Title = message,
                Detail = string.Empty
            });
        }
    }
}

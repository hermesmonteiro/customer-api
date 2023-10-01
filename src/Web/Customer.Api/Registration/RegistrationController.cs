using System;
using System.Threading.Tasks;
using Customer.Api;
using Customer.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Registration
{
    [Route("~/registration")]
    [ApiController]
    [ApiKeyAuthorization]
    public class RegistrationController : ApiControllerBase
    {
        public RegistrationController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RegisterCustomerRequest registerCustomerRequest)
        {

            return new OkResult();
        }
    }
}
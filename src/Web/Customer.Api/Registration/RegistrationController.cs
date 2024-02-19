using System;
using System.Threading.Tasks;
using Customer.Api;
using Customer.Api.Filters;
using Customer.Application.Registration;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Registration
{
    [Route("~/registration")]
    [ApiController]
    [ApiKeyAuthorization]
    public class RegistrationController : ApiControllerBase
    {
        ICustomerRegistrationService _customerRegistrationService;

        public RegistrationController(ICustomerRegistrationService customerRegistrationService)
        {
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] RegisterCustomerRequest registerCustomerRequest)
        {
            await _customerRegistrationService.Register(new RegisterCustomerEntity()
            {
                Country = registerCustomerRequest.Country,
                Email = registerCustomerRequest.Email,
                FullName = registerCustomerRequest.FullName,
                PhoneNumber = registerCustomerRequest.PhoneNumber
            });

            return new OkResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _customerRegistrationService.Delete(id);

            return new OkResult();
        }
    }
}
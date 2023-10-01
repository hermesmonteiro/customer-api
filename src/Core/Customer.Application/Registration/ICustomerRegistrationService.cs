using Customer.Api.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Registration
{
    internal interface ICustomerRegistrationService
    {
        Task Register(RegisterCustomerEntity registerCustomerEntity);
    }
}

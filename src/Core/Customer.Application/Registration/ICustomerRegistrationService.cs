using Customer.Api.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Registration
{
    public interface ICustomerRegistrationService
    {
        Task Register(RegisterCustomerEntity registerCustomerEntity);
    }
}

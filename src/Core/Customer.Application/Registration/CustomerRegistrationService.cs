using Customer.Api.Registration;
using Customer.Infrastructure.Repositories.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Registration
{
    internal class CustomerRegistrationService : ICustomerRegistrationService
    {
        ICustomerRepository _customerRepository;

        public CustomerRegistrationService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task Register(RegisterCustomerEntity registerCustomerEntity)
        {
            await _customerRepository.InsertCustomer(registerCustomerEntity);
        }
    }
}

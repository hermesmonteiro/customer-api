using Customer.Api.Registration;
using Customer.Infrastructure.Repositories.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Registration
{
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        ICustomerRepository _customerRepository;

        public CustomerRegistrationService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task Register(RegisterCustomerEntity registerCustomerEntity)
        {
            var amountToValidate = Constants.COEF / registerCustomerEntity.InitialAmount;
            if (amountToValidate <= 0 )
            {
                throw new Exception("User with invalid initial amount");
            }

            await _customerRepository.InsertCustomer(registerCustomerEntity);
        }
    }
}

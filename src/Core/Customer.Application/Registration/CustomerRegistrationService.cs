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
        }

        public async Task Delete(int userRegistrationId)
        {
            var customer = await _customerRepository.GetCustomer(userRegistrationId);
            await _customerRepository.DeleteCustomer(customer); 
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

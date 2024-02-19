using Customer.Api.Registration;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task<RegisterCustomerEntity> GetCustomer(int customerId);
        Task InsertCustomer(RegisterCustomerEntity registerCustomerEntity);
        Task DeleteCustomer(RegisterCustomerEntity customerEntity);
    }
}

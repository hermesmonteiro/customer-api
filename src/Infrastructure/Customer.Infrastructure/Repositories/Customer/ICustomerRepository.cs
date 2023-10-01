using Customer.Api.Registration;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task InsertCustomer(RegisterCustomerEntity registerCustomerEntity);
    }
}

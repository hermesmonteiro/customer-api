using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories.CustomerContract
{
    public interface ICustomerContractRepository
    {
        Task InsertCustomer();
    }
}

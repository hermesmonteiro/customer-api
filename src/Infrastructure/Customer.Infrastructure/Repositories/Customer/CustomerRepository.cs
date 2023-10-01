using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Customer.Api.Registration;
using Customer.Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Customer.Infrastructure.Repositories.Customer
{
    public class CustomerRepository : BaseSqlRepository, ICustomerRepository
    {
        public CustomerRepository(IOptions<DbConnectionStrings> options)
            => ConnectionString = options.Value.DbConnectionString;

        //public async Task<LaunchUrlConfig> GetUrlBaseConfigAsync()
        //{
        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@providerId", SqlDbType.Int) { Value =1 }
        //    };

        //    return await ExecuteQueryAsync(
        //        "LaunchConfigSqlRepository.GetUrlBaseConfigAsync",
        //        @"
        //            INSERT INTO [dbo].[Customer]
        //                       ([FullName]
        //                       ,[Country]
        //                       ,[Email]
        //                       ,[PhoneNumber])
        //                 VALUES
        //                       (<FullName, varchar(500),>
        //                       ,<Country, varchar(50),>
        //                       ,<Email, varchar(255),>
        //                       ,<PhoneNumber, varchar(50),>)
        //        ",
        //        parameters,
        //        MapUrlLaunchConfig());
        //}

        public async Task InsertCustomer(RegisterCustomerEntity registerCustomerEntity)
        {
            await ExecuteNonQueryAsync(
                "InsertCustomer",
                @"
                    INSERT INTO [dbo].[Customer]
                               ([FullName]
                               ,[Country]
                               ,[Email]
                               ,[PhoneNumber])
                         VALUES
                               (<FullName, varchar(500),>
                               ,<Country, varchar(50),>
                               ,<Email, varchar(255),>
                               ,<PhoneNumber, varchar(50),>)
                ");
        }

    }
}

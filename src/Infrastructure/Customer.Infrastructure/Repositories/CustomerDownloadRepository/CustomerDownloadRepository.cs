using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Customer.Domain.Configurations;
using Customer.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace Customer.Infrastructure.Repositories.LaunchConfig
{
    public class CustomerDownloadRepository : BaseSqlRepository, ICustomerDownloadRepository
    {
        public CustomerDownloadRepository(IOptions<DbConnectionStrings> options)
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

        public async Task InsertCustomer()
        {
            await ExecuteNonQueryAsync(
                "InsertCustomer",
                @"
                    INSERT INTO [dbo].[CustomerDownloads]
                               ([CustomerId]
                               ,[DownloadDate]
                               ,[DownloadUrl])
                         VALUES
                               (<CustomerId, bigint,>
                               ,<DownloadDate, datetime,>
                               ,<DownloadUrl, varchar(500),>)
                ");
        }

    }
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Giv.Shared;

namespace Customer.Infrastructure.Repositories
{
    public abstract class BaseSqlRepository
    {
        protected string ConnectionString;

        public async Task<ReturnEntity> ExecuteQueryAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            SqlParameter[] parameters,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync)
            => await ExecuteCommandAsync(className, sqlQuery, sqlConnection, sqlTransaction, parameters, readAsync, CommandType.Text);

        public async Task<ReturnEntity> ExecuteQueryAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlParameter[] parameters,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync)
            => await ExecuteCommandAsync(className, sqlQuery, parameters, readAsync, CommandType.Text);

        public async Task<ReturnEntity> ExecuteQueryAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            SqlParameter parameter,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync)
            => await ExecuteCommandAsync(className, sqlQuery, sqlConnection, sqlTransaction, new[] { parameter }, readAsync, CommandType.Text);

        public async Task<ReturnEntity> ExecuteQueryAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync)
            => await ExecuteCommandAsync(className, sqlQuery, readAsync, CommandType.Text);


        public async Task<ReturnEntity> ExecuteQueryAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlParameter parameter,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync)
            => await ExecuteCommandAsync(className, sqlQuery, new[] { parameter }, readAsync, CommandType.Text);

        public async Task ExecuteNonQueryAsync(
            string className,
            string sqlQuery)
            => await ExecuteNonQuery(className, sqlQuery);

        private async Task<int> ExecuteNonQuery(
            string className,
            string sqlQuery)
        {
            using var connection = new SqlConnection(ConnectionString);
            using (var activityOpenConn = DiagnosticController.StartActivity(className + ".OpenAsync"))
            {
                await connection.OpenAsync();
            }

            using var activity = DiagnosticController.StartActivity(className);
            using SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.CommandType = CommandType.Text;

            return await command.ExecuteNonQueryAsync();
        }

        private async Task<ReturnEntity> ExecuteCommandAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlParameter[] parameters,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync,
            CommandType commandType)
        {
            using var connection = new SqlConnection(ConnectionString);
            using (var activityOpenConn = DiagnosticController.StartActivity(className + ".OpenAsync"))
            {
                await connection.OpenAsync();
            }

            using var activity = DiagnosticController.StartActivity(className);
            using SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.Parameters.AddRange(parameters);
            command.CommandType = commandType;
            using var reader = await command.ExecuteReaderAsync();

            return await readAsync(reader);
        }

        private async Task<ReturnEntity> ExecuteCommandAsync<ReturnEntity>(
             string className,
             string sqlQuery,
             Func<SqlDataReader, Task<ReturnEntity>> readAsync,
             CommandType commandType)
        {
            using var connection = new SqlConnection(ConnectionString);
            using (var activityOpenConn = DiagnosticController.StartActivity(className + ".OpenAsync"))
            {
                await connection.OpenAsync();
            }

            using var activity = DiagnosticController.StartActivity(className);
            using SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.CommandType = commandType;
            using var reader = await command.ExecuteReaderAsync();

            return await readAsync(reader);
        }


        private async Task<ReturnEntity> ExecuteCommandAsync<ReturnEntity>(
            string className,
            string sqlQuery,
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            SqlParameter[] parameters,
            Func<SqlDataReader, Task<ReturnEntity>> readAsync,
            CommandType commandType)
        {
            using var activity = DiagnosticController.StartActivity(className);
            using var reader =
                await ExecuteReaderAsync(sqlConnection, sqlQuery, parameters, commandType, sqlTransaction);

            return await readAsync(reader);
        }

        private async Task<SqlDataReader> ExecuteReaderAsync(
            SqlConnection sqlConnection,
            string sqlQuery,
            SqlParameter[] sqlParameters,
            CommandType commandType,
            SqlTransaction sqlTransaction = null)
        {
            using var command = GetSqlCommand(sqlQuery, commandType, sqlParameters, sqlConnection, sqlTransaction);

            return await command.ExecuteReaderAsync();
        }

        private static SqlCommand GetSqlCommand(
            string sqlQuery,
            CommandType commandType,
            SqlParameter[] parameters,
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction = null)
        {
            SqlCommand command = new SqlCommand(sqlQuery, sqlConnection, sqlTransaction);
            command.Parameters.AddRange(parameters);
            command.CommandType = commandType;

            return command;
        }
    }
}

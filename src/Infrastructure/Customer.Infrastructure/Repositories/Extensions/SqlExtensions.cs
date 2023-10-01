using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories.Extensions
{
    internal static class SqlExtensions
    {
        /// <summary>
        /// This will get the value of a column if it is not NULL. Otherwise, it will return default value for the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The SqlReader object to retrieve the value from.</param>
        /// <param name="ordinal">The column number.</param>
        /// <returns></returns>
        public static async Task<T> GetValueOrDefaultAsync<T>(
            this SqlDataReader reader,
            int ordinal)
            => await reader.IsDBNullAsync(ordinal)
                ? default
                : await reader.GetFieldValueAsync<T>(ordinal);

        /// <summary>
        /// This will add an array of parameters to a SqlParameter Collection. This is used for an IN statement.
        /// Use the returned value for the IN part of your SQL call. (i.e. "SELECT * FROM table WHERE field IN (returnValue)")
        /// </summary>
        /// <param name="sqlParameters">The SqlParameter Collection to add parameters to.</param>
        /// <param name="parameterValues">The array of values that need to be added as parameters.</param>
        /// <param name="paramName">What the parameter should be named.</param>
        public static string AddCollectionParameters(
            this ICollection<SqlParameter> sqlParameters,
            IEnumerable<long> parameterValues,
            string paramName)
        {
            var parameterValuesCount = parameterValues.Count();
            var parameters = new string[parameterValuesCount];

            for (int i = 0; i < parameterValuesCount; i++)
            {
                parameters[i] = string.Format("@{0}{1}", paramName, i);
                sqlParameters.Add(new SqlParameter(string.Format("@{0}{1}", paramName, i), parameterValues.ElementAt(i)));
            }

            return string.Join(",", parameters);
        }

        /// <summary>
        /// This will get the value of a string column if it is not NULL. Otherwise, it will return an empty string.
        /// </summary>
        /// <param name="reader">The SqlReader object to retrieve the value from.</param>
        /// <param name="ordinal">The column number.</param>
        /// <returns></returns>
        public static async Task<string> GetStringValueOrEmptyStringAsync(
            this SqlDataReader reader,
            int ordinal)
            => await reader.IsDBNullAsync(ordinal)
                ? string.Empty
                : await reader.GetFieldValueAsync<string>(ordinal);
    }
}

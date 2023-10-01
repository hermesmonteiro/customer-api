using System.ComponentModel.DataAnnotations;

namespace Customer.Domain.Configurations
{
    public class DbConnectionStrings
    {
        public const string LaunchDbConnectionStringName = "GAMING_INTEGRATION_API_DB";

        [Required(ErrorMessage = "Launch DB API Connection String is missing")]
        public string DbConnectionString { get; private set; }

        public DbConnectionStrings() { }

        public void SetProperties(string launchDbApiConnectionString)
            => DbConnectionString = launchDbApiConnectionString;
    }
}

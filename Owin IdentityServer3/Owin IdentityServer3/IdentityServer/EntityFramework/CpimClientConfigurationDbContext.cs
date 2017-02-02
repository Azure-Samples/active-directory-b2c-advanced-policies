//using System.Data.Entity;
using IdentityServer3.EntityFramework;

namespace IdentityServer.EntityFramework
{
    //[DbConfigurationType(typeof(CpimDbConfiguration))]
    public class CpimClientConfigurationDbContext : ClientConfigurationDbContext
    {
        public CpimClientConfigurationDbContext()
            : this("CpimIdentityServerDbConnectionString")
        {
        }

        public CpimClientConfigurationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public CpimClientConfigurationDbContext(string connectionString, string schema)
            : base(connectionString, schema)
        {
        }
    }
}

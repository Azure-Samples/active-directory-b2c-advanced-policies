using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace IdentityServer.EntityFramework
{
    public class CpimDbConfiguration : DbConfiguration
    {
        public CpimDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}

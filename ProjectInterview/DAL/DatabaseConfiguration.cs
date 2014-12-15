using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace ProjectInterview.DAL
{
    public class DatbaseConfiguration : DbConfiguration
    {
        public DatbaseConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
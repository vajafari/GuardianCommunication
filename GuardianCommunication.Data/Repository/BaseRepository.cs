using System.Data.SqlClient;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{
	
	public class BaseRepository
	{

		protected readonly ConnectionConfiguration ConnectionConfig;
		public BaseRepository(ConnectionConfiguration connectionConfig)
		{
			this.ConnectionConfig = connectionConfig;
		}

		protected SqlConnection GetConnection()
		{
			return new SqlConnection(ConnectionConfig.ConnectionString);
		}

        protected SqlConnection GetLogConnection()
        {
            return new SqlConnection(ConnectionConfig.LogConnectionString);
        }

    }
}

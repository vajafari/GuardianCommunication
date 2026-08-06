using System.Data.SqlClient;

namespace GuardianCommunication.Data.Repository
{
	
	public class BaseRepository
	{

		protected readonly ConnectionConfiguration connectionConfig;
		public BaseRepository(ConnectionConfiguration connectionConfig)
		{
			this.connectionConfig = connectionConfig;
		}

		protected SqlConnection GetConnection()
		{
			return new SqlConnection(connectionConfig.ConnectionString);
		}

        protected SqlConnection GetKarnamaLogConnection()
        {
            return new SqlConnection(connectionConfig.KarnamaLogConnectionString);
        }

    }
}

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
	public interface IEndPointUrlRepository
	{

		List<DtoEndPointUrl> GetAll();

		DtoEndPointUrl GetByName(string name);
		
		void Update(List<DtoEndPointUrl> entities);

	}


	public class SqlLiteEndPointUrlRepository : BaseRepository, IEndPointUrlRepository
	{

		public SqlLiteEndPointUrlRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
		{ }

		#region Command Strings DtoGrade


		private const string SelectAllCommand =
			@"	SELECT        
					epu.[EndPointName] AS EndPointName
					, epu.[EndPointUrl] AS EndPointUrl
				FROM EndPointUrl AS epu ";

		private const string SelectByNameCommand =
			@"	SELECT        
					epu.[EndPointName] AS EndPointName
					, epu.[EndPointUrl] AS EndPointUrl
				FROM EndPointUrl AS epu
				WHERE epu.[EndPointName] = @EndPointName";

		private const string UpdateCommand =
			@"	UPDATE	EndPointUrl
					SET [EndPointUrl] = @EndPointUrl
				WHERE [EndPointName] = @EndPointName";



		#endregion


		public List<DtoEndPointUrl> GetAll()
		{
			using (var connection = GetConnection())
			{
				return connection.Query<DtoEndPointUrl>(SelectAllCommand
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).ToList();
			}
		}

		public DtoEndPointUrl GetByName(string name)
		{
			if (name.IsNullOrEmpty())
			{
				return null;
			}

			using (var connection = GetConnection())
			{
				return connection.QueryFirstOrDefault<DtoEndPointUrl>(SelectByNameCommand, param: new
				{
					EndPointName = name
				}, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
			}
		}


		public void Update(List<DtoEndPointUrl> entities)
		{
			using (var connection = GetConnection())
			{
				connection.Execute(UpdateCommand, param: entities
					, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
			}
		}

		


	}
}

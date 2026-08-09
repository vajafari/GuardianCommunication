using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{

	public interface ISystemConfigRepository
	{
		List<DtoSystemConfigPure> GetAllConfigs();

		List<DtoSystemConfigPure> GetConfigsByNames(List<string> configNames);

		void Update(List<DtoSystemConfigPure> entities);
	}

	public class SystemConfigRepository : BaseRepository, ISystemConfigRepository
	{

		public SystemConfigRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
		{ }


		#region Command Strings NewSystemConfig

		private const string SelectCommand =
			@"	SELECT        
				 scfg.[ConfigName] AS ConfigName
				, scfg.[ConfigValue] AS ConfigValue
					FROM [com].[SystemConfig] AS scfg
     		WHERE  1 = 1
     				{0}    -- Search";

		private const string UpdateCommand =
			@"	UPDATE [com].[SystemConfig]
   					SET 
   						[ConfigValue] = @ConfigValue
 				WHERE [ConfigName] =  @ConfigName";

		#endregion


		public List<DtoSystemConfigPure> GetAllConfigs()
		{
			using (var connection = GetConnection())
			{
				return connection.Query<DtoSystemConfigPure>(
                    SelectCommand.FormatInvariantCulture(string.Empty)
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).ToList();
			}
		}

		public List<DtoSystemConfigPure> GetConfigsByNames(List<string> configNames)
		{
			if (configNames.IsCollectionNullOrEmpty())
			{
				return new List<DtoSystemConfigPure>();
			}

			var searchClause = " scfg.[ConfigName] IN ({0})".FormatInvariantCulture(DatabaseHelper.GetInClause(configNames));
			using (var connection = GetConnection())
			{
				return connection.Query<DtoSystemConfigPure>(
                    SelectCommand.FormatInvariantCulture(searchClause)
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).ToList();
			}
		}

		public void Update(List<DtoSystemConfigPure> entities)
		{
			using (var connection = GetConnection())
			{
				connection.Execute(UpdateCommand, param: entities
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
			}
		}



	}

}

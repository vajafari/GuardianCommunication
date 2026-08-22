using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{
	// TODO: Complete this
	public interface IDeviceCommunicationDataRepository
	{
		void Insert(DtoDeviceCommunicationData entity);
		void Update(DtoDeviceCommunicationData entity);
		List<DtoDeviceCommunicationData> GetByDeviceIds(List<Guid> deviceIds);
	}

	public class DeviceCommunicationDataRepository : BaseRepository, IDeviceCommunicationDataRepository
	{

		public DeviceCommunicationDataRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
		{ }


		#region Command Strings NewSupremaDevice

		private const string SelectCommand =
			@"	SELECT dcd.*
				FROM [com].[DeviceCommunicationData] AS dcd
     			WHERE  1 = 1            
     				{0}    -- Search";


		private const string UpdateCommand =
            @"	
UPDATE [com].[DeviceCommunicationData]
   	SET [DeviceCommunicationDataInJson] = @DeviceCommunicationDataInJson
        , [UpdatedAt] = GETUTCDATE() 
WHERE [Id] =  @Id;
";


		private const string InsertCommand =
            @"
INSERT INTO [com].[DeviceCommunicationData]
(
      [Id]
    , [DeviceId]
    , [DeviceCommunicationDataInJson]
    , [InsertedAt]
    , [UpdatedAt]
)
VALUES
(
      @Id
	, @DeviceId
	, @DeviceCommunicationDataInJson
	, GETUTCDATE()
	, NULL
);
			";


		#endregion


		public void Insert(DtoDeviceCommunicationData entity)
		{
			try
            {
				entity.UpdateDeviceSettings();
                entity.Id = Guid.NewGuid();
				using (var connection = GetConnection())
				{
					connection.Execute(InsertCommand, entity
						, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}

		public void Update(DtoDeviceCommunicationData entity)
		{
			entity.UpdateDeviceSettings();
			using (var connection = GetConnection())
			{
				connection.Execute(UpdateCommand, entity
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
			}
		}

		public List<DtoDeviceCommunicationData> GetByDeviceIds(List<Guid> deviceIds)
		{
			if (deviceIds.IsCollectionNullOrEmpty())
			{
				return new List<DtoDeviceCommunicationData>();
			}

			var condition = $" AND sd.[DeviceId] IN @DeviceIds";
			using (var connection = GetConnection())
			{
				return connection.Query<DtoDeviceCommunicationData>(
                    SelectCommand.FormatInvariantCulture(condition)
					, new
                    {
                        DeviceIds = deviceIds
                    }
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
			}
		}



	}

}

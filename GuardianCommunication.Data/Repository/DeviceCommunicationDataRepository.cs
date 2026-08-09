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
		List<DtoDeviceCommunicationData> GetByDeviceNumbers(List<int> deviceNumbers);
	}

	public class DeviceCommunicationDataRepository : BaseRepository, IDeviceCommunicationDataRepository
	{

		public DeviceCommunicationDataRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
		{ }


		#region Command Strings NewSupremaDevice

		private const string SelectCommand =
			@"	SELECT sd.*
				FROM [com].[DeviceCommunicationData] AS sd
     			WHERE  1 = 1            
     				{0}    -- Search";


		private const string UpdateCommand =
			@"	UPDATE [com].[DeviceCommunicationData]
   					SET 
   						  [LastLogId] = @LastLogId
   						, [LastAttendanceLogId] = @LastAttendanceLogId
   						, [LastLogDateTime] = @LastLogDateTime
   						, [LastAttendanceLogDateTime] = @LastAttendanceLogDateTime
 				WHERE [DeviceNumber] =  @DeviceNumber";


		private const string InsertCommand =
			@"	INSERT INTO         [com].[DeviceCommunicationData]
				(
					  [DeviceNumber]
					, [LastLogId]
					, [LastAttendanceLogId]
					, [LastLogDateTime]
					, [LastAttendanceLogDateTime]
				)
				VALUES
				(
					  @DeviceNumber
					, @LastLogId
					, @LastAttendanceLogId
					, @LastLogDateTime
					, @LastAttendanceLogDateTime
				) ;
			";


		#endregion


		public void Insert(DtoDeviceCommunicationData entity)
		{
			try
			{
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
			using (var connection = GetConnection())
			{
				connection.Execute(UpdateCommand, entity
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
			}
		}

		public List<DtoDeviceCommunicationData> GetByDeviceNumbers(List<int> deviceNumbers)
		{
			if (deviceNumbers.IsCollectionNullOrEmpty())
			{
				return new List<DtoDeviceCommunicationData>();
			}

			var condition = $" AND sd.[DeviceNumber] IN ({deviceNumbers.JoinWithComma()})";
			using (var connection = GetConnection())
			{
				return connection.Query<DtoDeviceCommunicationData>(
                    SelectCommand.FormatInvariantCulture(condition)
					, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
			}
		}



	}

}

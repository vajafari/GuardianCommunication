using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace GuardianCommunication.Data.Repository
{

	public interface ICameraCommunicationDataRepository
	{
		void Upsert(DtoCameraCommunicationData entity);
		List<DtoCameraCommunicationData> GetByCameraIds(List<int> cameraIds);
	}

	public class CameraCommunicationDataRepository : BaseRepository, ICameraCommunicationDataRepository
	{

		public CameraCommunicationDataRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
		{ }


		#region Command Strings NewSupremaCamera

		private const string SelectCommand =
			@"	SELECT        
					  ccd.[CameraId] AS CameraId
					, ccd.[LastAttendanceLogDateTime] AS LastAttendanceLogDateTime
				FROM [CameraCommunicationData] AS ccd
     			WHERE  1 = 1            
     				{0}    -- Search";



        protected const string UpsertCommand =
            @"	UPDATE [CameraCommunicationData]
   					SET 
   						  [LastAttendanceLogDateTime] = @LastAttendanceLogDateTime
 				WHERE [CameraId] =  @CameraId;
                IF @@ROWCOUNT = 0
                BEGIN
                    INSERT INTO         [CameraCommunicationData]
				    (
					      [CameraId]
					    , [LastAttendanceLogDateTime]
				    )
				    VALUES
				    (
					      @CameraId
					    , @LastAttendanceLogDateTime
				    );
                END
              ";

        #endregion


        public void Upsert(DtoCameraCommunicationData entity)
		{
			try
			{
				using (var connection = GetConnection())
				{
					connection.Execute(UpsertCommand, entity
						, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}

		public List<DtoCameraCommunicationData> GetByCameraIds(List<int> cameraIds)
		{
			if (cameraIds.IsCollectionNullOrEmpty())
			{
				return new List<DtoCameraCommunicationData>();
			}

			var condition = $" AND ccd.[CameraId] IN ({cameraIds.JoinWithComma()})";
			using (var connection = GetConnection())
			{
				return connection.Query<DtoCameraCommunicationData>(
                    SelectCommand.FormatInvariantCulture(condition)
					, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
			}
		}



	}

}

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
    public interface IOtherHardwareCommandRepository
    {

        List<DtoOtherHardwareCommand> Insert(List<DtoOtherHardwareCommand> entities);

        void SetResponse(DtoOtherHardwareCommandProcessingResult commandResult);

        void SetDescription(DtoOtherHardwareCommandProcessingDescription commandResult);

        void DeleteByIds(List<int> ids);

        void DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypes
            (HardwareTypeEnumeration hardwareType, long objectId, List<OtherHardwareCommandTypeEnumeration> commandTypes);

        void UpdateSendData(List<int> ids);

        List<DtoUnsentCommandForFaceDetectionSystem> GetUnsentCommandsForFaceDetectionSystem(int count);
    }


    public class OtherHardwareCommandRepository : BaseRepository, IOtherHardwareCommandRepository
    {

        public OtherHardwareCommandRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }


        #region Command Strings

        private static readonly Dictionary<OtherHardwareCommandSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<OtherHardwareCommandSortEnumeration, string>
            {
                { OtherHardwareCommandSortEnumeration.Id, "ohc." },
                { OtherHardwareCommandSortEnumeration.HardwareSerialNumber, "ohc.HardwareSerialNumber" },
                { OtherHardwareCommandSortEnumeration.HardwareId, "ohc.HardwareId" },
                { OtherHardwareCommandSortEnumeration.CommandContent, "ohc.CommandContent" },
                { OtherHardwareCommandSortEnumeration.HardwareType, "ohc.HardwareType" },
                { OtherHardwareCommandSortEnumeration.CommitTime, "ohc.CommitTime" },
                { OtherHardwareCommandSortEnumeration.SendTime, "ohc.SendTime" },
                { OtherHardwareCommandSortEnumeration.ResponseTime, "ohc.ResponseTime" },
                { OtherHardwareCommandSortEnumeration.ResponseValue, "ohc.ResponseValue" },
                { OtherHardwareCommandSortEnumeration.CommandType, "ohc.CommandType" },
                { OtherHardwareCommandSortEnumeration.ObjectId, "ohc.ObjectId" },
                { OtherHardwareCommandSortEnumeration.RetryCount, "ohc.RetryCount" },
                { OtherHardwareCommandSortEnumeration.Priority, "ohc.Priority" },
                { OtherHardwareCommandSortEnumeration.MaxRetry, "ohc.MaxRetry" },
                { OtherHardwareCommandSortEnumeration.HardwareContent, "ohc.HardwareContent" },
                { OtherHardwareCommandSortEnumeration.Deadline, "ohc.Deadline" },
                { OtherHardwareCommandSortEnumeration.VisiblilityTime, "ohc.VisiblilityTime" },
                { OtherHardwareCommandSortEnumeration.Description, "ohc.Description" },
                { OtherHardwareCommandSortEnumeration.CommandIdentifier, "ohc.CommandIdentifier" },
            };


        private const string InsertCommand =
            @"	INSERT INTO         [OtherHardwareCommand]
				(
					  [HardwareSerialNumber]
					, [HardwareId]
					, [CommandContent]
					, [HardwareType]
					, [CommitTime]
					, [SendTime]
					, [ResponseTime]
					, [ResponseValue]
					, [CommandType]
					, [ObjectId]
					, [RetryCount]
					, [Priority]
					, [MaxRetry]
					, [HardwareContent]
					, [Deadline]
					, [VisiblilityTime]
					, [Description]
					, [CommandIdentifier]
				)
				VALUES
				(
					  @HardwareSerialNumber
					, @HardwareId
					, @CommandContent
					, @HardwareType
					, @CommitTime
					, @SendTime
					, @ResponseTime
					, @ResponseValue
					, @CommandType
					, @ObjectId
					, @RetryCount
					, @Priority
					, @MaxRetry
					, @HardwareContent
					, @Deadline
					, @VisiblilityTime
					, @Description
					, @CommandIdentifier
				) ;
				SELECT SCOPE_IDENTITY();
			";

        private const string DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypesCommand =
            @"	
                DELETE FROM        [OtherHardwareCommand]
        	    WHERE   [ObjectId] = @ObjectId
                     AND [CommandType] IN @CommandTypes
                     AND [HardwareType] = @HardwareType
                     AND [ResponseTime] IS NULL
            ";


        private const string SetResponseCommand =
            @"	UPDATE        [OtherHardwareCommand]
        		SET  
        			 ResponseTime = @ResponseTime
        			, SendTime = @SendTime
        			, ResponseValue = @ResponseValue
        	WHERE  Id = @Id";

        private const string SetDescriptionCommand =
            @"	UPDATE        [OtherHardwareCommand]
        		SET  
        			 Description = @Description
        	WHERE  Id = @Id";

        private const string DeleteByIdCommand =
            @"	DELETE FROM        [OtherHardwareCommand]
        	WHERE  Id IN ({0})";

        private const string NotSendConditionForOtherHardwareCommand =
            @"
        	ohc.[ResponseTime] IS NULL
        	AND ohc.[RetryCount] < ohc.[MaxRetry]
        	AND (
        			ohc.[VisiblilityTime] IS NULL
        			OR ohc.[VisiblilityTime] <= GETDATE()
        		)
        	AND (
        			ohc.[Deadline] IS NULL 
        			OR ohc.[Deadline] >= GETDATE()
        		)
            ";

        private const string SelectUnsentCommandsForFaceDetectionSystemCommand =
            @"	
        	    SELECT	tmp.[Id] AS Id
        			    , tmp.[CommandContent] AS CommandContent
        			    , tmp.[CommandType] AS CommandType
        	    FROM    (
        	            SELECT  ROW_NUMBER() OVER (ORDER BY ohc.[Priority] DESC, ohc.[RetryCount] ASC, ohc.[CommitTime] ASC) RowNumber
        					    , ohc.[Id] AS Id
        					    , ohc.[CommandContent] AS CommandContent
        					    , ohc.[CommandType] AS CommandType
        	            FROM [OtherHardwareCommand] ohc
        			    WHERE	1 = 1
        					    AND  {0}
        	            ) tmp
        	    WHERE   tmp.RowNumber <= @Count
            ";

        private const string UpdateSendDataCommand =
            @"	UPDATE  [OtherHardwareCommand]
					SET  
						  SendTime = GETDATE()
						, RetryCount = RetryCount + 1
				WHERE  Id IN ({0})";


        #endregion


        #region Private Methods


        private static string GetSearchClause(OtherHardwareCommandFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.EmployeeNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ohc.[EmployeeNumber] IN  ({filter.EmployeeNumbers.JoinWithComma()})");
                }
                if (filter.EmployeeNumberLike.HasValue)
                {
                    sb.AppendLine($" AND ohc.[EmployeeNumber] LIKE {DatabaseHelper.GetLikeClause(filter.EmployeeNumberLike.Value)}");
                }
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine(" AND ohc.[Id] IN @Ids");
                }
                if (filter.CommandIdentifiers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine(" AND ohc.[CommandIdentifier] IN @CommandIdentifiers");
                }
                if (filter.CameraSerialNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ohc.[CameraSerialNumber] IN ({filter.CameraSerialNumbers.Select(row => $"'{row}'").JoinWithComma()})");
                }
                if (filter.CameraIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ohc.[CameraId] IN @{nameof(filter.CameraIds)}");
                }
                if (filter.CommitTimeFrom.HasValue)
                {
                    sb.AppendLine($" AND ohc.[CommitTime] >= @{nameof(filter.CommitTimeFrom)}");
                }
                if (filter.CommitTimeTo.HasValue)
                {
                    sb.AppendLine($" AND ohc.[CommitTime] <= @{nameof(filter.CommitTimeTo)}");
                }
                if (filter.CommandType.HasValue)
                {
                    sb.AppendLine($" AND ohc.[CommandType] = @{nameof(filter.CommandType)}");
                }
                if (filter.Priority.HasValue)
                {
                    sb.AppendLine($" AND ohc.[Priority] = @{nameof(filter.Priority)}");
                }
                if (filter.VisiblilityTimeHasValue.HasValue)
                {
                    sb.AppendLine(!filter.VisiblilityTimeHasValue.Value
                        ? " AND ohc.[VisiblilityTime] IS NOT NULL "
                        : " AND ohc.[VisiblilityTime] IS NULL");
                }
                if (filter.VisiblilityTimeFrom.HasValue)
                {
                    sb.AppendLine($" AND ohc.[VisiblilityTime] >= @{nameof(filter.VisiblilityTimeFrom)}");
                }
                if (filter.VisiblilityTimeTo.HasValue)
                {
                    sb.AppendLine($" AND ohc.[VisiblilityTime] <= @{nameof(filter.VisiblilityTimeTo)}");
                }
                if (filter.IsSend.HasValue)
                {
                    sb.AppendLine(filter.IsSend.Value
                        ? "AND ohc.[ResponseTime] IS NOT NULL"
                        : @"AND ohc.[ResponseTime] IS NULL
								AND (
									ohc.[VisiblilityTime] IS NULL
									OR ohc.[VisiblilityTime] <= GETDATE()
								)
						");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoOtherHardwareCommand> Insert(List<DtoOtherHardwareCommand> entities)
        {
            using (var connection = GetConnection())
            {
                foreach (var entity in entities)
                {
                    entity.Id = connection.ExecuteScalar<int>(InsertCommand, entity
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
                }
            }

            return entities;
        }


        public void DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypes
            (HardwareTypeEnumeration hardwareType, long objectId, List<OtherHardwareCommandTypeEnumeration> commandTypes)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypesCommand,
                    new
                    {
                        ObjectId = objectId,
                        HardwareType = hardwareType,
                        CommandTypes = commandTypes.Select(ct => (int)ct).ToList()
                    }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }

        public void SetResponse(DtoOtherHardwareCommandProcessingResult commandResult)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(SetResponseCommand, new
                {
                    ResponseTime = commandResult.CommandResponseTime,
                    ResponseValue = commandResult.CommandResponseResult,
                    SendTime = commandResult.CommandSendTime,
                    commandResult.Id,
                }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }

        public void SetDescription(DtoOtherHardwareCommandProcessingDescription commandResult)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(SetDescriptionCommand, new
                {
                    Description = commandResult.Description,
                    commandResult.Id,
                }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }

        public void DeleteByIds(List<int> ids)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteByIdCommand.FormatInvariantCulture(ids.JoinWithComma())
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }

        public void UpdateSendData(List<int> ids)
        {
            if (ids.IsCollectionNotNullOrEmpty())
            {
                using (var connection = GetConnection())
                {
                    connection.Execute(UpdateSendDataCommand.FormatInvariantCulture(ids.JoinWithComma())
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
                }
            }
        }

        public List<DtoUnsentCommandForFaceDetectionSystem> GetUnsentCommandsForFaceDetectionSystem(int count)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<DtoUnsentCommandForFaceDetectionSystem>(
                    SelectUnsentCommandsForFaceDetectionSystemCommand.FormatCurrentCulture(NotSendConditionForOtherHardwareCommand)
                    , new { Count = count }
                    , commandType: CommandType.Text
                    , commandTimeout: ConnectionConfig.Timeout).AsList();
            }
        }

        //    public List<DtoOtherHardwareCommandResendInfo> GetUnsentCommandsForEachCamera(CameraNotSentCommandsFilter filter)
        //    {
        //        var sb = new StringBuilder();
        //        if (filter.CameraIds.IsCollectionNotNullOrEmpty())
        //        {
        //            sb.AppendLine($"AND ohc.[CameraId] IN ({filter.CameraIds.JoinWithComma()})");
        //        }
        //        if (filter.CameraSerialNumbers.IsCollectionNotNullOrEmpty())
        //        {
        //            sb.AppendLine($"AND ohc.[CameraSerialNumber] IN ({filter.CameraSerialNumbers.JoinWithComma()})");
        //        }

        //        using (var connection = GetConnection())
        //        {
        //            return connection.Query<DtoOtherHardwareCommandResendInfo>(
        //                SelectUnsentCommandsForEachOtherHardwareCommand.FormatInvariantCulture(NotSendConditionForOtherHardwareCommand, sb.ToString())
        //                , new { filter.Count, filter.Producer, filter.SdkVersion }
        //                , commandType: CommandType.Text
        //                , commandTimeout: connectionConfig.Timeout).AsList();
        //        }
        //    }

        //    public List<DtoFailedCommandStatistics> GetNotSendCommandsStatistics(List<int> CameraIds)
        //    {
        //        using (var connection = GetConnection())
        //        {
        //            return connection.Query<DtoFailedCommandStatistics>(
        //                SelectNotSendCommandsStatisticsCommand, new { CameraIds = CameraIds }
        //                , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
        //        }
        //    }

    }

}

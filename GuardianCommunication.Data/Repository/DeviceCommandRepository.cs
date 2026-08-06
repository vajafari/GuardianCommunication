using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
    public interface IDeviceCommandRepository
    {

        List<DtoDeviceCommand> Insert(List<DtoDeviceCommand> entities);

        List<DtoDeviceUnsentCommand> GetUnsentCommandsForEachDevice(DeviceNotSentCommandsFilter filter);

        List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumberForEachDevice(
            DeviceNotSentCommandsFilter filter);

        List<DtoUnsentCommandCountByDeviceSerialNumber> GetUnsentCommandsCountByDeviceSerialNumberForEachDevice(
            DeviceNotSentCommandsFilter filter);

        void UpdateSendData(List<int> ids);

        void ResetSendData(List<int> ids);

        void SetResponse(DtoDeviceCommandProcessingResult commandResult);

        void SetDescription(DtoDeviceCommandProcessingDescription commandResult);

        void DeleteByIds(List<int> ids, long? mode);

        void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers);

        void DeleteNotSendByDeviceNumber(List<int> deviceNumbers);

        void DeleteNotSentByEmployeeDeviceAndCommandTypes(long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes);

        void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier
            (long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes, List<Guid> commandIds);

        void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandDateInterval
            (long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes, DateTime startDate, DateTime endDate);

        void DeleteFailedBeforeDate(DateTime dateTime, bool justDeleteFailedCommands);

        List<DtoDeviceCommandWithoutContent> SearchWithoutContent(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo);

        List<DtoDeviceCommand> Search(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo);

        int GetCount(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo);

        List<DtoFailedCommandStatistics> GetNotSendCommandsStatistics(List<int> deviceNumbers);
    }


    public class DeviceCommandRepository : BaseRepository, IDeviceCommandRepository
    {

        public DeviceCommandRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }


        #region Command Strings

        private static readonly Dictionary<DeviceCommandSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<DeviceCommandSortEnumeration, string>
            {
                { DeviceCommandSortEnumeration.Id, "dc.[Id]" },
                { DeviceCommandSortEnumeration.DeviceSerialNumber, "dc.[DeviceSerialNumber]" },
                { DeviceCommandSortEnumeration.CommitTime, "dc.[CommitTime]" },
                { DeviceCommandSortEnumeration.SendTime, "dc.[SendTime]" },
                { DeviceCommandSortEnumeration.RetryCount, "dc.[RetryCount]" },
                { DeviceCommandSortEnumeration.ResponseTime, "dc.[ResponseTime]" },
                { DeviceCommandSortEnumeration.CommandType, "dc.[CommandType]" },
                { DeviceCommandSortEnumeration.Priority, "dc.[Priority]" },
                { DeviceCommandSortEnumeration.EmployeeNumber, "dc.[EmployeeNumber]" },
                { DeviceCommandSortEnumeration.MaxRetry, "dc.[MaxRetry]" },
                { DeviceCommandSortEnumeration.DeviceNumber, "dc.[DeviceNumber]" },
                { DeviceCommandSortEnumeration.Deadline, "dc.[Deadline]" },
                { DeviceCommandSortEnumeration.ProducerNumber, "dc.[ProducerNumber]" },
                { DeviceCommandSortEnumeration.SdkVersion, "dc.[SdkVersion]" },
                { DeviceCommandSortEnumeration.VisiblilityTime, "dc.[VisiblilityTime]" },
                { DeviceCommandSortEnumeration.Description, "dc.[Description]" },
            };



        private const string SelectNotSendCommandsStatisticsCommand =
            @"	
				SELECT
					dc.[DeviceNumber],
					COUNT(
						CASE 
							WHEN (dc.[ResponseTime] IS NULL AND dc.[RetryCount] > dc.[MaxRetry]) THEN 1
							ELSE NULL
						END) AS MaxAttemptCommandCount,
					COUNT(
						CASE 
							WHEN (dc.[ResponseTime] IS NULL AND dc.[RetryCount] <= dc.[MaxRetry]) THEN 1
							ELSE NULL
						END) AS NotSendCommandCounts
				FROM	[DeviceCommand] dc
				WHERE	dc.[ResponseTime] IS NULL 
						AND (
								dc.[VisiblilityTime] IS NULL
								OR dc.[VisiblilityTime] <= GETDATE()
							)
						AND dc.[DeviceNumber] IN @DeviceNumbers
				GROUP BY dc.[DeviceNumber]
			";

        private const string NotSendConditionForDeviceCommand =
            @"
						dc.[ResponseTime] IS NULL
						AND dc.[RetryCount] < dc.[MaxRetry]
						AND (
								dc.[VisiblilityTime] IS NULL
								OR dc.[VisiblilityTime] <= GETDATE()
							)
						AND (
								dc.[Deadline] IS NULL 
								OR dc.[Deadline] >= GETDATE()
							)";

        private const string SelectUnsentCommandsForEachDeviceCommand =
            @"	
				SELECT	tmp.[Id] AS Id
						, tmp.[DeviceSerialNumber] AS DeviceSerialNumber
						, tmp.[CommandContent] AS CommandContent
						, tmp.[CommandType] AS CommandType
						, tmp.[DeviceNumber] AS DeviceNumber
						, tmp.[DeviceContent] AS DeviceContent
				FROM    (
				        SELECT  ROW_NUMBER() OVER (PARTITION BY dc.[DeviceNumber] ORDER BY dc.[Priority] DESC, dc.[RetryCount] ASC, dc.[CommitTime] ASC) RowNumber
								, dc.[Id] AS Id
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommandContent] AS CommandContent
								, dc.[CommandType] AS CommandType
								, dc.[DeviceNumber] AS DeviceNumber
								, dc.[DeviceContent] AS DeviceContent
				        FROM [DeviceCommand] dc
						WHERE	dc.[ProducerNumber] = @Producer
								AND	dc.[SdkVersion] = @SdkVersion
								AND  {0}
                                {1}
				        ) tmp
				WHERE   tmp.RowNumber <= @Count
			";

        private const string SelectUnsentCommandsCountForEachDeviceByDeviceNumberCommand =
            @"DeviceCommandCountByDeviceNumber";

        private const string SelectUnsentCommandsCountForEachDeviceByDeviceSerialNumberCommand =
            @"DeviceCommandCountByDeviceSerialNumber";

        private const string CountCommand =
            @"	SELECT        
					  COUNT(dc.[Id])
				FROM [DeviceCommand] dc
				WHERE  1 = 1                
						{0}    -- Search            
				";

        private const string SelectWithoutContentCommand =
            @"	SELECT        
					  dc.[Id] AS Id
					, dc.[DeviceSerialNumber] AS DeviceSerialNumber
					, dc.[CommitTime] AS CommitTime
					, dc.[SendTime] AS SendTime
					, dc.[ResponseTime] AS ResponseTime
					, dc.[ResponseValue] AS ResponseValue
					, dc.[CommandType] AS CommandType
					, dc.[EmployeeNumber] AS EmployeeNumber
					, dc.[RetryCount] AS RetryCount
					, dc.[MaxRetry] AS MaxRetry
					, dc.[Priority] AS Priority
					, dc.[DeviceNumber] AS DeviceNumber
					, dc.[Deadline] AS Deadline
					, dc.[ProducerNumber] AS ProducerNumber
					, dc.[SdkVersion] AS SdkVersion
					, dc.[VisiblilityTime] AS VisiblilityTime
					, dc.[Description] AS Description
					, dc.[CommandIdentifier] AS CommandIdentifier
				FROM [DeviceCommand] dc
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithoutContentWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.DeviceSerialNumber
					 , tmp.CommitTime
					 , tmp.SendTime
					 , tmp.ResponseTime
					 , tmp.ResponseValue
					 , tmp.CommandType
					 , tmp.EmployeeNumber
					 , tmp.RetryCount
					 , tmp.MaxRetry
					 , tmp.Priority
					 , tmp.DeviceNumber
					 , tmp.Deadline
					 , tmp.ProducerNumber
					 , tmp.SdkVersion
					 , tmp.VisiblilityTime
					 , tmp.Description
					 , tmp.CommandIdentifier
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, dc.[Id] AS Id
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommitTime] AS CommitTime
								, dc.[SendTime] AS SendTime
								, dc.[ResponseTime] AS ResponseTime
								, dc.[ResponseValue] AS ResponseValue
								, dc.[CommandType] AS CommandType
								, dc.[EmployeeNumber] AS EmployeeNumber
								, dc.[RetryCount] AS RetryCount
								, dc.[MaxRetry] AS MaxRetry
								, dc.[Priority] AS Priority
								, dc.[DeviceNumber] AS DeviceNumber
								, dc.[Deadline] AS Deadline
								, dc.[ProducerNumber] AS ProducerNumber
								, dc.[SdkVersion] AS SdkVersion
								, dc.[VisiblilityTime] AS VisiblilityTime
								, dc.[Description] AS Description
								, dc.[CommandIdentifier] AS CommandIdentifier
							FROM [DeviceCommand] dc
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string SelectCommand =
            @"	SELECT        
					  dc.[Id] AS Id
					, dc.[DeviceSerialNumber] AS DeviceSerialNumber
					, dc.[CommandContent] AS CommandContent
					, dc.[CommitTime] AS CommitTime
					, dc.[SendTime] AS SendTime
					, dc.[ResponseTime] AS ResponseTime
					, dc.[ResponseValue] AS ResponseValue
					, dc.[CommandType] AS CommandType
					, dc.[EmployeeNumber] AS EmployeeNumber
					, dc.[RetryCount] AS RetryCount
					, dc.[MaxRetry] AS MaxRetry
					, dc.[Priority] AS Priority
					, dc.[DeviceNumber] AS DeviceNumber
					, dc.[Deadline] AS Deadline
					, dc.[DeviceContent] AS DeviceContent
					, dc.[ProducerNumber] AS ProducerNumber
					, dc.[SdkVersion] AS SdkVersion
					, dc.[VisiblilityTime] AS VisiblilityTime
					, dc.[Description] AS Description
					, dc.[CommandIdentifier] AS CommandIdentifier
				FROM [DeviceCommand] dc
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.DeviceSerialNumber
					 , tmp.CommandContent
					 , tmp.CommitTime
					 , tmp.SendTime
					 , tmp.ResponseTime
					 , tmp.ResponseValue
					 , tmp.CommandType
					 , tmp.EmployeeNumber
					 , tmp.RetryCount
					 , tmp.MaxRetry
					 , tmp.Priority
					 , tmp.DeviceNumber
					 , tmp.Deadline
					 , tmp.DeviceContent
					 , tmp.ProducerNumber
					 , tmp.SdkVersion
					 , tmp.VisiblilityTime
					 , tmp.Description
					 , tmp.CommandIdentifier
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, dc.[Id] AS Id
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommandContent] AS CommandContent
								, dc.[CommitTime] AS CommitTime
								, dc.[SendTime] AS SendTime
								, dc.[ResponseTime] AS ResponseTime
								, dc.[ResponseValue] AS ResponseValue
								, dc.[CommandType] AS CommandType
								, dc.[EmployeeNumber] AS EmployeeNumber
								, dc.[RetryCount] AS RetryCount
								, dc.[MaxRetry] AS MaxRetry
								, dc.[Priority] AS Priority
								, dc.[DeviceNumber] AS DeviceNumber
								, dc.[Deadline] AS Deadline
								, dc.[DeviceContent] AS DeviceContent
								, dc.[ProducerNumber] AS ProducerNumber
								, dc.[SdkVersion] AS SdkVersion
								, dc.[VisiblilityTime] AS VisiblilityTime
								, dc.[Description] AS Description
								, dc.[CommandIdentifier] AS CommandIdentifier
							FROM [DeviceCommand] dc
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string InsertCommand =
            @"	INSERT INTO         [DeviceCommand]
				(
					  [DeviceSerialNumber]
					, [CommandContent]
					, [CommitTime]
					, [SendTime]
					, [ResponseTime]
					, [ResponseValue]
					, [CommandType]
					, [EmployeeNumber]
					, [RetryCount]
					, [MaxRetry]
					, [Priority]
					, [DeviceNumber]
					, [Deadline]
					, [DeviceContent]
					, [ProducerNumber]
					, [SdkVersion]
					, [VisiblilityTime]
					, [Description]
					, [CommandIdentifier]
				)
				VALUES
				(
					  @DeviceSerialNumber
					, @CommandContent
					, @CommitTime
					, @SendTime
					, @ResponseTime
					, @ResponseValue
					, @CommandType
					, @EmployeeNumber
					, @RetryCount
					, @MaxRetry
					, @Priority
					, @DeviceNumber
					, @Deadline
					, @DeviceContent
					, @ProducerNumber
					, @SdkVersion
					, @VisiblilityTime
					, @Description
					, @CommandIdentifier
				) ;
				SELECT SCOPE_IDENTITY();
			";

        private const string UpdateSendDataCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						  SendTime = GETDATE()
						, RetryCount = RetryCount + 1
				WHERE  Id IN @Ids";

        private const string ResetSendDataCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						  [SendTime] = NULL
						, [ResponseTime] = NULL
						, [ResponseValue] = NULL
						, [RetryCount] = 0
				WHERE  Id IN ({0})";

        private const string SetResponseCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						 ResponseTime = @ResponseTime
						, ResponseValue = @ResponseValue
				WHERE  Id = @Id";

        private const string SetDescriptionCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						 Description = @Description
				WHERE  Id = @Id";

        private const string SetResponseWithModeCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						 ResponseTime = @ResponseTime
						, ResponseValue = @ResponseValue
				WHERE  (Id % @Mode) = @Id";

        private const string SetDescriptionWithModeCommand =
            @"	UPDATE        [DeviceCommand]
					SET  
						 Description = @Description
				WHERE  (Id % @Mode) = @Id";

        private const string DeleteByCommandIdentifierCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE  CommandIdentifier IN @CommandIdentifiers";

        private const string DeleteByIdsWithModeCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE (Id % @Mode) IN @Ids";

        private const string DeleteByIdsCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE  Id IN @Ids";

        private const string DeleteNotSentByEmployeeAndDeviceAndCommandTypesCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE   [DeviceNumber] = @DeviceNumber
                        AND [EmployeeNumber] = @EmployeeNumber
                        AND [CommandType] IN @CommandTypes
                        AND [ResponseTime] IS NULL
            ";

        private const string DeleteNotSentByEmployeeAndDeviceCommandTypesAndCommandIdentifierCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE   [DeviceNumber] = @DeviceNumber
                        AND [EmployeeNumber] = @EmployeeNumber
                        AND [CommandType] IN @CommandTypes
                        AND [ResponseTime] IS NULL
                        AND [CommandIdentifier] IN @CommandIdentifiers
            ";

        private const string DeleteNotSentByEmployeeDeviceCommandTypesAndDateIntervalCommand =
            @"	DELETE FROM        [DeviceCommand]
				WHERE   [DeviceNumber] = @DeviceNumber
                        AND [EmployeeNumber] = @EmployeeNumber
                        AND [CommandType] IN @CommandTypes
                        AND [ResponseTime] IS NULL
                        AND (
                            [VisiblilityTime] IS NULL
                            OR
                            (
                                [VisiblilityTime] >= @StartDate
                                AND
                                [VisiblilityTime] <= @EndDate
                            )
                        )
                        AND [ResponseTime] IS NULL
            ";

        private const string DeleteNotSendByDeviceNumberCommand =
            @"	DELETE FROM       [DeviceCommand]
				WHERE  DeviceNumber IN ({0})
						AND [ResponseTime] IS NULL
						AND (
							[VisiblilityTime] IS NULL
							OR [VisiblilityTime] <= GETDATE()
						) 
			";


        #endregion


        #region Private Methods


        private static string GetSearchClause(DeviceCommandFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.EmployeeNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND dc.[EmployeeNumber] IN  ({filter.EmployeeNumbers.JoinWithComma()})");
                }
                if (filter.EmployeeNumberLike.HasValue)
                {
                    sb.AppendLine($" AND dc.[EmployeeNumber] LIKE {DatabaseHelper.GetLikeClause(filter.EmployeeNumberLike.Value)}");
                }
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine(" AND dc.[Id] IN @Ids");
                }
                if (filter.CommandIdentifiers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine(" AND dc.[CommandIdentifier] IN @CommandIdentifiers");
                }
                if (filter.DeviceSerialNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND dc.[DeviceSerialNumber] IN ({filter.DeviceSerialNumbers.Select(row => $"'{row}'").JoinWithComma()})");
                }
                if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND dc.[DeviceNumber] IN @{nameof(filter.DeviceNumbers)}");
                }
                if (filter.CommitTimeFrom.HasValue)
                {
                    sb.AppendLine($" AND dc.[CommitTime] >= @{nameof(filter.CommitTimeFrom)}");
                }
                if (filter.CommitTimeTo.HasValue)
                {
                    sb.AppendLine($" AND dc.[CommitTime] <= @{nameof(filter.CommitTimeTo)}");
                }
                if (filter.CommandType.HasValue)
                {
                    sb.AppendLine($" AND dc.[CommandType] = @{nameof(filter.CommandType)}");
                }
                if (filter.Priority.HasValue)
                {
                    sb.AppendLine($" AND dc.[Priority] = @{nameof(filter.Priority)}");
                }
                if (filter.VisiblilityTimeHasValue.HasValue)
                {
                    sb.AppendLine(!filter.VisiblilityTimeHasValue.Value
                        ? " AND dc.[VisiblilityTime] IS NOT NULL "
                        : " AND dc.[VisiblilityTime] IS NULL");
                }
                if (filter.VisiblilityTimeFrom.HasValue)
                {
                    sb.AppendLine($" AND dc.[VisiblilityTime] >= @{nameof(filter.VisiblilityTimeFrom)}");
                }
                if (filter.VisiblilityTimeTo.HasValue)
                {
                    sb.AppendLine($" AND dc.[VisiblilityTime] <= @{nameof(filter.VisiblilityTimeTo)}");
                }
                if (filter.IsSend.HasValue)
                {
                    sb.AppendLine(filter.IsSend.Value
                        ? "AND dc.[ResponseTime] IS NOT NULL"
                        : @"AND dc.[ResponseTime] IS NULL
								AND (
									dc.[VisiblilityTime] IS NULL
									OR dc.[VisiblilityTime] <= GETDATE()
								)
						");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoDeviceCommand> Insert(List<DtoDeviceCommand> entities)
        {
            if (entities.IsCollectionNullOrEmpty())
            {
                return entities;
            }
            using (var connection = GetConnection())
            {
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        entity.Id = connection.ExecuteScalar<int>(InsertCommand, entity
                            , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                    }
                }
            }

            return entities;
        }


        public void DeleteByIds(List<int> ids, long? mode)
        {

            using (var connection = GetConnection())
            {
                if (mode.HasValue)
                {
                    connection.Execute(DeleteByIdsWithModeCommand,
                        new
                        {
                            Mode = mode.Value,
                            Ids = ids
                        }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }
                else
                {
                    connection.Execute(DeleteByIdsCommand
                        , new
                        {
                            Ids = ids
                        }
                        , commandType: CommandType.Text
                        , commandTimeout: connectionConfig.Timeout);
                }
            }
        }

        public void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteByCommandIdentifierCommand, new { CommandIdentifiers = commandIdentifiers }
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void DeleteNotSentByEmployeeDeviceAndCommandTypes(long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteNotSentByEmployeeAndDeviceAndCommandTypesCommand,
                    new
                    {
                        DeviceNumber = deviceNumber,
                        EmployeeNumber = employeeNumber,
                        CommandTypes = commandTypes.Select(ct => (int)ct).ToList()
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier(long employeeNumber, int deviceNumber,
            List<DeviceCommandTypeEnumeration> commandTypes, List<Guid> commandIds)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteNotSentByEmployeeAndDeviceCommandTypesAndCommandIdentifierCommand,
                    new
                    {
                        DeviceNumber = deviceNumber,
                        EmployeeNumber = employeeNumber,
                        CommandTypes = commandTypes.Select(ct => (int)ct).ToList(),
                        CommandIdentifiers = commandIds
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandDateInterval(long employeeNumber, int deviceNumber,
            List<DeviceCommandTypeEnumeration> commandTypes, DateTime startDate, DateTime endDate)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteNotSentByEmployeeDeviceCommandTypesAndDateIntervalCommand,
                    new
                    {
                        DeviceNumber = deviceNumber,
                        EmployeeNumber = employeeNumber,
                        CommandTypes = commandTypes.Select(ct => (int)ct).ToList(),
                        StartDate = startDate,
                        EndDate = endDate
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void DeleteFailedBeforeDate(DateTime dateTime, bool justDeleteFailedCommands)
        {

            var commandText = $@"	DELETE FROM        [DeviceCommand]
				WHERE   [ResponseTime] IS NULL
                        AND [CommitTime] <= @EndDateTime
                        {(justDeleteFailedCommands ? "AND [RetryCount] >= [MaxRetry]" : string.Empty)}
            ";
            using (var connection = GetConnection())
            {
                connection.Execute(commandText,
                    new
                    {
                        EndDateTime = dateTime,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void DeleteNotSendByDeviceNumber(List<int> deviceNumbers)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteNotSendByDeviceNumberCommand.FormatInvariantCulture
                        (deviceNumbers.JoinWithComma())
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void UpdateSendData(List<int> ids)
        {
            if (ids.IsCollectionNotNullOrEmpty())
            {
                using (var connection = GetConnection())
                {
                    connection.Execute(UpdateSendDataCommand
                        , new { Ids = ids }
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }
            }
        }

        public void ResetSendData(List<int> ids)
        {
            if (ids.IsCollectionNotNullOrEmpty())
            {
                using (var connection = GetConnection())
                {
                    connection.Execute(ResetSendDataCommand.FormatInvariantCulture(ids.JoinWithComma())
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }
            }
        }

        public void SetResponse(DtoDeviceCommandProcessingResult commandResult)
        {
            using (var connection = GetConnection())
            {
                if (commandResult.Mode.HasValue)
                {
                    connection.Execute(SetResponseWithModeCommand, new
                    {
                        ResponseTime = commandResult.CommandResponseTime,
                        ResponseValue = commandResult.CommandResponseResult,
                        commandResult.Id,
                        commandResult.Mode,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }
                else
                {
                    connection.Execute(SetResponseCommand, new
                    {
                        ResponseTime = commandResult.CommandResponseTime,
                        ResponseValue = commandResult.CommandResponseResult,
                        commandResult.Id,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }

            }
        }

        public void SetDescription(DtoDeviceCommandProcessingDescription commandResult)
        {
            using (var connection = GetConnection())
            {
                if (commandResult.Mode.HasValue)
                {
                    connection.Execute(SetDescriptionWithModeCommand, new
                    {
                        commandResult.Description,
                        commandResult.Id,
                        commandResult.Mode,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }
                else
                {
                    connection.Execute(SetDescriptionCommand, new
                    {
                        commandResult.Description,
                        commandResult.Id,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }

            }
        }

        public List<DtoDeviceCommandWithoutContent> SearchWithoutContent(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            using (var connection = GetConnection())
            {
                string commandText;
                if (searchInfo != null)
                {
                    var whereClause = GetSearchClause(searchInfo.Filter);
                    var orderByClause = searchInfo.GetNormalSortString(MapSortEnumToFieldName);
                    var pagingClause = searchInfo.GetRowNumberClause("tmp", ServiceConstants.RowNumberColumnName);
                    var searchType = searchInfo.GetSearchType();
                    commandText = searchType == SearchTypeEnumeration.SimpleSearch
                        ? SelectWithoutContentCommand.FormatInvariantCulture(whereClause, orderByClause)
                        : SelectWithoutContentWithPagingCommand.FormatInvariantCulture(whereClause, orderByClause, pagingClause);
                    return connection.Query<DtoDeviceCommandWithoutContent>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectWithoutContentCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceCommandWithoutContent>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

        public List<DtoDeviceCommand> Search(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            using (var connection = GetConnection())
            {
                string commandText;
                if (searchInfo != null)
                {
                    var whereClause = GetSearchClause(searchInfo.Filter);
                    var orderByClause = searchInfo.GetNormalSortString(MapSortEnumToFieldName);
                    var pagingClause = searchInfo.GetRowNumberClause("tmp", ServiceConstants.RowNumberColumnName);
                    var searchType = searchInfo.GetSearchType();
                    commandText = searchType == SearchTypeEnumeration.SimpleSearch
                        ? SelectCommand.FormatInvariantCulture(whereClause, orderByClause)
                        : SelectWithPagingCommand.FormatInvariantCulture(whereClause, orderByClause, pagingClause);
                    return connection.Query<DtoDeviceCommand>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceCommand>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

        public int GetCount(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            using (var connection = GetConnection())
            {
                if (searchInfo != null)
                {
                    return connection.ExecuteScalar<int>(
                            CountCommand.FormatInvariantCulture(GetSearchClause(searchInfo.Filter))
                        , searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                }

                return connection.ExecuteScalar<int>(
                    CountCommand.FormatInvariantCulture(string.Empty)
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public List<DtoDeviceUnsentCommand> GetUnsentCommandsForEachDevice(DeviceNotSentCommandsFilter filter)
        {
            var sb = new StringBuilder();
            if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
            {
                sb.AppendLine($"AND dc.[DeviceNumber] IN @{nameof(filter.DeviceNumbers)}");
            }
            if (filter.DeviceSerialNumbers.IsCollectionNotNullOrEmpty())
            {
                sb.AppendLine($"AND dc.[DeviceSerialNumber] IN @{nameof(filter.DeviceSerialNumbers)}");
            }

            using (var connection = GetConnection())
            {
                return connection.Query<DtoDeviceUnsentCommand>(
                    SelectUnsentCommandsForEachDeviceCommand.FormatInvariantCulture(NotSendConditionForDeviceCommand, sb.ToString())
                    , new { filter.Count, filter.Producer, filter.SdkVersion, filter.DeviceNumbers, filter.DeviceSerialNumbers }
                    , commandType: CommandType.Text
                    , commandTimeout: connectionConfig.Timeout).AsList();
            }
        }

        public List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumberForEachDevice(DeviceNotSentCommandsFilter filter)
        {
            if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Producer", (int)filter.Producer);
                parameters.Add("@SdkVersion", (int)filter.SdkVersion);
                parameters.Add(
                    "@DeviceNumbersList",
                    CreateDeviceNumberTable(filter.DeviceNumbers)
                        .AsTableValuedParameter("dbo.IntegerValueListTable"));
                using (var connection = GetConnection())
                {
                    return connection.Query<DtoUnsentCommandCountByDeviceNumber>(
                        SelectUnsentCommandsCountForEachDeviceByDeviceNumberCommand
                        , parameters
                        , commandType: CommandType.StoredProcedure
                        , commandTimeout: connectionConfig.Timeout).AsList();
                }
            }

            return new List<DtoUnsentCommandCountByDeviceNumber>();
        }

        public List<DtoUnsentCommandCountByDeviceSerialNumber> GetUnsentCommandsCountByDeviceSerialNumberForEachDevice(DeviceNotSentCommandsFilter filter)
        {
            if (filter.DeviceSerialNumbers.IsCollectionNotNullOrEmpty())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Producer", (int)filter.Producer);
                parameters.Add("@SdkVersion", (int)filter.SdkVersion);
                parameters.Add(
                    "@DeviceSerialNumbersList",
                    CreateDeviceSerialNumberTable(filter.DeviceSerialNumbers)
                        .AsTableValuedParameter("dbo.StringValueListTable"));
                using (var connection = GetConnection())
                {
                    return connection.Query<DtoUnsentCommandCountByDeviceSerialNumber>(
                        SelectUnsentCommandsCountForEachDeviceByDeviceSerialNumberCommand
                        , parameters
                        , commandType: CommandType.StoredProcedure
                        , commandTimeout: connectionConfig.Timeout).AsList();
                }
            }
            return new List<DtoUnsentCommandCountByDeviceSerialNumber>();
        }

        public List<DtoFailedCommandStatistics> GetNotSendCommandsStatistics(List<int> deviceNumbers)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<DtoFailedCommandStatistics>(
                    SelectNotSendCommandsStatisticsCommand, new { DeviceNumbers = deviceNumbers }
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
            }
        }





        private static DataTable CreateDeviceSerialNumberTable(IEnumerable<string> serialNumbers)
        {
            var table = new DataTable();

            table.Columns.Add("@DeviceSerialNumbersList", typeof(string));

            foreach (var serialNumber in serialNumbers)
            {
                table.Rows.Add(serialNumber);
            }

            return table;
        }

        private static DataTable CreateDeviceNumberTable(IEnumerable<int> deviceNumbers)
        {
            var table = new DataTable();

            table.Columns.Add("@DeviceNumbersList", typeof(int));

            foreach (var deviceNumber in deviceNumbers)
            {
                table.Rows.Add(deviceNumber);
            }

            return table;
        }
    }

}

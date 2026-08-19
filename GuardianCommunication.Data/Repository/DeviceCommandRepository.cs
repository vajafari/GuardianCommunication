using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{
    public interface IDeviceCommandRepository
    {

        List<DtoDeviceCommand> Insert(List<DtoDeviceCommand> entities);

        List<DtoDeviceUnsentCommand> GetUnsentCommandsForEachDevice(DeviceNotSentCommandsFilter filter);

        List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumbers(
            DeviceNotSentCommandsCountByDeviceNumberFilter filter);

        List<DtoUnsentCommandCountByDeviceId> GetUnsentCommandsCountByDeviceIds(
            DeviceNotSentCommandsCountByDeviceIdFilter filter);

        List<DtoUnsentCommandCountByDeviceSerialNumber> GetUnsentCommandsCountByDeviceSerialNumber(
            DeviceNotSentCommandsCountByDeviceSerialNumberFilter filter);

        void UpdateSendData(List<long> numericIds);

        void SetResponse(DtoDeviceCommandProcessingResult commandResult);

        void SetDescription(DtoDeviceCommandProcessingDescription commandResult);

        void DeleteByIds(List<Guid> ids, long? mode);

        void DeleteByNumericIds(List<long> numericIds, long? mode);

        void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers);

        List<DtoDeviceCommandWithoutContent> SearchWithoutContent(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo);

        List<DtoDeviceCommand> Search(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo);
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
            };


        private const string InsertCommand =
            @"	
            INSERT INTO [com].[DeviceCommand]
            (
                  [Id]
                , [DeviceId]
                , [DeviceNumber]
                , [DeviceContent]
                , [DeviceSerialNumber]
                , [UserIdOnDevice]
                , [CommandContent]
                , [CommitTime]
                , [SendTime]
                , [ResponseTime]
                , [ResponseValue]
                , [CommandType]
                , [RetryCount]
                , [Priority]
                , [MaxRetry]
                , [Deadline]
                , [ProducerNumber]
                , [SdkVersion]
                , [VisiblilityTime]
                , [Description]
                , [CommandIdentifier]
                , [InsertedAt]
                , [UpdatedAt]
            )
            VALUES
            (
                  @Id
                , @DeviceId
                , @DeviceNumber
                , @DeviceContent
                , @DeviceSerialNumber
                , @UserIdOnDevice
                , @CommandContent
                , @CommitTime
                , @SendTime
                , @ResponseTime
                , @ResponseValue
                , @CommandType
                , @RetryCount
                , @Priority
                , @MaxRetry
                , @Deadline
                , @ProducerNumber
                , @SdkVersion
                , @VisiblilityTime
                , @Description
                , @CommandIdentifier
                , GETUTCDATE()
                , NULL
            );
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
			";

        private const string SelectUnsentCommandsForEachDeviceCommand =
            @"	
				SELECT	  tmp.[Id] AS Id
						, tmp.[NumericId] AS NumericId
						, tmp.[DeviceId] AS DeviceId
						, tmp.[DeviceNumber] AS DeviceNumber
						, tmp.[DeviceSerialNumber] AS DeviceSerialNumber
						, tmp.[CommandType] AS CommandType
						, tmp.[CommandContent] AS CommandContent
						, tmp.[DeviceContent] AS DeviceContent
				FROM    (
				        SELECT  ROW_NUMBER() OVER (PARTITION BY dc.[DeviceId] ORDER BY dc.[Priority] DESC, dc.[RetryCount] ASC, dc.[CommitTime] ASC) RowNumber
								, dc.[Id] AS Id
								, dc.[NumericId] AS NumericId
								, dc.[DeviceId] AS DeviceId
								, dc.[DeviceNumber] AS DeviceNumber
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommandType] AS CommandType
								, dc.[CommandContent] AS CommandContent
								, dc.[DeviceContent] AS DeviceContent
				        FROM [com].[DeviceCommand] dc
						WHERE	dc.[ProducerNumber] = @Producer
								AND	dc.[SdkVersion] = @SdkVersion
								AND  {0}
                                {1}
				        ) tmp
				WHERE   tmp.RowNumber <= @Count
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

        private const string SelectUnsentCommandsCountByDeviceNumberCommand =
            @"[com].[DeviceCommandCountByDeviceNumber]";

        private const string SelectUnsentCommandsCountByDeviceIdCommand =
            @"[com].[DeviceCommandCountByDeviceId]";

        private const string SelectUnsentCommandsCountByDeviceSerialNumberCommand =
            @"[com].[DeviceCommandCountByDeviceSerialNumber]";

        private const string UpdateSendDataCommand =
            @"	UPDATE        [com].[DeviceCommand]
					SET  
						  [SendTime] = GETUTCDATE()
						, [RetryCount] = RetryCount + 1
                        , [UpdatedAt] = GETUTCDATE()
				WHERE  [NumericId] IN @NumericIds";

        private const string SetResponseWithModeCommand =
            @"	UPDATE        [com].[DeviceCommand]
					SET  
						  [ResponseTime] = @ResponseTime
						, [ResponseValue] = @ResponseValue
				        , [UpdatedAt] = GETUTCDATE()
                WHERE  ([NumericId] % @Mode) = @NumericId";

        private const string SetResponseCommand =
            @"	UPDATE        [com].[DeviceCommand]
					SET  
						  [ResponseTime] = @ResponseTime
						, [ResponseValue] = @ResponseValue
				WHERE  [NumericId] = @NumericId";

        private const string SetDescriptionCommand =
            @"	UPDATE        [com].[DeviceCommand]
					SET  
						 Description = @Description
				WHERE  Id = @Id";

        private const string SetDescriptionWithModeCommand =
            @"	UPDATE        [com].[DeviceCommand]
					SET  
						 Description = @Description
				WHERE  (Id % @Mode) = @Id";

        private const string SelectWithoutContentCommand =
            @"	SELECT        
					  dc.[Id] AS Id
					, dc.[NumericId] AS NumericId
					, dc.[DeviceSerialNumber] AS DeviceSerialNumber
					, dc.[CommitTime] AS CommitTime
					, dc.[SendTime] AS SendTime
					, dc.[ResponseTime] AS ResponseTime
					, dc.[ResponseValue] AS ResponseValue
					, dc.[CommandType] AS CommandType
					, dc.[UserIdOnDevice] AS UserIdOnDevice
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
				FROM [com].[DeviceCommand] dc
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithoutContentWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.NumericId
					 , tmp.DeviceSerialNumber
					 , tmp.CommitTime
					 , tmp.SendTime
					 , tmp.ResponseTime
					 , tmp.ResponseValue
					 , tmp.CommandType
					 , tmp.UserIdOnDevice
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
								, dc.[NumericId] AS NumericId
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommitTime] AS CommitTime
								, dc.[SendTime] AS SendTime
								, dc.[ResponseTime] AS ResponseTime
								, dc.[ResponseValue] AS ResponseValue
								, dc.[CommandType] AS CommandType
								, dc.[UserIdOnDevice] AS UserIdOnDevice
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
							FROM [com].[DeviceCommand] dc
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string SelectCommand =
            @"	SELECT        
					  dc.[Id] AS Id
					, dc.[NumericId] AS NumericId
					, dc.[DeviceSerialNumber] AS DeviceSerialNumber
					, dc.[CommandContent] AS CommandContent
					, dc.[CommitTime] AS CommitTime
					, dc.[SendTime] AS SendTime
					, dc.[ResponseTime] AS ResponseTime
					, dc.[ResponseValue] AS ResponseValue
					, dc.[CommandType] AS CommandType
					, dc.[UserIdOnDevice] AS UserIdOnDevice
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
				FROM [com].[DeviceCommand] dc
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.NumericId
					 , tmp.DeviceSerialNumber
					 , tmp.CommandContent
					 , tmp.CommitTime
					 , tmp.SendTime
					 , tmp.ResponseTime
					 , tmp.ResponseValue
					 , tmp.CommandType
					 , tmp.UserIdOnDevice
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
								, dc.[NumericId] AS NumericId
								, dc.[DeviceSerialNumber] AS DeviceSerialNumber
								, dc.[CommandContent] AS CommandContent
								, dc.[CommitTime] AS CommitTime
								, dc.[SendTime] AS SendTime
								, dc.[ResponseTime] AS ResponseTime
								, dc.[ResponseValue] AS ResponseValue
								, dc.[CommandType] AS CommandType
								, dc.[UserIdOnDevice] AS UserIdOnDevice
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
							FROM [com].[DeviceCommand] dc
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string DeleteByCommandIdentifierCommand =
            @"	DELETE FROM        [com].[DeviceCommand]
				WHERE  CommandIdentifier IN @CommandIdentifiers";

        private const string DeleteByIdsWithModeCommand =
            @"	DELETE FROM        [com].[DeviceCommand]
				WHERE (Id % @Mode) IN @Ids";

        private const string DeleteByIdsCommand =
            @"	DELETE FROM        [com].[DeviceCommand]
				WHERE  Id IN @Ids";

        private const string DeleteByNumericIdsWithModeCommand =
            @"	DELETE FROM        [com].[DeviceCommand]
				WHERE (NumericId % @Mode) IN @NumericIds";

        private const string DeleteByNumericIdsCommand =
            @"	DELETE FROM        [com].[DeviceCommand]
				WHERE  NumericId IN @NumericIds";


        #endregion


        #region Private Methods


        private static string GetSearchClause(DeviceCommandFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
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

        private static DynamicParameters GetInsertParameters(DtoDeviceCommand entity)
        {
            var parameters = new DynamicParameters(entity);
            parameters.Add(nameof(entity.CommitTime), entity.CommitTime.ToUniversalTime());
            parameters.Add(nameof(entity.SendTime), entity.SendTime?.ToUniversalTime());
            parameters.Add(nameof(entity.ResponseTime), entity.ResponseTime?.ToUniversalTime());
            parameters.Add(nameof(entity.Deadline), entity.Deadline?.ToUniversalTime());
            parameters.Add(nameof(entity.VisiblilityTime), entity.VisiblilityTime?.ToUniversalTime());
            return parameters;
        }

        private static DynamicParameters GetSearchParameters(DeviceCommandFilter filter)
        {
            var parameters = new DynamicParameters(filter);
            if (filter != null)
            {
                parameters.Add(nameof(filter.CommitTimeFrom), filter.CommitTimeFrom?.ToUniversalTime());
                parameters.Add(nameof(filter.CommitTimeTo), filter.CommitTimeTo?.ToUniversalTime());
                parameters.Add(nameof(filter.VisiblilityTimeFrom), filter.VisiblilityTimeFrom?.ToUniversalTime());
                parameters.Add(nameof(filter.VisiblilityTimeTo), filter.VisiblilityTimeTo?.ToUniversalTime());
            }
            return parameters;
        }

        #endregion


        public List<DtoDeviceCommand> Insert(List<DtoDeviceCommand> entities)
        {
            if (entities.IsCollectionNullOrEmpty())
            {
                return entities;
            }
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
            }
            using (var connection = GetConnection())
            {
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        entity.NumericId = connection.ExecuteScalar<long>(InsertCommand, GetInsertParameters(entity)
                            , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                    }
                }
            }

            return entities;
        }

        public List<DtoDeviceUnsentCommand> GetUnsentCommandsForEachDevice(DeviceNotSentCommandsFilter filter)
        {
            var sb = new StringBuilder();
            if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
            {
                sb.AppendLine($"AND dc.[DeviceNumber] IN @{nameof(filter.DeviceNumbers)}");
            }
            if (filter.DeviceIds.IsCollectionNotNullOrEmpty())
            {
                sb.AppendLine($"AND dc.[DeviceId] IN @{nameof(filter.DeviceIds)}");
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
                    , commandTimeout: ConnectionConfig.CommandTimeout).AsList();
            }
        }

        public List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumbers(DeviceNotSentCommandsCountByDeviceNumberFilter filter)
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
                        SelectUnsentCommandsCountByDeviceNumberCommand
                        , parameters
                        , commandType: CommandType.StoredProcedure
                        , commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }
            }

            return new List<DtoUnsentCommandCountByDeviceNumber>();
        }

        public List<DtoUnsentCommandCountByDeviceId> GetUnsentCommandsCountByDeviceIds(DeviceNotSentCommandsCountByDeviceIdFilter filter)
        {
            if (filter.DeviceIds.IsCollectionNotNullOrEmpty())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Producer", (int)filter.Producer);
                parameters.Add("@SdkVersion", (int)filter.SdkVersion);
                parameters.Add(
                    "@DeviceNumbersList",
                    CreateDeviceIdTable(filter.DeviceIds)
                        .AsTableValuedParameter("dbo.UniqueIdentifierValueListTable"));
                using (var connection = GetConnection())
                {
                    return connection.Query<DtoUnsentCommandCountByDeviceId>(
                        SelectUnsentCommandsCountByDeviceIdCommand
                        , parameters
                        , commandType: CommandType.StoredProcedure
                        , commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }
            }

            return new List<DtoUnsentCommandCountByDeviceId>();
        }

        public List<DtoUnsentCommandCountByDeviceSerialNumber> GetUnsentCommandsCountByDeviceSerialNumber(DeviceNotSentCommandsCountByDeviceSerialNumberFilter filter)
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
                        SelectUnsentCommandsCountByDeviceSerialNumberCommand
                        , parameters
                        , commandType: CommandType.StoredProcedure
                        , commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }
            }
            return new List<DtoUnsentCommandCountByDeviceSerialNumber>();
        }

        public void UpdateSendData(List<long> numericIds)
        {
            if (numericIds.IsCollectionNotNullOrEmpty())
            {
                using (var connection = GetConnection())
                {
                    connection.Execute(UpdateSendDataCommand
                        , new { NumericIds = numericIds }
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
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
                        ResponseTime = commandResult.CommandResponseTime.ToUniversalTime(),
                        ResponseValue = commandResult.CommandResponseResult,
                        commandResult.NumericId,
                        commandResult.Mode,
                    }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                }
                else
                {
                    connection.Execute(SetResponseCommand, new
                    {
                        ResponseTime = commandResult.CommandResponseTime.ToUniversalTime(),
                        ResponseValue = commandResult.CommandResponseResult,
                        commandResult.NumericId,
                    }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
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
                        Id = commandResult.NumericId,
                        commandResult.Mode,
                    }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                }
                else
                {
                    connection.Execute(SetDescriptionCommand, new
                    {
                        commandResult.Description, Id = commandResult.NumericId,
                    }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                }

            }
        }

        public void DeleteByIds(List<Guid> ids, long? mode)
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
                        }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                }
                else
                {
                    connection.Execute(DeleteByIdsCommand
                        , new
                        {
                            Ids = ids
                        }
                        , commandType: CommandType.Text
                        , commandTimeout: ConnectionConfig.CommandTimeout);
                }
            }
        }

        public void DeleteByNumericIds(List<long> numericIds, long? mode)
        {

            using (var connection = GetConnection())
            {
                if (mode.HasValue)
                {
                    connection.Execute(DeleteByIdsWithModeCommand,
                        new
                        {
                            Mode = mode.Value,
                            NumericIds = numericIds
                        }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                }
                else
                {
                    connection.Execute(DeleteByIdsCommand
                        , new
                        {
                            NumericIds = numericIds
                        }
                        , commandType: CommandType.Text
                        , commandTimeout: ConnectionConfig.CommandTimeout);
                }
            }
        }

        public void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(DeleteByCommandIdentifierCommand, new { CommandIdentifiers = commandIdentifiers }
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
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
                    return connection.Query<DtoDeviceCommandWithoutContent>(commandText, GetSearchParameters(searchInfo.Filter)
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectWithoutContentCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceCommandWithoutContent>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
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
                    return connection.Query<DtoDeviceCommand>(commandText, GetSearchParameters(searchInfo.Filter)
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceCommand>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }


        #region Helper Methods

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

        private static DataTable CreateDeviceIdTable(IEnumerable<Guid> deviceIds)
        {
            var table = new DataTable();

            table.Columns.Add("@DeviceIdsList", typeof(int));

            foreach (var deviceId in deviceIds)
            {
                table.Rows.Add(deviceId);
            }

            return table;
        }

        #endregion


    }

}

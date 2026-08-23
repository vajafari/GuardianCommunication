using Dapper;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GuardianCommunication.Data.Repository
{
    public interface IAttendanceRepository
    {

        Guid? Insert(DtoAttendance entity, List<DtoAttendanceRegisterIntervalSetting> intervalSettings, List<Guid> hookDefinitionIds);

        void MarkAsSentToGuardian(List<DtoAttendance> entities);

        void IncreaseSentToGuardianRetryCount(List<DtoAttendance> entities);

        bool CheckExistence(long userIdOnDevice, DateTime attendanceDate
            , List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings);

        List<DtoAttendance> Search(PagingData<AttendanceFilter, AttendanceSortEnumeration> searchInfo);

        List<DtoUnhookedAttendances> GetUnhookedAttendances(int count);

        List<DtoUnhookedAttendances> GetUnsentToGuardianAttendances(int count, int retryCount);

    }


    public class AttendanceRepository : BaseRepository, IAttendanceRepository
    {
        private static readonly object LockObject = new object();

        public AttendanceRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<AttendanceSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<AttendanceSortEnumeration, string>()
            {
                { AttendanceSortEnumeration.Id, "att.[Id]" },
                { AttendanceSortEnumeration.AttendanceDate, "att.[AttendanceDateTime]" },
                { AttendanceSortEnumeration.UserIdOnDevice, "att.[UserIdOnDevice]" }
            };



        private const string SelectCommand =
            @"	SELECT        
					  att.*
	            FROM [com].[Attendance] att
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.*
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, att.*
				            FROM [com].[Attendance] att
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private static readonly string CheckExistenceCommand =
            $@"	SELECT  COUNT(att.[Id])   
				FROM [com].[Attendance] att
				WHERE  att.[UserIdOnDevice] = @PersonNumberOnDevice
                       AND att.[AttendanceSource] != {(int)AttendanceSourceEnumeration.Manual}
					   AND ({{0}})";

        private const string SelectUnhookedAttendancesCommand =
            @"	SELECT  TOP ({0})       
					  att.*
					, ahd.[HookDefinitionId] AS  HookDefinitionId
				FROM [com].[AttendanceHookDefinition] ahd
						INNER JOIN [com].[Attendance] att ON att.[Id] = ahd.[AttendanceId]
						INNER JOIN [com].[HookDefinition] hd ON hd.[Id] = ahd.[HookDefinitionId]
				WHERE 
					hd.[IsActive] > 0
					AND ahd.[RetryCount] <= hd.[RetryCount]
					AND ahd.[IsSent] = 0
				ORDER BY ahd.[RetryCount] DESC
			";

        private const string SelectUnsentToGuardianAttendancesCommand =
            @"	SELECT  TOP ({0})       
					  att.*
				FROM [com].[Attendance] att ON att.[Id] = ahd.[AttendanceId]
				WHERE  att.[IsSentToGuardian] = 0
					AND att.[SentToGuardianRetryCount] <= @RetryCount
				ORDER BY ahd.[SentToGuardianRetryCount] DESC
			";

        private const string InsertNormalCommand =
            @"	
                DECLARE @CurrentId UNIQUEIDENTIFIER;
                SET @CurrentId = NEWID();
                INSERT INTO [com].[Attendance]
                (
                     [Id]
                   , [UserIdOnDevice]
                   , [LogIdOnDevice]
                   , [AttendanceDateTime]
                   , [DeviceId]
                   , [CameraId]
                   , [ReaderDeviceId]
                   , [LocationId]
                   , [VerificationStyle]
                   , [RfCardNumber]
                   , [StatusCode]
                   , [IsSentToGuardian]
                   , [SentToGuardianRetryCount]
                   , [ModuleId]
                   , [IoType]
                   , [AttendanceSource]
                   , [DeviceAttendanceIoRetrieveType]
                   , [InsertedAt]
                   , [UpdatedAt]
                )
                VALUES
                (
                     @CurrentId
                   , @UserIdOnDevice
                   , @LogIdOnDevice
                   , @AttendanceDateTime
                   , @DeviceId
                   , @CameraId
                   , @ReaderDeviceId
                   , @LocationId
                   , @VerificationStyle
                   , @RfCardNumber
                   , @StatusCode
                   , @IsSentToGuardian
                   , @SentToGuardianRetryCount
                   , @ModuleId
                   , @IoType
                   , @AttendanceSource
                   , @DeviceAttendanceIoRetrieveType
                   , GETUTCDATE()
                   , NULL
                );
                SELECT @CurrentId;
			";

        private const string InsertWithConditionCommand =
            @"	IF {0}
                BEGIN
                    DECLARE @CurrentId UNIQUEIDENTIFIER;
                    SET @CurrentId = NEWID();
                
                    INSERT INTO [com].[Attendance]
                    (
                         [Id]
                       , [UserIdOnDevice]
                       , [LogIdOnDevice]
                       , [AttendanceDateTime]
                       , [DeviceId]
                       , [CameraId]
                       , [ReaderDeviceId]
                       , [LocationId]
                       , [VerificationStyle]
                       , [RfCardNumber]
                       , [StatusCode]
                       , [IsSentToGuardian]
                       , [SentToGuardianRetryCount]
                       , [ModuleId]
                       , [IoType]
                       , [AttendanceSource]
                       , [DeviceAttendanceIoRetrieveType]
                       , [InsertedAt]
                       , [UpdatedAt]
                    )
                    VALUES
                    (
                         @CurrentId
                       , @UserIdOnDevice
                       , @LogIdOnDevice
                       , @AttendanceDateTime
                       , @DeviceId
                       , @CameraId
                       , @ReaderDeviceId
                       , @LocationId
                       , @VerificationStyle
                       , @RfCardNumber
                       , @StatusCode
                       , @IsSentToGuardian
                       , @SentToGuardianRetryCount
                       , @ModuleId
                       , @IoType
                       , @AttendanceSource
                       , @DeviceAttendanceIoRetrieveType
                       , GETUTCDATE()
                       , NULL
                    );
                    SELECT @CurrentId;
                END
                ELSE
                BEGIN
                    SELECT NULL;
                END
			";

        private const string InsertAttendanceHookDefinitionCommand =
            @"	
                IF @CurrentId IS NOT NULL
                BEGIN
                    INSERT INTO         [com].[AttendanceHookDefinition]
				    (
					      [AttendanceId]
					    , [HookDefinitionId]
					    , [IsSent]
					    , [RetryCount]
					    , [SentTime]
				    )
				    VALUES
				    (
					      @CurrentId
					    , {0}
					    , 0
					    , 0
					    , NULL
				    )
                END
			";

        private const string InsertNormalWithAttendanceHookDefinitionsCommand =
            @"	
                DECLARE @CurrentId UNIQUEIDENTIFIER;
                SET @CurrentId = NEWID();
                
                INSERT INTO [com].[Attendance]
                (
                     [Id]
                   , [UserIdOnDevice]
                   , [LogIdOnDevice]
                   , [AttendanceDateTime]
                   , [DeviceId]
                   , [CameraId]
                   , [ReaderDeviceId]
                   , [LocationId]
                   , [VerificationStyle]
                   , [RfCardNumber]
                   , [StatusCode]
                   , [IsSentToGuardian]
                   , [SentToGuardianRetryCount]
                   , [ModuleId]
                   , [IoType]
                   , [AttendanceSource]
                   , [DeviceAttendanceIoRetrieveType]
                   , [InsertedAt]
                   , [UpdatedAt]
                )
                VALUES
                (
                     @CurrentId
                   , @UserIdOnDevice
                   , @LogIdOnDevice
                   , @AttendanceDateTime
                   , @DeviceId
                   , @CameraId
                   , @ReaderDeviceId
                   , @LocationId
                   , @VerificationStyle
                   , @RfCardNumber
                   , @StatusCode
                   , @IsSentToGuardian
                   , @SentToGuardianRetryCount
                   , @ModuleId
                   , @IoType
                   , @AttendanceSource
                   , @DeviceAttendanceIoRetrieveType
                   , GETUTCDATE()
                   , NULL
                );
                {0}
                SELECT @CurrentId;
			";

        private const string InsertWithConditionWithAttendanceHookDefinitionsCommand =
            @"	IF {0}
                BEGIN
                    DECLARE @CurrentId UNIQUEIDENTIFIER;
                    SET @CurrentId = NEWID();
                    
                    INSERT INTO [com].[Attendance]
                    (
                         [Id]
                       , [UserIdOnDevice]
                       , [LogIdOnDevice]
                       , [AttendanceDateTime]
                       , [DeviceId]
                       , [CameraId]
                       , [ReaderDeviceId]
                       , [LocationId]
                       , [VerificationStyle]
                       , [RfCardNumber]
                       , [StatusCode]
                       , [IsSentToGuardian]
                       , [SentToGuardianRetryCount]
                       , [ModuleId]
                       , [IoType]
                       , [AttendanceSource]
                       , [DeviceAttendanceIoRetrieveType]
                       , [InsertedAt]
                       , [UpdatedAt]
                    )
                    VALUES
                    (
                         @CurrentId
                       , @UserIdOnDevice
                       , @LogIdOnDevice
                       , @AttendanceDateTime
                       , @DeviceId
                       , @CameraId
                       , @ReaderDeviceId
                       , @LocationId
                       , @VerificationStyle
                       , @RfCardNumber
                       , @StatusCode
                       , @IsSentToGuardian
                       , @SentToGuardianRetryCount
                       , @ModuleId
                       , @IoType
                       , @AttendanceSource
                       , @DeviceAttendanceIoRetrieveType
                       , GETUTCDATE()
                       , NULL
                    );
                    {1}
                    SELECT @CurrentId;
                END
                ELSE
                BEGIN
                    SELECT NULL;
                END
			";

        private const string MarkAsSentToGuardianCommand =
            @"	UPDATE        [com].[Attendance]
					SET IsSentToGuardian = 1
				WHERE  Id IN @Ids";

        private const string IncreaseSentToGuardianRetryCountCommand =
            @"	UPDATE        [com].[Attendance]
					SET SentToGuardianRetryCount = ISNULL(SentToGuardianRetryCount , 0) + 1
				WHERE  Id IN @Ids";


        #endregion

        #region Private Methods


        private static string GetSearchClause(AttendanceFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[Id] IN @{nameof(filter.Ids)}");
                }
                if (filter.IsHooked.HasValue)
                {
                    sb.AppendLine(filter.IsHooked.Value
                        ? " AND EXISTS (SELECT 1 FROM [com].[AttendanceHookDefinition] ahd WHERE ahd.[AttendanceId] = att.[Id] AND ahd.[IsSent] = 1)"
                        : " AND NOT EXISTS (SELECT 1 FROM [com].[AttendanceHookDefinition] ahd WHERE ahd.[AttendanceId] = att.[Id] AND ahd.[IsSent] = 1)");
                }
                if (filter.AttendanceDateFrom.HasValue)
                {
                    sb.AppendLine($" AND att.[AttendanceDateTime] >= @{nameof(filter.AttendanceDateFrom)}");
                }
                if (filter.AttendanceDateTo.HasValue)
                {
                    sb.AppendLine($" AND att.[AttendanceDateTime] <= @{nameof(filter.AttendanceDateTo)}");
                }
                if (filter.DeviceIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[DeviceId] IN @{nameof(filter.DeviceIds)}");
                }
                if (filter.UsersIdOnDevice.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[UserIdOnDevice] IN @{nameof(filter.UsersIdOnDevice)}");
                }
                if (filter.IsSentToGuardian.HasValue)
                {
                    sb.AppendLine($" AND att.[IsSentToGuardian] = @{nameof(filter.IsSentToGuardian)}");
                }
            }

            return sb.ToString();

        }

        private static DynamicParameters GetSearchParameters(AttendanceFilter filter)
        {
            var parameters = new DynamicParameters(filter);
            if (filter != null)
            {
                parameters.Add(nameof(filter.AttendanceDateFrom), filter.AttendanceDateFrom?.ToUniversalTime());
                parameters.Add(nameof(filter.AttendanceDateTo), filter.AttendanceDateTo?.ToUniversalTime());
            }
            return parameters;
        }

        private static DynamicParameters GetInsertParameters(DtoAttendance entity)
        {
            var parameters = new DynamicParameters(entity);
            parameters.Add(nameof(entity.AttendanceDateTime), entity.AttendanceDateTime.ToUniversalTime());
            return parameters;
        }


        #endregion

        public Guid? Insert(DtoAttendance entity
            , List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings
            , List<Guid> hookDefinitionIds)
        {
            if (hookDefinitionIds.IsCollectionNotNullOrEmpty())
            {
                var sb = new StringBuilder();
                foreach (var hookDefinitionId in hookDefinitionIds)
                {
                    sb.AppendLine(InsertAttendanceHookDefinitionCommand.FormatInvariantCulture(hookDefinitionId));
                }
                if (registerIntervalSettings.IsCollectionNullOrEmpty())
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<Guid?>(
                            InsertNormalWithAttendanceHookDefinitionsCommand.FormatInvariantCulture
                                (sb.ToString())
                            , GetInsertParameters(entity)
                            , commandType: CommandType.Text
                            , commandTimeout: ConnectionConfig.CommandTimeout);
                        return resultOfInsert;
                    }
                }

                var parameters = GetInsertParameters(entity);
                var condition = GetCheckExistenceCondition(registerIntervalSettings, entity, parameters);
                lock (LockObject)
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<Guid?>(
                            InsertWithConditionWithAttendanceHookDefinitionsCommand.
                                FormatInvariantCulture(condition, sb.ToString())
                            , parameters
                            , commandType: CommandType.Text
                            , commandTimeout: ConnectionConfig.CommandTimeout);
                        if (resultOfInsert.HasValue)
                        {
                            entity.Id = resultOfInsert.Value;
                        }
                        return resultOfInsert;
                    }
                }
            }
            else
            {
                if (registerIntervalSettings.IsCollectionNullOrEmpty())
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<Guid?>(InsertNormalCommand
                            , GetInsertParameters(entity)
                            , commandType: CommandType.Text
                            , commandTimeout: ConnectionConfig.CommandTimeout);
                        return resultOfInsert;
                    }
                }

                var parameters = GetInsertParameters(entity);
                var condition = GetCheckExistenceCondition(registerIntervalSettings, entity, parameters);
                lock (LockObject)
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<Guid?>(
                            InsertWithConditionCommand.FormatInvariantCulture(condition)
                            , parameters
                            , commandType: CommandType.Text
                            , commandTimeout: ConnectionConfig.CommandTimeout);
                        if (resultOfInsert.HasValue)
                        {
                            entity.Id = resultOfInsert.Value;
                        }

                        return resultOfInsert;
                    }
                }
            }
        }

        public void MarkAsSentToGuardian(List<DtoAttendance> entities)
        {

            if (entities.IsCollectionNullOrEmpty()) return;
            using (var connection = GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var batch in entities.Batch(500))
                        {
                            connection.Execute(
                                MarkAsSentToGuardianCommand
                                , batch
                                , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void IncreaseSentToGuardianRetryCount(List<DtoAttendance> entities)
        {

            if (entities.IsCollectionNullOrEmpty()) return;
            using (var connection = GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var batch in entities.Batch(500))
                        {
                            connection.Execute(
                                IncreaseSentToGuardianRetryCountCommand
                                , batch
                                , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<DtoAttendance> Search(PagingData<AttendanceFilter, AttendanceSortEnumeration> searchInfo)
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
                    return connection.Query<DtoAttendance>(commandText, GetSearchParameters(searchInfo.Filter)
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoAttendance>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }

        public bool CheckExistence(long employeeNumber, DateTime attendanceDate, List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings)
        {
            attendanceDate = attendanceDate.ToUniversalTime();

            var conditions = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("PersonNumberOnDevice", employeeNumber);

            for (var i = 0; i < registerIntervalSettings.Count; i++)
            {
                var setting = registerIntervalSettings[i];
                var applicationCondition = string.Empty;
                switch (setting.ApplicationType)
                {
                    case AttendanceRegisterIntervalTypeEnumeration.Parking:
                        applicationCondition =
                            $"AND (att.[ModuleId] & {(int)ModuleEnumeration.Parking}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.General:
                    default:
                        break;
                }

                var startDateParameterName = $"StartDate{i}";
                var endDateParameterName = $"EndDate{i}";
                parameters.Add(startDateParameterName, attendanceDate.AddMinutes(setting.Interval * -1));
                parameters.Add(endDateParameterName, attendanceDate.AddMinutes(setting.Interval));

                conditions.Add($@" (
                                        att.[AttendanceDateTime] >=  @{startDateParameterName}
                                        AND att.[AttendanceDateTime] <=  @{endDateParameterName}
                                        {applicationCondition}
                                    )");
            }

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                    CheckExistenceCommand.FormatInvariantCulture(string.Join("\n OR \n", conditions)),
                    parameters, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout) > 0;
            }
        }

        public List<DtoUnhookedAttendances> GetUnhookedAttendances(int count)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<DtoUnhookedAttendances>(
                    SelectUnhookedAttendancesCommand.FormatInvariantCulture(count)
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).ToList();
            }
        }

        public List<DtoUnhookedAttendances> GetUnsentToGuardianAttendances(int count, int retryCount)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<DtoUnhookedAttendances>(
                    SelectUnsentToGuardianAttendancesCommand.FormatInvariantCulture(count)
                    , new
                    {
                        RetryCount = retryCount
                    }
                    , commandType: CommandType.Text
                    , commandTimeout: ConnectionConfig.CommandTimeout).ToList();
            }
        }

        private static string GetCheckExistenceCondition(List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings, DtoAttendance entity, DynamicParameters parameters)
        {
            if (registerIntervalSettings.IsCollectionNullOrEmpty()) return null;
            var conditions = new List<string>();
            for (var i = 0; i < registerIntervalSettings.Count; i++)
            {
                var setting = registerIntervalSettings[i];
                var applicationCondition = string.Empty;
                switch (setting.ApplicationType)
                {
                    case AttendanceRegisterIntervalTypeEnumeration.Parking:
                        applicationCondition =
                            $"AND (att.[ModuleId] & {(int)ModuleEnumeration.Parking}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.General:
                    default:
                        break;
                }

                var startDateParameterName = $"CheckExistenceStartDate{i}";
                var endDateParameterName = $"CheckExistenceEndDate{i}";
                parameters.Add(startDateParameterName, entity.AttendanceDateTime.ToUniversalTime().AddMinutes(setting.Interval * -1));
                parameters.Add(endDateParameterName, entity.AttendanceDateTime.ToUniversalTime().AddMinutes(setting.Interval));

                conditions.Add($@" (
                                        att.[AttendanceDateTime] >= @{startDateParameterName}
                                        AND att.[AttendanceDateTime] <= @{endDateParameterName}
                                        {applicationCondition}
                                    )");
            }

            return $@"NOT EXISTS (
                                        SELECT  *
                                        FROM [com].[Attendance] att
                                        WHERE   att.[UserIdOnDevice] = @PersonNumberOnDevice
                                                AND att.[AttendanceSource] != {(int)AttendanceSourceEnumeration.Manual}
                                                AND ({string.Join("\n OR \n", conditions)})
                                    )";
        }

    }

}

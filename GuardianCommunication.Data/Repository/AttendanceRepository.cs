using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
    public interface IAttendanceRepository
    {

        long Insert(DtoAttendance entity, List<DtoAttendanceRegisterIntervalSetting> intervalSettings, List<int> hookSystemIds);

        void MarkAsSent(List<DtoAttendance> entities);

        bool CheckExistence(long employeeNumber, DateTime attendanceDate
            , List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings);

        List<DtoAttendance> Search(PagingData<AttendanceFilter, AttendanceSortEnumeration> searchInfo);

        List<DtoUnhookedAttendances> GetUnhookedAttendances(int count);

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
                { AttendanceSortEnumeration.EmployeeNumber, "att.[EmployeeNumber]" }
            };



        private const string SelectCommand =
            @"	SELECT        
					  att.[Id] AS Id
					, att.[EmployeeNumber] AS EmployeeNumber
					, att.[RfCardNumber] AS RfCardNumber
					, att.[IoType] AS IoType
					, att.[VerificationStyle] AS VerificationStyle
					, att.[DoorId] AS DoorId
					, att.[AttendanceDateTime] AS AttendanceDateTime
					, att.[AttendanceSource] AS AttendanceSource
					, att.[DeviceAttendanceIoRetrieveType] AS DeviceAttendanceIoRetrieveType
					, att.[DeviceNumber] AS DeviceNumber
					, att.[ReaderDeviceNumber] AS ReaderDeviceNumber
					, att.[CameraId] AS CameraId
					, att.[CameraId] AS CameraId
					, att.[StatusCode] AS StatusCode
					, att.[IsSent] AS IsSent
					, att.[IsInvalid] AS IsInvalid
					, att.[ApplicationId] AS ApplicationId
					, att.[InsertDateTime] AS InsertDateTime
	            FROM [Attendance] att
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.EmployeeNumber
					 , tmp.RfCardNumber
					 , tmp.IoType
					 , tmp.VerificationStyle
					 , tmp.DoorId
					 , tmp.AttendanceDateTime
					 , tmp.AttendanceSource
					 , tmp.DeviceAttendanceIoRetrieveType
					 , tmp.DeviceNumber
					 , tmp.ReaderDeviceNumber
					 , tmp.CameraId
					 , tmp.StatusCode
					 , tmp.IsSent
					 , tmp.IsInvalid
					 , tmp.ApplicationId
					 , tmp.InsertDateTime
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, att.[Id] AS Id
								, att.[EmployeeNumber] AS EmployeeNumber
								, att.[RfCardNumber] AS RfCardNumber
								, att.[IoType] AS IoType
								, att.[VerificationStyle] AS VerificationStyle
								, att.[DoorId] AS DoorId
								, att.[AttendanceDateTime] AS AttendanceDateTime
								, att.[AttendanceSource] AS AttendanceSource
								, att.[DeviceAttendanceIoRetrieveType] AS DeviceAttendanceIoRetrieveType
								, att.[DeviceNumber] AS DeviceNumber
								, att.[ReaderDeviceNumber] AS ReaderDeviceNumber
								, att.[CameraId] AS CameraId
								, att.[StatusCode] AS StatusCode
								, att.[IsSent] AS IsSent
								, att.[IsInvalid] AS IsInvalid
								, att.[ApplicationId] AS ApplicationId
								, att.[InsertDateTime] AS InsertDateTime
				            FROM [Attendance] att
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private static readonly string CheckExistenceCommand =
            $@"	SELECT  COUNT(att.[Id])   
				FROM [Attendance] att
				WHERE  att.[EmployeeNumber] = @EmployeeNumber
                       AND att.[AttendanceSource] != {(int)AttendanceSourceEnumeration.Manual}
					   AND ({{0}})";

        private const string SelectUnhookedAttendancesCommand =
            @"	SELECT  TOP ({0})       
					  att.[Id] AS Id
					, att.[EmployeeNumber] AS EmployeeNumber
					, att.[RfCardNumber] AS RfCardNumber
					, att.[IoType] AS IoType
					, att.[VerificationStyle] AS VerificationStyle
					, att.[DoorId] AS DoorId
					, att.[AttendanceDateTime] AS AttendanceDateTime
					, att.[AttendanceSource] AS AttendanceSource
					, att.[DeviceAttendanceIoRetrieveType] AS DeviceAttendanceIoRetrieveType
					, att.[DeviceNumber] AS DeviceNumber
					, att.[ReaderDeviceNumber] AS ReaderDeviceNumber
					, att.[CameraId] AS CameraId
					, att.[StatusCode] AS StatusCode
					, att.[IsSent] AS IsSent
					, att.[IsInvalid] AS IsInvalid
					, att.[ApplicationId] AS ApplicationId
					, att.[InsertDateTime] AS InsertDateTime
					, ahs.[HookSystemId] AS  HookSystemId
				FROM [AttendanceHookSystem] ahs
						INNER JOIN [Attendance] att ON ahs.[AttendanceId] = att.[Id]
						INNER JOIN [HookSystemDetail] hsd ON ahs.[HookSystemId] = hsd.[HookSystemId]
				WHERE 
					hsd.[IsActive] > 0
					AND ahs.[RetryCount] <= hsd.[RetryCount]
					AND ahs.[IsSent] = 0
				ORDER BY ahs.[RetryCount] DESC
			";

        private const string InsertNormalCommand =
            @"	
                INSERT INTO         [Attendance]
				(
					  [EmployeeNumber]
					, [RfCardNumber]
					, [IoType]
					, [VerificationStyle]
					, [DoorId]
					, [AttendanceDateTime]
					, [AttendanceSource]
					, [DeviceAttendanceIoRetrieveType]
					, [StatusCode]
					, [DeviceNumber]
					, [ReaderDeviceNumber]
					, [CameraId]
					, [IsSent]
					, [IsInvalid]
					, [ApplicationId]
					, [InsertDateTime]
				)
				VALUES
				(
					  @EmployeeNumber
					, @RfCardNumber
					, @IoType
					, @VerificationStyle
					, @DoorId
					, @AttendanceDateTime
					, @AttendanceSource
					, @DeviceAttendanceIoRetrieveType
					, @StatusCode
					, @DeviceNumber
					, @ReaderDeviceNumber
					, @CameraId
					, @IsSent
					, @IsInvalid
					, @ApplicationId
					, @InsertDateTime
				) ;
				SELECT SCOPE_IDENTITY();
			";

        private const string InsertWithConditionCommand =
            @"	IF {0}
                BEGIN
                    INSERT INTO         [Attendance]
				    (
					      [EmployeeNumber]
					    , [RfCardNumber]
					    , [IoType]
					    , [VerificationStyle]
					    , [DoorId]
					    , [AttendanceDateTime]
					    , [AttendanceSource]
					    , [DeviceAttendanceIoRetrieveType]
					    , [StatusCode]
					    , [DeviceNumber]
					    , [ReaderDeviceNumber]
					    , [CameraId]
					    , [IsSent]
					    , [IsInvalid]
					    , [ApplicationId]
					    , [InsertDateTime]
				    )
				    VALUES
				    (
					      @EmployeeNumber
					    , @RfCardNumber
					    , @IoType
					    , @VerificationStyle
					    , @DoorId
					    , @AttendanceDateTime
					    , @AttendanceSource
					    , @DeviceAttendanceIoRetrieveType
					    , @StatusCode
					    , @DeviceNumber
					    , @ReaderDeviceNumber
					    , @CameraId
					    , @IsSent
					    , @IsInvalid
					    , @ApplicationId
					    , @InsertDateTime
				    ) ;
				    SELECT SCOPE_IDENTITY();
                END
                ELSE
                BEGIN
                    SELECT -1;
                END
			";

        private const string InsertAttendanceHookSystemCommand =
            @"	
                IF @LastId > 0
                BEGIN
                    INSERT INTO         [AttendanceHookSystem]
				    (
					      [AttendanceId]
					    , [HookSystemId]
					    , [IsSent]
					    , [RetryCount]
					    , [SentTime]
				    )
				    VALUES
				    (
					      @LastId
					    , {0}
					    , 0
					    , 0
					    , NULL
				    )
                END
			";

        private const string InsertNormalWithAttendanceHookSystemsCommand =
            @"	
                INSERT INTO         [Attendance]
				(
					  [EmployeeNumber]
					, [RfCardNumber]
					, [IoType]
					, [VerificationStyle]
					, [DoorId]
					, [AttendanceDateTime]
					, [AttendanceSource]
					, [DeviceAttendanceIoRetrieveType]
					, [StatusCode]
					, [DeviceNumber]
					, [ReaderDeviceNumber]
					, [CameraId]
					, [IsSent]
					, [IsInvalid]
					, [ApplicationId]
					, [InsertDateTime]
				)
				VALUES
				(
					  @EmployeeNumber
					, @RfCardNumber
					, @IoType
					, @VerificationStyle
					, @DoorId
					, @AttendanceDateTime
					, @AttendanceSource
					, @DeviceAttendanceIoRetrieveType
					, @StatusCode
					, @DeviceNumber
					, @ReaderDeviceNumber
					, @CameraId
					, @IsSent
					, @IsInvalid
					, @ApplicationId
					, @InsertDateTime
				) ;
                DECLARE @LastId BIGINT
                SET @LastId= SCOPE_IDENTITY();
                {0}
                SELECT @LastId;
			";

        private const string InsertWithConditionWithAttendanceHookSystemsCommand =
            @"	IF {0}
                BEGIN
                    DECLARE @LastId BIGINT;
                    INSERT INTO         [Attendance]
				    (
					      [EmployeeNumber]
					    , [RfCardNumber]
					    , [IoType]
					    , [VerificationStyle]
					    , [DoorId]
					    , [AttendanceDateTime]
					    , [AttendanceSource]
					    , [DeviceAttendanceIoRetrieveType]
					    , [StatusCode]
					    , [DeviceNumber]
					    , [ReaderDeviceNumber]
					    , [CameraId]
					    , [IsSent]
					    , [IsInvalid]
					    , [ApplicationId]
					    , [InsertDateTime]
				    )
				    VALUES
				    (
					      @EmployeeNumber
					    , @RfCardNumber
					    , @IoType
					    , @VerificationStyle
					    , @DoorId
					    , @AttendanceDateTime
					    , @AttendanceSource
					    , @DeviceAttendanceIoRetrieveType
					    , @StatusCode
					    , @DeviceNumber
					    , @ReaderDeviceNumber
					    , @CameraId
					    , @IsSent
					    , @IsInvalid
					    , @ApplicationId
					    , @InsertDateTime
				    ) ;
                    SELECT @LastId = SCOPE_IDENTITY();
                    {1}
                    SELECT @LastId;
                END
                ELSE
                BEGIN
                    SELECT -1;
                END
			";

        private const string MarkAsSentCommand =
            @"	UPDATE        [Attendance]
					SET IsSent = 1
				WHERE  Id IN ({0})";


        #endregion


        #region Private Methods


        private static string GetSearchClause(AttendanceFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[Id] IN ({filter.Ids.JoinWithComma()})");
                }
                if (filter.EmployeeNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[EmployeeNumber] IN  ({filter.EmployeeNumbers.JoinWithComma()})");
                }
                if (filter.AttendanceDate.HasValue)
                {
                    sb.AppendLine($" AND att.[AttendanceDateTime] = @{nameof(filter.AttendanceDate)}");
                }
                if (filter.AttendanceDateFrom.HasValue)
                {
                    sb.AppendLine($" AND att.[AttendanceDateTime] >= @{nameof(filter.AttendanceDateFrom)}");
                }
                if (filter.AttendanceDateTo.HasValue)
                {
                    sb.AppendLine($" AND att.[AttendanceDateTime] <= @{nameof(filter.AttendanceDateTo)}");
                }
                if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND att.[DeviceNumber] IN @{nameof(filter.DeviceNumbers)}");
                }
                if (filter.IsSent.HasValue)
                {
                    sb.AppendLine($" AND att.[IsSent] = @{nameof(filter.IsSent)}");
                }
                if (filter.IsHooked.HasValue)
                {
                    sb.AppendLine($" AND att.[IsHooked] = @{nameof(filter.IsHooked)}");
                }
            }

            return sb.ToString();

        }


        #endregion

        public long Insert(DtoAttendance entity, List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings, List<int> hookSystemIds)
        {
            entity.InsertDateTime = DateTime.Now;
            if (hookSystemIds.IsCollectionNotNullOrEmpty())
            {
                var sb = new StringBuilder();
                foreach (var hookSystemId in hookSystemIds)
                {
                    sb.AppendLine(InsertAttendanceHookSystemCommand.FormatInvariantCulture(hookSystemId));
                }
                if (registerIntervalSettings.IsCollectionNullOrEmpty())
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<long>(InsertNormalWithAttendanceHookSystemsCommand.FormatInvariantCulture(sb.ToString())
                            , entity
                            , commandType: CommandType.Text
                            , commandTimeout: connectionConfig.Timeout);
                        entity.Id = resultOfInsert;
                        return resultOfInsert;
                    }
                }

                var condition = GetCheckExistenceCondition(registerIntervalSettings, entity);
                lock (LockObject)
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<long>(
                            InsertWithConditionWithAttendanceHookSystemsCommand.FormatInvariantCulture(condition, sb.ToString())
                            , entity
                            , commandType: CommandType.Text
                            , commandTimeout: connectionConfig.Timeout);
                        if (resultOfInsert > 0)
                        {
                            entity.Id = resultOfInsert;
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
                        var resultOfInsert = connection.ExecuteScalar<long>(InsertNormalCommand
                            , entity
                            , commandType: CommandType.Text
                            , commandTimeout: connectionConfig.Timeout);
                        entity.Id = resultOfInsert;
                        return resultOfInsert;
                    }
                }

                var condition = GetCheckExistenceCondition(registerIntervalSettings, entity);
                lock (LockObject)
                {
                    using (var connection = GetConnection())
                    {
                        var resultOfInsert = connection.ExecuteScalar<long>(
                            InsertWithConditionCommand.FormatInvariantCulture(condition)
                            , entity
                            , commandType: CommandType.Text
                            , commandTimeout: connectionConfig.Timeout);
                        if (resultOfInsert > 0)
                        {
                            entity.Id = resultOfInsert;
                        }

                        return resultOfInsert;
                    }
                }
            }
        }

        public void MarkAsSent(List<DtoAttendance> entities)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(MarkAsSentCommand.FormatInvariantCulture(entities.Select(row => row.Id).JoinWithComma())
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
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
                    return connection.Query<DtoAttendance>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoAttendance>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

        // TODO: Parameterize this
        public bool CheckExistence(long employeeNumber, DateTime attendanceDate, List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings)
        {
            var conditions = new List<string>();
            foreach (var setting in registerIntervalSettings)
            {
                var applicationCondition = string.Empty;
                switch (setting.ApplicationType)
                {
                    case AttendanceRegisterIntervalTypeEnumeration.AccessControl:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.AccessControl}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.TimeAndAttendance:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.TimeAndAttendance}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.Parking:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.Parking}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.General:
                    default:
                        break;
                }
                conditions.Add($@" (
                                        att.[AttendanceDateTime] >=  {DatabaseHelper.ParameterValueForSql(attendanceDate.AddMinutes(setting.Interval * -1))}
                                        AND att.[AttendanceDateTime] <=  {DatabaseHelper.ParameterValueForSql(attendanceDate.AddMinutes(setting.Interval))}
                                        {applicationCondition}
                                    )");
            }

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<int>(
                    CheckExistenceCommand.FormatInvariantCulture(string.Join("\n OR \n", conditions)),
                    new
                    {
                        EmployeeNumber = employeeNumber,
                    }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout) > 0;
            }
        }

        public List<DtoUnhookedAttendances> GetUnhookedAttendances(int count)
        {
            using (var connection = GetConnection())
            {
                return connection.Query<DtoUnhookedAttendances>(
                    SelectUnhookedAttendancesCommand.FormatInvariantCulture(count)
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).ToList();
            }
        }

        private string GetCheckExistenceCondition(List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings, DtoAttendance entity)
        {
            if (registerIntervalSettings.IsCollectionNullOrEmpty()) return null;
            var conditions = new List<string>();
            foreach (var setting in registerIntervalSettings)
            {
                var applicationCondition = string.Empty;
                switch (setting.ApplicationType)
                {
                    case AttendanceRegisterIntervalTypeEnumeration.AccessControl:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.AccessControl}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.TimeAndAttendance:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.TimeAndAttendance}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.Parking:
                        applicationCondition =
                            $"AND (att.[ApplicationId] & {(int)ApplicationTypeEnumeration.Parking}) > 0";
                        break;
                    case AttendanceRegisterIntervalTypeEnumeration.General:
                    default:
                        break;
                }
                conditions.Add($@" (
                                        att.[AttendanceDateTime] >= {DatabaseHelper.ParameterValueForSql(entity.AttendanceDateTime.AddMinutes(setting.Interval * -1))}
                                        AND att.[AttendanceDateTime] <= {DatabaseHelper.ParameterValueForSql(entity.AttendanceDateTime.AddMinutes(setting.Interval))}
                                        {applicationCondition}
                                    )");
            }

            return $@"NOT EXISTS (  
                                        SELECT  *   
                                        FROM [Attendance] att
				                        WHERE   att.[EmployeeNumber] = @EmployeeNumber
                                                AND att.[AttendanceSource] != {(int)AttendanceSourceEnumeration.Manual}
					                            AND ({string.Join("\n OR \n", conditions)}) 
                                    )";
        }

    }

}

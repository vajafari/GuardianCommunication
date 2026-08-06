using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{

    public interface IAttendanceHookSystemRepository
    {

        List<DtoAttendanceHookSystem> Search(PagingData<AttendanceHookSystemFilter, AttendanceHookSystemSortEnumeration> searchInfo);

        //void Insert(List<DtoAttendanceHookSystem> entities);

        void MarkAsSent(long attendanceId, int hookSystemId);

        void IncreaseRetryCount(long attendanceId, int hookSystemId);

    }


    public class AttendanceHookSystemRepository : BaseRepository, IAttendanceHookSystemRepository
    {

        public AttendanceHookSystemRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<AttendanceHookSystemSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<AttendanceHookSystemSortEnumeration, string>()
            {
                { AttendanceHookSystemSortEnumeration.AttendanceId, "ahs.[AttendanceId]" },
                { AttendanceHookSystemSortEnumeration.HookSystemId, "ahs.[HookSystemId]" }
            };


        private const string SelectCommand =
            @"	SELECT        
					  ahs.[Id] AS  Id
					, ahs.[AttendanceId] AS  AttendanceId
					, ahs.[HookSystemId] AS  HookSystemId
					, ahs.[IsSent] AS  IsSent
					, ahs.[RetryCount] AS  RetryCount
					, ahs.[SentTime] AS  SentTime
	            FROM [dbo].[AttendanceHookSystem] AS ahs
				WHERE	1 = 1            
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
				   tmp.Id
				 , tmp.AttendanceId
				 , tmp.HookSystemId
				 , tmp.IsSent
				 , tmp.RetryCount
				 , tmp.SentTime
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, ahs.[Id] AS  Id
								, ahs.[AttendanceId] AS  AttendanceId
								, ahs.[HookSystemId] AS  HookSystemId
								, ahs.[IsSent] AS  IsSent
								, ahs.[RetryCount] AS  RetryCount
								, ahs.[SentTime] AS  SentTime
				            FROM [dbo].[AttendanceHookSystem] AS ahs
				            WHERE	1 = 1            
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        internal const string Delete =
            @"	
                DELETE FROM [AttendanceHookSystem]
				WHERE AttendanceId = @AttendanceId;
			";

        private const string MarkAsSentCommand =
            @"	UPDATE        [AttendanceHookSystem]
					SET   IsSent = 1
						, SentTime = @CurrentTime
				WHERE  AttendanceId = @AttendanceId
						AND HookSystemId = @HookSystemId";

        private const string IncreaseRetryCountCommand =
            @"	UPDATE        [AttendanceHookSystem]
					SET   RetryCount = RetryCount + 1
				WHERE  AttendanceId = @AttendanceId
						AND HookSystemId = @HookSystemId";

        private const string ResetHookByEmployeeNumberAndDateTimeCommand =
            @"	UUPDATE ahs
                    SET
                            ahs.[IsSent] = 0
                          , ahs.[RetryCount] = 0
                          , ahs.[SentTime] = NULL
                FROM [AttendanceHookSystem] ahs
						INNER JOIN [Attendance] att ON ahs.[AttendanceId] = att.[Id]
						INNER JOIN [HookSystemDetail] hsd ON ahs.[HookSystemId] = hsd.[HookSystemId]
				WHERE 
					hsd.[IsActive] > 0
					AND ahs.[RetryCount] > hsd.[RetryCount]
					AND ahs.[IsSent] = 0
                    AND att.[EmployeeNumber] = @EmployeeNumber
                    AND att.[AttendanceDateTime] = @AttendanceDateTime
            ";

        #endregion


        #region Private Methods

        private static string GetSearchClause(AttendanceHookSystemFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.AttendanceIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ahs.[AttendanceId] IN ({filter.AttendanceIds.JoinWithComma()})");
                }
                if (filter.HookSystemIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ahs.[HookSystemId] IN ({filter.HookSystemIds.JoinWithComma()})");
                }
                if (filter.RetryCountFrom.HasValue)
                {
                    sb.AppendLine($" AND att.[RetryCount] >= @{nameof(filter.RetryCountFrom)}");
                }
                if (filter.RetryCountTo.HasValue)
                {
                    sb.AppendLine($" AND att.[RetryCount] <= @{nameof(filter.RetryCountTo)}");
                }
                if (filter.IsSent.HasValue)
                {
                    sb.AppendLine($" AND att.[IsSent] = @{nameof(filter.IsSent)}");
                }
            }

            return sb.ToString();

        }

        #endregion

        public List<DtoAttendanceHookSystem> Search(PagingData<AttendanceHookSystemFilter, AttendanceHookSystemSortEnumeration> searchInfo)
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
                    return connection.Query<DtoAttendanceHookSystem>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoAttendanceHookSystem>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

        //public void Insert(List<DtoAttendanceHookSystem> entities)
        //{
        //	using (var connection = GetConnection())
        //	{
        //		connection.Execute(InsertCommand, entities
        //			, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
        //	}
        //}

        public void MarkAsSent(long attendanceId, int hookSystemId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(MarkAsSentCommand, new
                {
                    CurrentTime = DateTime.Now,
                    AttendanceId = attendanceId,
                    HookSystemId = hookSystemId
                }, commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void IncreaseRetryCount(long attendanceId, int hookSystemId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(IncreaseRetryCountCommand, new
                {
                    AttendanceId = attendanceId,
                    HookSystemId = hookSystemId
                }
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

    }
}

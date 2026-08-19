using System;
using System.Collections.Generic;
using System.Data;
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

    public interface IAttendanceHookDefinitionRepository
    {
        List<DtoAttendanceHookDefinition> Search(PagingData<AttendanceHookDefinitionFilter, AttendanceHookDefinitionSortEnumeration> searchInfo);

        void MarkAsSent(Guid attendanceId, Guid hookSystemId);

        void IncreaseRetryCount(Guid attendanceId, Guid hookSystemId);
    }


    public class AttendanceHookDefinitionRepository : BaseRepository, IAttendanceHookDefinitionRepository
    {

        public AttendanceHookDefinitionRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<AttendanceHookDefinitionSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<AttendanceHookDefinitionSortEnumeration, string>()
            {
                { AttendanceHookDefinitionSortEnumeration.AttendanceId, "ahd.[AttendanceId]" },
                { AttendanceHookDefinitionSortEnumeration.HookDefinitionId, "ahd.[HookDefinitionId]" }
            };


        private const string SelectCommand =
            @"	SELECT        
					  ahd.*
	            FROM [com].[AttendanceHookDefinition] AS ahd
				WHERE	1 = 1            
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
				   tmp.*
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, ahd.*
				            FROM [com].[AttendanceHookDefinition] AS ahd
				            WHERE	1 = 1            
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string MarkAsSentCommand =
            @"	UPDATE  [com].[AttendanceHookDefinition]
					SET   IsSent = 1
						, SentTime = @CurrentTime
				WHERE  AttendanceId = @AttendanceId
						AND HookDefinitionId = @HookDefinitionId";

        private const string IncreaseRetryCountCommand =
            @"	UPDATE [com].[AttendanceHookDefinition]
					SET   RetryCount = RetryCount + 1
				WHERE  AttendanceId = @AttendanceId
						AND HookDefinitionId = @HookDefinitionId";

        #endregion


        #region Private Methods

        private static string GetSearchClause(AttendanceHookDefinitionFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.AttendanceIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ahd.[AttendanceId] IN ({filter.AttendanceIds.JoinWithComma()})");
                }
                if (filter.HookDefinitionIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ahd.[HookDefinitionId] IN ({filter.HookDefinitionIds.JoinWithComma()})");
                }
                if (filter.RetryCountFrom.HasValue)
                {
                    sb.AppendLine($" AND ahd.[RetryCount] >= @{nameof(filter.RetryCountFrom)}");
                }
                if (filter.RetryCountTo.HasValue)
                {
                    sb.AppendLine($" AND ahd.[RetryCount] <= @{nameof(filter.RetryCountTo)}");
                }
                if (filter.IsSent.HasValue)
                {
                    sb.AppendLine($" AND ahd.[IsSent] = @{nameof(filter.IsSent)}");
                }
            }

            return sb.ToString();

        }

        #endregion

        public List<DtoAttendanceHookDefinition> Search
            (PagingData<AttendanceHookDefinitionFilter, AttendanceHookDefinitionSortEnumeration> searchInfo)
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
                    return connection.Query<DtoAttendanceHookDefinition>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoAttendanceHookDefinition>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }


        public void MarkAsSent(Guid attendanceId, Guid hookSystemId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(MarkAsSentCommand, new
                {
                    CurrentTime = DateTime.Now.ToUniversalTime(),
                    AttendanceId = attendanceId,
                    HookDefinitionId = hookSystemId
                }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
            }
        }

        public void IncreaseRetryCount(Guid attendanceId, Guid hookSystemId)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(IncreaseRetryCountCommand, new
                {
                    AttendanceId = attendanceId,
                    HookDefinitionId = hookSystemId
                }
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
            }
        }

    }
}

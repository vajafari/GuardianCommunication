using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
    public interface IScheduledApiCallTaskRepository
    {
        void Insert(DtoScheduledApiCallTask entity);

        void Update(DtoScheduledApiCallTask entity);

        void Delete(int id);

        List<DtoScheduledApiCallTask> Search(PagingData<ScheduledApiCallTaskFilter, ScheduledApiCallTaskSortEnumeration> searchInfo);
    }


    public class ScheduledApiCallTaskRepository : BaseRepository, IScheduledApiCallTaskRepository
    {

        public ScheduledApiCallTaskRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }


        #region Command Strings

        private static readonly Dictionary<ScheduledApiCallTaskSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<ScheduledApiCallTaskSortEnumeration, string>()
            {
                { ScheduledApiCallTaskSortEnumeration.Id, "st.[Id]" },
            };


        private const string SelectCommand =
            @"	SELECT        
					  st.[Id] AS Id
					, st.[ApiName] AS ApiName
					, st.[Cron] AS Cron
					, st.[AuthorizationUsername] AS AuthorizationUsername
					, st.[AuthorizationPassword] AS AuthorizationPassword
					, st.[AuthorizationType] AS AuthorizationType
					, st.[AuthorizationToken] AS AuthorizationToken
					, st.[EndPointUrl] AS EndPointUrl
					, st.[HttpMethod] AS HttpMethod
					, st.[QueryString] AS QueryString
					, st.[Header] AS Header
					, st.[Body] AS Body
					, st.[DateFormat] AS DateFormat
					, st.[RequestTimeoutInSeconds] AS RequestTimeoutInSeconds
	            FROM [dbo].[ScheduledApiCallTask] st
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.ApiName
					 , tmp.Cron
					 , tmp.AuthorizationUsername
					 , tmp.AuthorizationPassword
					 , tmp.AuthorizationType
					 , tmp.AuthorizationToken
					 , tmp.EndPointUrl
					 , tmp.HttpMethod
					 , tmp.QueryString
					 , tmp.Header
					 , tmp.Body
					 , tmp.DateFormat
					 , tmp.RequestTimeoutInSeconds
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber  
								, st.[Id] AS Id
								, st.[ApiName] AS ApiName
								, st.[Cron] AS Cron
								, st.[AuthorizationUsername] AS AuthorizationUsername
								, st.[AuthorizationPassword] AS AuthorizationPassword
								, st.[AuthorizationType] AS AuthorizationType
								, st.[AuthorizationToken] AS AuthorizationToken
								, st.[EndPointUrl] AS EndPointUrl
								, st.[HttpMethod] AS HttpMethod
								, st.[QueryString] AS QueryString
								, st.[Header] AS Header
								, st.[Body] AS Body
								, st.[DateFormat] AS DateFormat
								, st.[RequestTimeoutInSeconds] AS RequestTimeoutInSeconds
				            FROM [dbo].[ScheduledApiCallTask] st
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";

        private const string InsertCommand =
            @"	INSERT INTO         [dbo].[ScheduledApiCallTask]
				(
					  [ApiName]
					, [Cron]
					, [AuthorizationUsername]
					, [AuthorizationPassword]
					, [AuthorizationType]
					, [AuthorizationToken]
					, [EndPointUrl]
					, [HttpMethod]
					, [QueryString]
					, [Header]
					, [Body]
					, [DateFormat]
					, [RequestTimeoutInSeconds]
				)
				VALUES
				(
					  @ApiName
					, @Cron
					, @AuthorizationUsername
					, @AuthorizationPassword
					, @AuthorizationType
					, @AuthorizationToken
					, @EndPointUrl
					, @HttpMethod
					, @QueryString
					, @Header
					, @Body
					, @DateFormat
					, @RequestTimeoutInSeconds
				) ;
				SELECT SCOPE_IDENTITY();
			";

        private const string UpdateCommand =
            @"	UPDATE  [dbo].[ScheduledApiCallTask]
				SET 
					  [ApiName] = @ApiName
					, [Cron] = @Cron
					, [AuthorizationUsername] = @AuthorizationUsername
					, [AuthorizationPassword] = @AuthorizationPassword
					, [AuthorizationType] = @AuthorizationType
					, [AuthorizationToken] = @AuthorizationToken
					, [EndPointUrl] = @EndPointUrl
					, [HttpMethod] = @HttpMethod
					, [QueryString] = @QueryString
					, [Header] = @Header
					, [Body] = @Body
					, [DateFormat] = @DateFormat
					, [RequestTimeoutInSeconds] = @RequestTimeoutInSeconds
                WHERE [Id] = @Id
			";

        private const string DeleteCommand =
            @"	DELETE FROM  [dbo].[ScheduledApiCallTask]
                WHERE [Id] = @Id
			";

        #endregion


        #region Private Methods


        private static string GetSearchClause(ScheduledApiCallTaskFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND st.[Id] IN ({filter.Ids.JoinWithComma()})");
                }
            }

            return sb.ToString();

        }


        #endregion

        public void Insert(DtoScheduledApiCallTask entity)
        {
            using (var connection = GetConnection())
            {
                entity.Id = connection.ExecuteScalar<int>(InsertCommand, entity
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                //entity.Id = connection.LastInsertRowId;
            }
        }

        public void Update(DtoScheduledApiCallTask entity)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(UpdateCommand, entity
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                //entity.Id = connection.LastInsertRowId;
            }
        }

        public void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.ExecuteScalar<long>(DeleteCommand, new { Id = id }
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
                //entity.Id = connection.LastInsertRowId;
            }
        }

        public List<DtoScheduledApiCallTask> Search(PagingData<ScheduledApiCallTaskFilter, ScheduledApiCallTaskSortEnumeration> searchInfo)
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
                    return connection.Query<DtoScheduledApiCallTask>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoScheduledApiCallTask>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

    }

}

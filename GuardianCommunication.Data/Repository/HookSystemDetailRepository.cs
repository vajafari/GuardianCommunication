using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{

    public interface IHookSystemDetailRepository
    {
        List<DtoHookSystemDetail> Search(PagingData<HookSystemDetailFilter, HookSystemDetailSortEnumeration> searchInfo);

        void Insert(DtoHookSystemDetail entity);

        void Update(DtoHookSystemDetail entity);

    }


    public class HookSystemDetailRepository : BaseRepository, IHookSystemDetailRepository
    {

        public HookSystemDetailRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<HookSystemDetailSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<HookSystemDetailSortEnumeration, string>()
            {
                {HookSystemDetailSortEnumeration.Id, "hsd.[Id]"},
                {HookSystemDetailSortEnumeration.HookSystemId, "hsd.[HookSystemId]"}
            };


        private const string SelectCommand =
            @"	SELECT        
					  hsd.[Id] AS  Id
					, hsd.[HookSystemId] AS  HookSystemId
					, hsd.[ApplicationId] AS  ApplicationId
					, hsd.[DetailType] AS  DetailType
					, hsd.[EndPointUrl] AS  EndPointUrl
					, hsd.[HttpMethod] AS  HttpMethod
					, hsd.[QueryStringTemplate] AS  QueryStringTemplate
					, hsd.[HeaderTemplate] AS  HeaderTemplate
					, hsd.[BodyTemplate] AS  BodyTemplate
					, hsd.[DateFormat] AS  DateFormat
					, hsd.[RetryCount] AS  RetryCount
					, hsd.[RequestTimeoutInSeconds] AS  RequestTimeoutInSeconds
					, hsd.[ResponseResultJsonPath] AS  ResponseResultJsonPath
					, hsd.[IsActive] AS  IsActive
				FROM [HookSystemDetail] hsd
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.HookSystemId
					 , tmp.ApplicationId
					 , tmp.DetailType
					 , tmp.EndPointUrl
					 , tmp.HttpMethod
					 , tmp.QueryStringTemplate
					 , tmp.HeaderTemplate
					 , tmp.BodyTemplate
					 , tmp.DateFormat
					 , tmp.RetryCount
					 , tmp.RequestTimeoutInSeconds
					 , tmp.ResponseResultJsonPath
					 , tmp.IsActive
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber      
								, hsd.[Id] AS  Id
								, hsd.[HookSystemId] AS  HookSystemId
								, hsd.[ApplicationId] AS  ApplicationId
								, hsd.[DetailType] AS  DetailType
								, hsd.[EndPointUrl] AS  EndPointUrl
								, hsd.[HttpMethod] AS  HttpMethod
								, hsd.[QueryStringTemplate] AS  QueryStringTemplate
								, hsd.[HeaderTemplate] AS  HeaderTemplate
								, hsd.[BodyTemplate] AS  BodyTemplate
								, hsd.[DateFormat] AS  DateFormat
								, hsd.[RetryCount] AS  RetryCount
								, hsd.[RequestTimeoutInSeconds] AS  RequestTimeoutInSeconds
								, hsd.[ResponseResultJsonPath] AS  ResponseResultJsonPath
								, hsd.[IsActive] AS  IsActive
							FROM [HookSystemDetail] hsd
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";


        private const string InsertCommand =
            @"	INSERT INTO         [Attendance]
				(
					  [HookSystemId]
					, [ApplicationId]
					, [DetailType]
					, [EndPointUrl]
					, [HttpMethod]
					, [QueryStringTemplate]
					, [HeaderTemplate]
					, [BodyTemplate]
					, [ProxyUrl]
					, [UseProxy]
					, [DateFormat]
					, [RetryCount]
					, [RequestTimeoutInSeconds]
					, [ResponseResultJsonPath]
					, [IsActive]
				)
				VALUES
				(
					  @HookSystemId
					, @ApplicationId
					, @DetailType
					, @EndPointUrl
					, @HttpMethod
					, @QueryStringTemplate
					, @HeaderTemplate
					, @BodyTemplate
					, @ProxyUrl
					, @UseProxy
					, @DateFormat
					, @RetryCount
					, @RequestTimeoutInSeconds
					, @ResponseResultJsonPath
					, @IsActive
				) ;
				SELECT SCOPE_IDENTITY();
			";

        private const string UpdateCommand =
            @"	UPDATE		[HookSystemDetail]
					SET   [ApplicationId] = @ApplicationId
					    , [DetailType] = @DetailType
					    , [EndPointUrl] = @EndPointUrl
					    , [HttpMethod] = @HttpMethod
					    , [QueryStringTemplate] = @QueryStringTemplate
					    , [HeaderTemplate] = @HeaderTemplate
					    , [BodyTemplate] = @BodyTemplate
					    , [ProxyUrl] = @ProxyUrl
					    , [UseProxy] = @UseProxy
					    , [DateFormat] = @DateFormat
					    , [RetryCount] = @RetryCount
					    , [IsActive] = @IsActive
				WHERE  Id = @Id";


        #endregion


        #region Private Methods

        private static string GetSearchClause(HookSystemDetailFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hsd.[Id] IN ({filter.Ids.JoinWithComma()})");
                }
                if (filter.HookSystemIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hsd.[HookSystemId] IN ({filter.HookSystemIds.JoinWithComma()})");
                }
                if (filter.IsActive.HasValue)
                {
                    sb.AppendLine($" AND hsd.[IsActive] = @{nameof(filter.IsActive)}");
                }
                if (filter.DetailTypes.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hsd.[DetailType] IN @{nameof(filter.DetailTypesValues)} ");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoHookSystemDetail> Search(PagingData<HookSystemDetailFilter, HookSystemDetailSortEnumeration> searchInfo)
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
                    return connection.Query<DtoHookSystemDetail>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoHookSystemDetail>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout)).AsList();
            }
        }

        public void Insert(DtoHookSystemDetail entity)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(InsertCommand, entity
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }

        public void Update(DtoHookSystemDetail entity)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(UpdateCommand, entity
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.Timeout);
            }
        }



    }
}

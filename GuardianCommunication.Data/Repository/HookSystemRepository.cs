using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace GuardianCommunication.Data.Repository
{

    public interface IHookSystemRepository
    {
        List<DtoHookSystem> Search(PagingData<HookSystemFilter, HookSystemSortEnumeration> searchInfo);

        void Insert(DtoHookSystem entity);

        void Update(DtoHookSystem entity);

    }


    public class HookSystemRepository : BaseRepository, IHookSystemRepository
    {

        public HookSystemRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<HookSystemSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<HookSystemSortEnumeration, string>()
            {
                { HookSystemSortEnumeration.Id, "hs.[Id]" },
                { HookSystemSortEnumeration.SystemName, "hs.[SystemName]" }
            };


        private const string SelectCommand =
            @"	SELECT        
					  hs.[Id] AS  Id
					, hs.[SystemName] AS  SystemName
					, hs.[AuthorizationUsername] AS  AuthorizationUsername
					, hs.[AuthorizationPassword] AS  AuthorizationPassword
					, hs.[AuthorizationType] AS  AuthorizationType
					, hs.[AuthorizationToken] AS  AuthorizationToken
					, hs.[UseProxy] AS  UseProxy
					, hs.[ProxyUrl] AS  ProxyUrl
					, hs.[ProxyUsername] AS  ProxyUsername
					, hs.[ProxyPassword] AS  ProxyPassword
					, hs.[BypassProxyOnLocal] AS BypassProxyOnLocal 
					, hs.[UseDefaultCredentials] AS  UseDefaultCredentials
				FROM [HookSystem] hs
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.Id
					 , tmp.SystemName
					 , tmp.AuthorizationUsername
					 , tmp.AuthorizationPassword
					 , tmp.AuthorizationType
					 , tmp.AuthorizationToken
					 , tmp.UseProxy
					 , tmp.ProxyUrl
					 , tmp.ProxyUsername
					 , tmp.ProxyPassword
					 , tmp.BypassProxyOnLocal
					 , tmp.UseDefaultCredentials
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber     
								, hs.[Id] AS  Id
								, hs.[SystemName] AS  SystemName
								, hs.[AuthorizationUsername] AS  AuthorizationUsername
								, hs.[AuthorizationPassword] AS  AuthorizationPassword
								, hs.[AuthorizationType] AS  AuthorizationType
								, hs.[AuthorizationToken] AS  AuthorizationToken
								, hs.[UseProxy] AS  UseProxy
								, hs.[ProxyUrl] AS  ProxyUrl
								, hs.[ProxyUsername] AS  ProxyUsername
								, hs.[ProxyPassword] AS  ProxyPassword
								, hs.[BypassProxyOnLocal] AS BypassProxyOnLocal 
								, hs.[UseDefaultCredentials] AS  UseDefaultCredentials
							FROM [HookSystem] hs
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";


        private const string InsertCommand =
            @"	INSERT INTO         [Attendance]
				(
					  [SystemName]
					, [AuthorizationUsername]
					, [AuthorizationPassword]
					, [AuthorizationType]
					, [AuthorizationToken]
					, [UseProxy]
					, [ProxyUrl]
					, [ProxyUsername]
					, [ProxyPassword]
					, [BypassProxyOnLocal]
					, [UseDefaultCredentials]
				)
				VALUES
				(
					  @SystemName
					, @AuthorizationUsername
					, @AuthorizationPassword
					, @AuthorizationType
					, @AuthorizationToken
					, @UseProxy
					, @ProxyUrl
					, @ProxyUsername
					, @ProxyPassword
					, @BypassProxyOnLocal
					, @UseDefaultCredentials
				) ;
				SELECT SCOPE_IDENTITY();
			";


        private const string UpdateCommand =
            @"	UPDATE		[HookSystemDetail]
					SET   [SystemName] = @SystemName
					    , [AuthorizationUsername] = @AuthorizationUsername
					    , [AuthorizationPassword] = @AuthorizationPassword
					    , [AuthorizationType] = @AuthorizationType
					    , [AuthorizationToken] = @AuthorizationToken
					    , [UseProxy] = @UseProxy
					    , [ProxyUrl] = @ProxyUrl
					    , [ProxyUsername] = @ProxyUsername
					    , [ProxyPassword] = @ProxyPassword
					    , [BypassProxyOnLocal] = @BypassProxyOnLocal
					    , [UseDefaultCredentials] = @UseDefaultCredentials
				WHERE  Id == @Id";

        #endregion


        #region Private Methods

        private static string GetSearchClause(HookSystemFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hs.[Id] IN ({filter.Ids.JoinWithComma()})");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoHookSystem> Search(PagingData<HookSystemFilter, HookSystemSortEnumeration> searchInfo)
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
                    return connection.Query<DtoHookSystem>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoHookSystem>(commandText,
                    commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout)).AsList();
            }
        }

        public void Insert(DtoHookSystem entity)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(InsertCommand, entity
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

        public void Update(DtoHookSystem entity)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(UpdateCommand, entity
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);
            }
        }

    }
}

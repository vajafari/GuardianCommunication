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

    public interface IHookDefinitionRepository
    {
        List<DtoHookDefinition> Search(PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration> searchInfo);
    }


    public class HookDefinitionRepository : BaseRepository, IHookDefinitionRepository
    {

        public HookDefinitionRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<HookDefinitionSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<HookDefinitionSortEnumeration, string>()
            {
                {HookDefinitionSortEnumeration.Id, "hd.[Id]"},
            };


        private const string SelectCommand =
            @"	SELECT        
					  hd.*
				FROM [com].[HookDefinition] hd
				WHERE  1 = 1                
						{0}    -- Search            
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT         
					   tmp.*
				 FROM            
				        (            
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber      
								, hd.*
							FROM [com].[HookDefinition] hd
							WHERE  1 = 1         
									{0}    -- Search            
				         ) tmp                        
				 {2}    -- Paging";



        #endregion


        #region Private Methods

        private static string GetSearchClause(HookDefinitionFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hd.[Id] IN ({filter.Ids.JoinWithComma()})");
                }
               
                if (filter.IsActive.HasValue)
                {
                    sb.AppendLine($" AND hd.[IsActive] = @{nameof(filter.IsActive)}");
                }
                if (filter.HookTypes.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND hd.[DefinitionType] IN @{nameof(filter.HookTypesValues)} ");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoHookDefinition> Search(PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration> searchInfo)
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
                    return connection.Query<DtoHookDefinition>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoHookDefinition>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }

    }
}

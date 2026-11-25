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

    public interface IHookDefinitionRepository
    {
        List<DtoHookDefinition> Search(PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration> searchInfo);

        void UpdateLastRunAt(Guid id, DateTime lastRunAt);
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

        private const string UpdateLastRunAtCommand =
            @"	UPDATE [com].[HookDefinition]
					SET [LastRunAt] = @LastRunAt
				WHERE [Id] = @Id";



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
                    sb.AppendLine($" AND hd.[HookType] IN @{nameof(filter.HookTypesValues)} ");
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
                List<DtoHookDefinition> result;
                if (searchInfo != null)
                {
                    var whereClause = GetSearchClause(searchInfo.Filter);
                    var orderByClause = searchInfo.GetNormalSortString(MapSortEnumToFieldName);
                    var pagingClause = searchInfo.GetRowNumberClause("tmp", ServiceConstants.RowNumberColumnName);
                    var searchType = searchInfo.GetSearchType();
                    commandText = searchType == SearchTypeEnumeration.SimpleSearch
                        ? SelectCommand.FormatInvariantCulture(whereClause, orderByClause)
                        : SelectWithPagingCommand.FormatInvariantCulture(whereClause, orderByClause, pagingClause);
                    result = connection.Query<DtoHookDefinition>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }
                else
                {
                    commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                    result = (connection.Query<DtoHookDefinition>(commandText,
                        commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
                }

                // LastRunAt در دیتابیس UTC است و بیرون از این لایه به وقت محلی
                foreach (var item in result.Where(item => item.LastRunAt.HasValue))
                {
                    item.LastRunAt = DateTime.SpecifyKind(item.LastRunAt.Value, DateTimeKind.Utc).ToLocalTime();
                }
                return result;
            }
        }

        /// <summary>
        /// ثبت زمان شروع آخرین اجرای یک ردیف زمان بند (به وقت محلی؛ UTC ذخیره می شود)
        /// </summary>
        public void UpdateLastRunAt(Guid id, DateTime lastRunAt)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(UpdateLastRunAtCommand, new
                {
                    Id = id,
                    LastRunAt = lastRunAt.ToUniversalTime(),
                }, commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout);
            }
        }

    }
}

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

    public interface IDeviceDoorRepository
    {
        List<DtoDeviceDoor> Search(PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration> searchInfo);
    }


    public class DeviceDoorRepository : BaseRepository, IDeviceDoorRepository
    {

        public DeviceDoorRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private static readonly Dictionary<DeviceDoorSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<DeviceDoorSortEnumeration, string>()
            {
                { DeviceDoorSortEnumeration.Id, "ddb.[Id]" },
                { DeviceDoorSortEnumeration.DoorNumber, "ddb.[DoorNumber]" },
            };


        private const string SelectCommand =
            @"	SELECT
					  ddb.*
				FROM [core].[DeviceDoorBase] ddb
				WHERE  1 = 1
						{0}    -- Search
				{1}    -- Order By";

        private const string SelectWithPagingCommand =
            @"	SELECT
					   tmp.*
				 FROM
				        (
				            SELECT    ROW_NUMBER() OVER ({1}) AS  RowNumber
								, ddb.*
				            FROM [core].[DeviceDoorBase] ddb
							WHERE  1 = 1
									{0}    -- Search
				         ) tmp
				 {2}    -- Paging";

        #endregion


        #region Private Methods

        private static string GetSearchClause(DeviceDoorFilter filter)
        {
            var sb = new StringBuilder();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ddb.[Id] IN @{nameof(filter.Ids)}");
                }
                if (filter.DeviceIds.IsCollectionNotNullOrEmpty())
                {
                    sb.AppendLine($" AND ddb.[DeviceId] IN @{nameof(filter.DeviceIds)}");
                }
                if (filter.IsActive.HasValue)
                {
                    sb.AppendLine($" AND ddb.[IsActive] = @{nameof(filter.IsActive)}");
                }
            }

            return sb.ToString();

        }

        #endregion


        public List<DtoDeviceDoor> Search(PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration> searchInfo)
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
                    return connection.Query<DtoDeviceDoor>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectCommand.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceDoor>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }

    }
}

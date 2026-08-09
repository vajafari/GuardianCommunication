using Dapper;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GuardianCommunication.Data.Repository
{

    public interface IDeviceRepository
    {
        Task<List<DtoDevice>> SearchFullInfoAsync(PagingData<DeviceFilter, DeviceSortEnumeration> searchInfo);
    }


    public class DeviceRepository : BaseRepository, IDeviceRepository
    {

        public DeviceRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings Device

        private static readonly Dictionary<DeviceSortEnumeration, string> MapSortEnumToFieldName =
            new Dictionary<DeviceSortEnumeration, string>
            {
            { DeviceSortEnumeration.DeviceNumber, "d.[DeviceNumber]" },
            };

        private const string SelectFullInfo =
            @"
        {2} -- With
        SELECT
              d.*
            , loc.[LocationNumber]    AS LocationNumber
            , dt.[ProducerNumber]     AS ProducerNumber
            , dt.[SdkVersion]         AS SdkVersion
            , dt.[DeviceTypeCode]     AS DeviceTypeCode
            , dt.[EnrollStandard]     AS EnrollStandard
            , dt.[HasFingerPrint]     AS HasFingerPrint
            , dt.[HasFace]            AS HasFace
            , dt.[HasVisiblelight]    AS HasVisiblelight
            , dt.[HasRfReader]        AS HasRfReader
            , dt.[HasPalm]            AS HasPalm
            , dt.[HasIris]            AS HasIris
        FROM [core].[Device] AS d
            LEFT JOIN [core].[Location] AS loc ON loc.[Id] = d.[LocationId]
            LEFT JOIN [core].[DeviceType] AS dt ON dt.[Id] = d.[DeviceTypeId]
            {3} -- Joins
        WHERE  1 = 1
                {0}    -- Search
        {1}    -- Order By
        ";

        private const string SelectFullInfoWithPaging = @"
        {3} -- With
        SELECT tmp.*
        FROM
        (
            SELECT    ROW_NUMBER() OVER ({1}) AS RowNumber
                  , d.*
                  , loc.[LocationNumber]    AS LocationNumber
                  , dt.[DeviceTypeNumber]   AS DeviceTypeNumber
                  , dt.[ProducerNumber]     AS ProducerNumber
                  , dt.[SdkVersion]         AS SdkVersion
                  , dt.[DeviceTypeCode]     AS DeviceTypeCode
                  , dt.[EnrollStandard]     AS EnrollStandard
                  , dt.[HasFingerPrint]     AS HasFingerPrint
                  , dt.[HasFace]            AS HasFace
                  , dt.[HasVisiblelight]    AS HasVisiblelight
                  , dt.[HasRfReader]        AS HasRfReader
                  , dt.[HasPalm]            AS HasPalm
                  , dt.[HasIris]            AS HasIris
            FROM [core].[Device] AS d
                LEFT JOIN [core].[Location] AS loc ON loc.[Id] = d.[LocationId]
                LEFT JOIN [core].[DeviceType] AS dt ON dt.[Id] = d.[DeviceTypeId]
                {4} -- Joins
            WHERE  1 = 1
                    {0}    -- Search
        ) tmp
        {2}    -- Paging
        ";

        #endregion


        #region Private Methods

        private static SearchClausePart GetSearchClause(DeviceFilter filter)
        {
            var result = new SearchClausePart();
            if (filter != null)
            {
                if (filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.Ids, result, "d.[Id]", "DeviceIds", "UNIQUEIDENTIFIER", false);
                }
                if (filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.DeviceNumbers, result, "d.[DeviceNumber]", "DeviceNumbers", "INT", true);
                }
                if (filter.LocationIds.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.LocationIds, result, "d.[LocationId]", "LocationIds", "UNIQUEIDENTIFIER", false);
                }
                if (filter.SerialNumbers.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.SerialNumbers, result, "d.[SerialNumber]", "SerialNumbers", "NVARCHAR(200)", false);
                }
                if (filter.DeviceTypeIds.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.DeviceTypeIds, result, "d.[DeviceTypeId]", "DeviceTypeIds", "UNIQUEIDENTIFIER", false);
                }
                if (filter.IoTypeValues.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.IoTypeValues, result, "d.[IoType]", "IoTypeValues", "SMALLINT", true);
                }
                if (filter.ProducerValues.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.ProducerValues, result, "dt.[ProducerNumber]", "ProducerValues", "SMALLINT", true);
                }
                if (filter.SdkVersionValues.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.SdkVersionValues, result, "dt.[SdkVersion]", "SdkVersionValues", "SMALLINT", true);
                }
                if (filter.ConnectionModes.IsCollectionNotNullOrEmpty())
                {
                    DatabaseHelper.ApplyInClause(filter.ConnectionModes, result, "d.[ConnectionMode]", "ConnectionModes", "SMALLINT", true);
                }
                if (filter.IsActive.HasValue)
                {
                    result.Parameters.Add($"{nameof(filter.IsActive)}", filter.IsActive);
                    result.WhereClauseStringBuilder.AppendLine($"d.[IsActive] = @{nameof(filter.IsActive)}");
                }
            }
            return result;
        }

        #endregion


        public async Task<List<DtoDevice>> SearchFullInfoAsync(PagingData<DeviceFilter, DeviceSortEnumeration> searchInfo)
        {
            using (var connection = GetLogConnection())
            {
                string commandText;

                if (searchInfo != null)
                {
                    var searchParts = GetSearchClause(searchInfo.Filter);
                    var whereClause = searchParts.WhereClauseStringBuilder.ToString();
                    var orderByClause = searchInfo.GetNormalSortString(MapSortEnumToFieldName);
                    var pagingClause = searchInfo.GetRowNumberClause("tmp", ServiceConstants.RowNumberColumnName);
                    var searchType = searchInfo.GetSearchType();
                    commandText = searchType == SearchTypeEnumeration.SimpleSearch
                        ? SelectFullInfo.FormatInvariantCulture(whereClause, orderByClause,
                            searchParts.PrevCommandClause, searchParts.JoinClauseStringBuilder.ToString())
                        : SelectFullInfoWithPaging.FormatInvariantCulture(whereClause, orderByClause, pagingClause,
                            searchParts.PrevCommandClause, searchParts.JoinClauseStringBuilder.ToString());
                    return (await connection.QueryAsync<DtoDevice>(commandText
                        , searchParts.Parameters
                        , commandType: CommandType.Text
                        , commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
                }

                commandText =
                    SelectFullInfo.FormatInvariantCulture(string.Empty, string.Empty, string.Empty, string.Empty);
                return (await connection.QueryAsync<DtoDevice>(commandText
                    , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }


        }

    }

}

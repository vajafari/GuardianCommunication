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
        List<DtoDeviceDoorFullInfo> Search(PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration> searchInfo);
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


        // Reader XOR resolution: exactly one of ReaderDeviceId / ReaderCameraId is populated,
        // so COALESCE across the two optional joins always yields the effective reader value.
        private const string SelectFullInfo = 
            @"
            SELECT
                  ddb.*
                , d.[Title]                                  AS DeviceTitle
                , dt.[Title]                                  AS DeviceTypeTitle
                , dt.[ProducerNumber]                          AS ProducerNumber
                , dt.[SdkVersion]                              AS SdkVersion
                , d.[ModuleId]                                 AS ModuleId
                , d.[LocationId]                                AS LocationId
                , loc.[Title]                                   AS LocationTitle
                , COALESCE(rd.[ModuleId], rc.[ModuleId])       AS ReaderModuleId
                , COALESCE(rd.[IoType], rc.[IoType])           AS ReaderIoType
                , COALESCE(rd.[LocationId], rc.[LocationId])   AS ReaderLocationId
                , rloc.[Title]                                  AS ReaderLocationTitle
                , rd.[Title]                                    AS ReaderDeviceTitle
                , rdt.[Title]                                   AS ReaderDeviceTypeTitle
                , rdt.[ProducerNumber]                          AS ReaderProducerNumber
                , rdt.[SdkVersion]                              AS ReaderSdkVersion
                , rc.[Title]                                    AS ReaderCameraTitle
            FROM [core].[DeviceDoorBase] AS ddb
                INNER JOIN [core].[Device] AS d ON d.[Id] = ddb.[DeviceId]
                LEFT JOIN [core].[DeviceType] AS dt ON dt.[Id] = d.[DeviceTypeId]
                LEFT JOIN [core].[Location] AS loc ON loc.[Id] = d.[LocationId]
                LEFT JOIN [core].[Device] AS rd ON rd.[Id] = ddb.[ReaderDeviceId]
                LEFT JOIN [core].[DeviceType] AS rdt ON rdt.[Id] = rd.[DeviceTypeId]
                LEFT JOIN [core].[Camera] AS rc ON rc.[Id] = ddb.[ReaderCameraId]
                LEFT JOIN [core].[Location] AS rloc ON rloc.[Id] = COALESCE(rd.[LocationId], rc.[LocationId])
            WHERE  1 = 1
                    {0}    -- Search
            {1}    -- Order By
            ";

        private const string SelectFullInfoWithPaging =
            @"
            SELECT tmp.*
            FROM
            (
                SELECT    ROW_NUMBER() OVER ({1}) AS RowNumber
                      , ddb.*
                      , d.[Title]                                  AS DeviceTitle
                      , dt.[Title]                                  AS DeviceTypeTitle
                      , dt.[ProducerNumber]                          AS ProducerNumber
                      , dt.[SdkVersion]                              AS SdkVersion
                      , d.[ModuleId]                                 AS ModuleId
                      , d.[LocationId]                                AS LocationId
                      , loc.[Title]                                   AS LocationTitle
                      , COALESCE(rd.[ModuleId], rc.[ModuleId])       AS ReaderModuleId
                      , COALESCE(rd.[IoType], rc.[IoType])           AS ReaderIoType
                      , COALESCE(rd.[LocationId], rc.[LocationId])   AS ReaderLocationId
                      , rloc.[Title]                                  AS ReaderLocationTitle
                      , rd.[Title]                                    AS ReaderDeviceTitle
                      , rdt.[Title]                                   AS ReaderDeviceTypeTitle
                      , rdt.[ProducerNumber]                          AS ReaderProducerNumber
                      , rdt.[SdkVersion]                              AS ReaderSdkVersion
                      , rc.[Title]                                    AS ReaderCameraTitle
                FROM [core].[DeviceDoorBase] AS ddb
                    INNER JOIN [core].[Device] AS d ON d.[Id] = ddb.[DeviceId]
                    LEFT JOIN [core].[DeviceType] AS dt ON dt.[Id] = d.[DeviceTypeId]
                    LEFT JOIN [core].[Location] AS loc ON loc.[Id] = d.[LocationId]
                    LEFT JOIN [core].[Device] AS rd ON rd.[Id] = ddb.[ReaderDeviceId]
                    LEFT JOIN [core].[DeviceType] AS rdt ON rdt.[Id] = rd.[DeviceTypeId]
                    LEFT JOIN [core].[Camera] AS rc ON rc.[Id] = ddb.[ReaderCameraId]
                    LEFT JOIN [core].[Location] AS rloc ON rloc.[Id] = COALESCE(rd.[LocationId], rc.[LocationId])
                WHERE  1 = 1
                        {0}    -- Search
            ) tmp
            {2}    -- Paging
        ";

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


        public List<DtoDeviceDoorFullInfo> Search(PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration> searchInfo)
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
                        ? SelectFullInfo.FormatInvariantCulture(whereClause, orderByClause)
                        : SelectFullInfoWithPaging.FormatInvariantCulture(whereClause, orderByClause, pagingClause);
                    return connection.Query<DtoDeviceDoorFullInfo>(commandText, searchInfo.Filter
                        , commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout).AsList();
                }

                commandText = SelectFullInfo.FormatInvariantCulture(string.Empty, string.Empty);
                return (connection.Query<DtoDeviceDoorFullInfo>(commandText,
                    commandType: CommandType.Text, commandTimeout: ConnectionConfig.CommandTimeout)).AsList();
            }
        }

    }
}

using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardianCommunication.Business.Component
{
    public class DeviceCommandComponent : BaseComponent
    {
        public DeviceCommandComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        public List<DtoDeviceUnsentCommand> GetUnsentCommandsForEachDevice
            (DeviceNotSentCommandsFilter filter)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetUnsentCommandsForEachDevice(filter);
        }

        public List<DtoUnsentCommandCountByDeviceSerialNumber> GetUnsentCommandsCountByDeviceSerialNumberForEachDevice
            (DeviceNotSentCommandsCountByDeviceSerialNumberFilter filter)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetUnsentCommandsCountByDeviceSerialNumber(filter);
        }

        public List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumberForEachDevice
            (DeviceNotSentCommandsCountByDeviceNumberFilter filter)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetUnsentCommandsCountByDeviceNumbers(filter);
        }

        public List<DtoDeviceCommandWithoutContent> SearchWithoutContent
            (PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetDeviceCommandRepository().SearchWithoutContent(searchInfo);
        }

        public List<DtoDeviceCommand> Insert(List<DtoDeviceCommand> entities)
        {
            if (entities.IsCollectionNullOrEmpty())
            {
                return entities;
            }
            return RepositoryFactory.GetDeviceCommandRepository().Insert(entities);
        }

        public DtoDeviceCommand Insert(DtoDeviceCommand entity)
        {
            return entity != null ? Insert(new List<DtoDeviceCommand> { entity }).FirstOrDefault() : null;
        }

        public void DeleteByIds(List<Guid> ids)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteByIds(ids, null);
        }

        public void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers)
        {
            if (commandIdentifiers.IsCollectionNullOrEmpty())
            {
                return;
            }
            RepositoryFactory.GetDeviceCommandRepository().DeleteByCommandIdentifiers(commandIdentifiers);
        }

        public void DeleteNotSentByUserIdOnDeviceAndCommandTypes
            (long userIdOnDevice, Guid deviceId, List<DeviceCommandTypeEnumeration> commandTypes)
        {
            RepositoryFactory.GetDeviceCommandRepository()
                .DeleteNotSentByUserIdOnDeviceAndCommandTypes(userIdOnDevice, deviceId, commandTypes);
        }

        public void DeleteNotSentByUserIdOnDeviceDeviceCommandTypesAndCommandIdentifier
            (long userIdOnDevice, Guid deviceId, List<DeviceCommandTypeEnumeration> commandTypes, List<Guid> commandIds)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSentByUserIdOnDeviceDeviceCommandTypesAndCommandIdentifier
                (userIdOnDevice, deviceId, commandTypes, commandIds);
        }

        public void DeleteNotSentByUserIdOnDeviceCommandTypesAndCommandDateInterval
            (long userIdOnDevice, Guid deviceId, List<DeviceCommandTypeEnumeration> commandTypes, DateTime startDate, DateTime endDate)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSentByUserIdOnDeviceCommandTypesAndCommandDateInterval
                (userIdOnDevice, deviceId, commandTypes, startDate, endDate);
        }


        public void UpdateSendDataByNumericIds(List<long> ids)
        {
            RepositoryFactory.GetDeviceCommandRepository().UpdateSendDataByNumericIds(ids);
        }

        public void SetResponse(DtoDeviceCommandProcessingResult entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            if (systemConfigComponent.GetSystemConfigCache().KeepCommandAfterResponse)
            {
                RepositoryFactory.GetDeviceCommandRepository().SetResponse(entity);
            }
            else
            {
                RepositoryFactory.GetDeviceCommandRepository().DeleteByNumericIds(new List<long> { entity.NumericId }, entity.Mode);
            }
        }

        public void SetDescription(DtoDeviceCommandProcessingDescription entity)
        {
            RepositoryFactory.GetDeviceCommandRepository().SetDescription(entity);
        }

    }
}

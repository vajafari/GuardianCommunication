using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Repository;

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
            (DeviceNotSentCommandsFilter filter)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetUnsentCommandsCountByDeviceSerialNumberForEachDevice(filter);
        }

        public List<DtoUnsentCommandCountByDeviceNumber> GetUnsentCommandsCountByDeviceNumberForEachDevice
            (DeviceNotSentCommandsFilter filter)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetUnsentCommandsCountByDeviceNumberForEachDevice(filter);
        }

        public List<DtoFailedCommandStatistics> GetDeviceNotSendCommandsStatistics(List<int> deviceNumbers)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetNotSendCommandsStatistics(deviceNumbers);
        }

        public List<DtoDeviceCommandWithoutContent> SearchWithoutContent(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetDeviceCommandRepository().SearchWithoutContent(searchInfo);
        }

        public int GetCount(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetDeviceCommandRepository().GetCount(searchInfo);
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

        public void DeleteNotSentByEmployeeDeviceAndCommandTypes
            (long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSentByEmployeeDeviceAndCommandTypes(employeeNumber, deviceNumber, commandTypes);
        }

        public void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier
            (long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes, List<Guid> commandIds)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier
                (employeeNumber, deviceNumber, commandTypes, commandIds);
        }

        public void DeleteNotSentByEmployeeDeviceCommandTypesAndCommandDateInterval
            (long employeeNumber, int deviceNumber, List<DeviceCommandTypeEnumeration> commandTypes, DateTime startDate, DateTime endDate)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSentByEmployeeDeviceCommandTypesAndCommandDateInterval
                (employeeNumber, deviceNumber, commandTypes, startDate, endDate);
        }

        public void DeleteNotSendByDeviceNumbers(List<int> deviceNumbers)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteNotSendByDeviceNumber(deviceNumbers);
        }

        public void DeleteFailedBeforeDate(DateTime dateTime, bool justDeleteFailedCommands)
        {
            RepositoryFactory.GetDeviceCommandRepository().DeleteFailedBeforeDate(dateTime, justDeleteFailedCommands);
        }

        public void DeleteByIds(List<int> ids)
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

        public void ResetSendData(List<int> ids)
        {
            RepositoryFactory.GetDeviceCommandRepository().ResetSendData(ids);
        }

        public void UpdateSendData(List<int> ids)
        {
            RepositoryFactory.GetDeviceCommandRepository().UpdateSendData(ids);
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
                RepositoryFactory.GetDeviceCommandRepository().DeleteByIds(new List<int> { entity.Id }, entity.Mode);
            }
        }

        public void SetDescription(DtoDeviceCommandProcessingDescription entity)
        {
            RepositoryFactory.GetDeviceCommandRepository().SetDescription(entity);
        }

    }
}

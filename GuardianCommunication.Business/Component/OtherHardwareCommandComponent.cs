using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Component
{
    public class OtherHardwareCommandComponent : BaseComponent
    {
        public OtherHardwareCommandComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }

        public DtoOtherHardwareCommand Insert(DtoOtherHardwareCommand entity)
        {
            return Insert(new List<DtoOtherHardwareCommand> { entity }).FirstOrDefault();
        }

        public List<DtoOtherHardwareCommand> Insert(List<DtoOtherHardwareCommand> entities)
        {
            return RepositoryFactory.GetOtherHardwareCommandRepository().Insert(entities);
        }

        public void DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypes(HardwareTypeEnumeration hardwareType,
            long objectId, List<OtherHardwareCommandTypeEnumeration> commandTypes)
        {
            RepositoryFactory.GetOtherHardwareCommandRepository().
                DeleteNotSentByHardwareTypeAndObjectIdAndCommandTypes(hardwareType, objectId, commandTypes);
        }

        public void SetResponse(DtoOtherHardwareCommandProcessingResult entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            if (systemConfigComponent.GetSystemConfigCache().KeepCommandAfterResponse)
            {
                RepositoryFactory.GetOtherHardwareCommandRepository().SetResponse(entity);
            }
            else
            {
                RepositoryFactory.GetOtherHardwareCommandRepository().DeleteByIds(new List<int> { entity.Id });
            }
        }

        public void SetDescription(DtoOtherHardwareCommandProcessingDescription entity)
        {
            RepositoryFactory.GetOtherHardwareCommandRepository().SetDescription(entity);
        }

        public List<DtoUnsentCommandForFaceDetectionSystem> GetUnsentCommandsForFaceDetectionSystem(int count)
        {
            return RepositoryFactory.GetOtherHardwareCommandRepository().GetUnsentCommandsForFaceDetectionSystem(count);
        }

        public void UpdateSendData(List<int> ids)
        {
            RepositoryFactory.GetOtherHardwareCommandRepository().UpdateSendData(ids);
        }

        //public List<DtoZkUnsentCommands> GetZkDeviceUnsentCommands(string deviceSerialNumber, int count)
        //{
        //    return RepositoryFactory.GetDeviceCommandRepository().GetZkUnsentCommands(deviceSerialNumber, count);
        //}

        //public List<DtoFailedCommandStatistics> GetDeviceNotSendCommandsStatistics(List<int> deviceNumbers)
        //{
        //    return RepositoryFactory.GetDeviceCommandRepository().GetNotSendCommandsStatistics(deviceNumbers);
        //}

        //public List<DtoDeviceCommandWithoutContent> SearchWithoutContent(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        //{
        //    return RepositoryFactory.GetDeviceCommandRepository().SearchWithoutContent(searchInfo);
        //}

        //public List<DtoDeviceCommand> Search(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        //{
        //    return RepositoryFactory.GetDeviceCommandRepository().Search(searchInfo);
        //}

        //public int GetCount(PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> searchInfo)
        //{
        //    return RepositoryFactory.GetDeviceCommandRepository().GetCount(searchInfo);
        //}

        //public void DeleteNotSendByDeviceNumbers(List<int> deviceNumbers)
        //{
        //    RepositoryFactory.GetDeviceCommandRepository().DeleteNotSendByDeviceNumber(deviceNumbers);
        //}

        //public void DeleteFailedBeforeDate(DateTime dateTime, bool justDeleteFailedCommands)
        //{
        //    RepositoryFactory.GetDeviceCommandRepository().DeleteFailedBeforeDate(dateTime, justDeleteFailedCommands);
        //}

        //public void DeleteByIds(List<int> ids)
        //{
        //    RepositoryFactory.GetDeviceCommandRepository().DeleteByIds(ids, null);
        //}

        //public void DeleteByCommandIdentifiers(List<Guid> commandIdentifiers)
        //{
        //    if (commandIdentifiers.IsCollectionNullOrEmpty())
        //    {
        //        return;
        //    }
        //    RepositoryFactory.GetDeviceCommandRepository().DeleteByCommandIdentifiers(commandIdentifiers);
        //}

        //public void ResetSendData(List<int> ids)
        //{
        //    RepositoryFactory.GetDeviceCommandRepository().ResetSendData(ids);
        //}

        //public void UpdateSendData(List<int> ids)
        //{
        //    RepositoryFactory.GetDeviceCommandRepository().UpdateSendData(ids);
        //}

    }
}

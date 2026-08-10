using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.CommunicationModels.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.CommunicationModels.FilterAndSearchModels;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public static class CommunicationModelMapper
    {

        public static DtoAttendance MapOtherResourcesAttendanceModelToDtoAttendance(OtherResourcesAttendanceModel inputItems)
        {
            return MapOtherResourcesAttendanceModelToDtoAttendance(new List<OtherResourcesAttendanceModel>() { inputItems }).FirstOrDefault();
        }

        public static List<DtoAttendance> MapOtherResourcesAttendanceModelToDtoAttendance(List<OtherResourcesAttendanceModel> inputItems)
        {
            var result = new List<DtoAttendance>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new DtoAttendance
                {
                    DeviceNumber = attendance.DeviceNumber,
                    ReaderDeviceNumber = attendance.ReaderDeviceNumber,
                    EmployeeNumber = attendance.EmployeeNumber,
                    VerificationStyle = attendance.VerificationStyle,
                    AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
                    AttendanceSource = attendance.AttendanceSource,
                    IsInvalid = false,
                    IsSent = attendance.IsSent,
                    Id = attendance.Id,
                    StatusCode = attendance.StatusCode,
                    ApplicationId = attendance.ApplicationId,
                    IoType = attendance.IoType,
                    RfCardNumber = attendance.RfCardNumber,
                    CameraId = attendance.CameraId,
                    DeviceAttendanceIoRetrieveType = null,
                    DoorId = attendance.DoorId,
                });
            }

            return result;
        }

        public static List<DtoAttendance> MapDeviceAttendanceModelToDtoAttendance(List<DeviceAttendanceModel> inputItems)
        {
            var result = new List<DtoAttendance>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new DtoAttendance
                {
                    DeviceNumber = attendance.DeviceNumber,
                    EmployeeNumber = attendance.EmployeeNumber,
                    VerificationStyle = attendance.VerificationStyle,
                    AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
                    AttendanceSource = attendance.AttendanceSource,
                    IsInvalid = attendance.IsInvalid,
                    IsSent = attendance.IsSent,
                    Id = attendance.Id,
                    StatusCode = attendance.StatusCode,
                    RfCardNumber = attendance.RfCardNumber,
                    IoType = attendance.IoType,
                    ApplicationId = attendance.ApplicationId,

                });
            }

            return result;
        }

        public static List<DeviceAttendanceModel> MapDtoAttendanceToDeviceAttendanceModel(List<DtoAttendance> inputItems)
        {
            var result = new List<DeviceAttendanceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new DeviceAttendanceModel
                {
                    DeviceNumber = attendance.DeviceNumber ?? 0,
                    EmployeeNumber = attendance.EmployeeNumber,
                    VerificationStyle = attendance.VerificationStyle ?? 0,
                    AttendanceDateTime = attendance.AttendanceDateTime.ToNumericDateTime(),
                    AttendanceSource = attendance.AttendanceSource,
                    IsInvalid = attendance.IsInvalid,
                    IsSent = attendance.IsSent,
                    Id = attendance.Id,
                    StatusCode = attendance.StatusCode,
                    RfCardNumber = attendance.RfCardNumber,
                    IoType = attendance.IoType,
                    ApplicationId = attendance.ApplicationId,
                    DoorId = attendance.DoorId,
                });
            }

            return result;
        }

        public static DeviceAttendanceModel MapDtoAttendanceToDeviceAttendanceModel(DtoAttendance inputItem)
        {
            return MapDtoAttendanceToDeviceAttendanceModel(new List<DtoAttendance> { inputItem }).FirstOrDefault();
        }

        public static List<DeviceInvalidAttendanceModel> MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(List<DtoInvalidAttendance> inputItems)
        {
            var result = new List<DeviceInvalidAttendanceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new DeviceInvalidAttendanceModel
                {
                    DeviceNumber = attendance.DeviceNumber ?? 0,
                    EmployeeNumber = attendance.UserIdOnDevice,
                    VerificationStyle = attendance.VerificationStyle ?? 0,
                    AttendanceDateTime = attendance.AttendanceDateTime.ToNumericDateTime(),
                    AttendanceSource = attendance.AttendanceSource,
                    Id = attendance.Id,
                    StatusCode = attendance.StatusCode,
                    RfCardNumber = attendance.RfCardNumber,
                    Reason = attendance.Reason,
                    Image = attendance.Image.IsCollectionNotNullOrEmpty() ? Convert.ToBase64String(attendance.Image) : null,
                    DoorId = attendance.DoorId,
                });
            }

            return result;
        }

        public static DeviceInvalidAttendanceModel MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(DtoInvalidAttendance inputItem)
        {
            return MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(new List<DtoInvalidAttendance> { inputItem }).FirstOrDefault();
        }

        public static ServerMatchDataModel MapServerMatchDataToServerMatchDataModel(DtoServerMatchData inputItem)
        {
            return new ServerMatchDataModel
            {
                Password = inputItem.Password,
                RfCardNumber = inputItem.RfCardNumber,
                DeviceNumber = inputItem.DeviceId,
                MatchType = inputItem.MatchType,
                EventDateTime = inputItem.EventDateTime.ToNumericDateTime(),
                TemplateData = inputItem.TemplateData != null ? Convert.ToBase64String(inputItem.TemplateData) : null,
                UserId = inputItem.UserIdOnDevice,
                TemplateType = inputItem.TemplateType,
                DoorId = inputItem.DoorId,
            };
        }

        public static List<ZkOperationLogModel> MapDtoZkOperationLogToZkOperationLogModel(List<DtoZkOperationLog> inputItems)
        {
            var result = new List<ZkOperationLogModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<ZkOperationLogModel>();
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new ZkOperationLogModel
                {
                    DeviceNumber = attendance.DeviceNumber,
                    User = attendance.User,
                    Object1 = attendance.Object1,
                    Object2 = attendance.Object2,
                    Object3 = attendance.Object3,
                    Object4 = attendance.Object4,
                    OperationTime = attendance.OperationTime.ToNumericDateTime(),
                    Id = attendance.Id,
                    OperationType = attendance.OperationType,
                    Operator = attendance.Operator
                });
            }

            return result;
        }

        public static ZkOperationLogModel MapDtoZkOperationLogToZkOperationLogModel(DtoZkOperationLog inputItem)
        {
            return MapDtoZkOperationLogToZkOperationLogModel(new List<DtoZkOperationLog> { inputItem }).FirstOrDefault();
        }

        public static List<DeviceEventLogModel> MapDtoDeviceEventLogToDeviceEventLogModel(List<DtoDeviceEventLog> inputItems)
        {
            var result = new List<DeviceEventLogModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<DeviceEventLogModel>();
            }

            foreach (var item in inputItems)
            {
                result.Add(new DeviceEventLogModel
                {
                    DeviceNumber = item.DeviceId,
                    EventDateTime = item.EventDateTime.ToNumericDateTime(),
                    Id = item.Id.ToString(),
                    EmployeeNumber = item.UserIdOnDevice,
                    EventCode = item.EventCode,
                    SdkVersion = item.SdkVersion,
                    Producer = item.Producer,
                });
            }

            return result;
        }

        public static DeviceEventLogModel MapDtoDeviceEventLogToDeviceEventLogModel(DtoDeviceEventLog inputItem)
        {
            return MapDtoDeviceEventLogToDeviceEventLogModel(new List<DtoDeviceEventLog> { inputItem }).FirstOrDefault();
        }

        public static List<EmployeeFingerModel> MapDtoEmployeeFingerToEmployeeFingerModel(List<DtoUserFinger> inputItems)
        {
            var result = new List<EmployeeFingerModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new EmployeeFingerModel
                {
                    EmployeeNumber = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    FingerIndex = model.FingerIndex,
                    TemplateData = Convert.ToBase64String(model.TemplateData)
                });
            }

            return result;
        }

        public static EmployeeFingerModel MapDtoEmployeeFingerToEmployeeFingerModel(DtoUserFinger inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeFingerToEmployeeFingerModel(new List<DtoUserFinger> { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static List<DtoUserFinger> MapToEmployeeFingerModelDtoEmployeeFinger(List<EmployeeFingerModel> inputItems)
        {
            var result = new List<DtoUserFinger>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserFinger
                {
                    UserIdOnDevice = model.EmployeeNumber,
                    CheckSum = model.CheckSum,
                    FingerIndex = model.FingerIndex,
                    TemplateData = Convert.FromBase64String(model.TemplateData)
                });
            }

            return result;
        }

        public static List<EmployeeFaceModel> MapDtoEmployeeFaceToEmployeeFaceModel(List<DtoUserFace> inputItems)
        {
            var result = new List<EmployeeFaceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new EmployeeFaceModel
                {
                    EmployeeNumber = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    FaceIndex = model.FaceIndex,
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    Length = model.Length,
                    SupremaSdk2FaceFlag = model.SupremaSdk2FaceFlag,
                    SupremaSdk2FaceImageData = model.SupremaSdk2FaceImageData.IsCollectionNotNullOrEmpty()
                            ? Convert.ToBase64String(model.SupremaSdk2FaceImageData)
                            : null,
                    SupremaSdk2FaceImageLen = model.SupremaSdk2FaceImageLen,
                    SupremaSdk2FaceNumOfTemplate = model.SupremaSdk2FaceNumOfTemplate,
                });
            }

            return result;
        }

        public static List<DtoUserFace> MapToEmployeeFaceModelDtoEmployeeFace(List<EmployeeFaceModel> inputItems)
        {
            var result = new List<DtoUserFace>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserFace
                {
                    UserIdOnDevice = model.EmployeeNumber,
                    CheckSum = model.CheckSum,
                    FaceIndex = model.FaceIndex,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                    Length = model.Length,
                    SupremaSdk2FaceFlag = model.SupremaSdk2FaceFlag,
                    SupremaSdk2FaceImageData = model.SupremaSdk2FaceImageData.IsNotNullOrEmpty()
                        ? Convert.FromBase64String(model.SupremaSdk2FaceImageData)
                        : null,
                    SupremaSdk2FaceImageLen = model.SupremaSdk2FaceImageLen,
                    SupremaSdk2FaceNumOfTemplate = model.SupremaSdk2FaceNumOfTemplate,
                });
            }

            return result;
        }

        public static EmployeeFaceModel MapDtoEmployeeFaceToEmployeeFaceModel(DtoUserFace inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeFaceToEmployeeFaceModel(new List<DtoUserFace> { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static List<DtoUserPalm> MapToEmployeePalmModelDtoEmployeePalm(List<EmployeePalmModel> inputItems)
        {
            var result = new List<DtoUserPalm>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserPalm
                {
                    UserIdOnDevice = model.EmployeeNumber,
                    CheckSum = model.CheckSum,
                    Index = model.Index,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                    Length = model.Length
                });
            }

            return result;
        }

        public static List<DtoUserIris> MapToEmployeeIrisModelDtoEmployeeIris(List<EmployeeIrisModel> inputItems)
        {
            var result = new List<DtoUserIris>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserIris
                {
                    UserIdOnDevice = model.EmployeeNumber,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                });
            }

            return result;
        }

        public static DtoPadisControllerUserAccessData MapPadisControllerUserAccessDataModelToDtoPadisControllerUserAccessData
            (PadisControllerUserAccessDataModel inputItem)
        {
            var result = new DtoPadisControllerUserAccessData();
            if (inputItem != null)
            {
                result.UserGroupNumbers = inputItem.UserGroupNumbers;
                if (inputItem.EasyPermissions.IsCollectionNotNullOrEmpty())
                {
                    result.EasyPermissions = inputItem.EasyPermissions.Select(ii => new DtoPadisControllerUserEasyPermission
                    {
                        
                        CalendarNumber = ii.CalendarNumber,
                        DoorId = ii.DoorId, 
                        EndDateTime = ii.EndDateTime.FromNumericDateTime(),
                        StartDateTime = ii.StartDateTime.FromNumericDateTime(),
                    }).ToList();
                }
                if (inputItem.AttendanceLimitations.IsCollectionNotNullOrEmpty())
                {
                    result.AttendanceLimitations = inputItem.AttendanceLimitations.Select(ii => new DtoPadisControllerUserLimitationAttendance
                    {
                        DoorId = ii.DoorId,
                        EndDateTime = ii.EndDateTime.FromNumericDateTime(),
                        StartDateTime = ii.StartDateTime.FromNumericDateTime(),
                        AttendanceCount = ii.AttendanceCount,
                        IoType = ii.IoType,
                    }).ToList();
                }
                if (inputItem.TimeLimitations.IsCollectionNotNullOrEmpty())
                {
                    result.TimeLimitations = inputItem.TimeLimitations.Select(ii => new DtoPadisControllerUserLimitationTime
                    {
                        DoorId = ii.DoorId,
                        EndDateTime = ii.EndDateTime.FromNumericDateTime(),
                        StartDateTime = ii.StartDateTime.FromNumericDateTime(),
                    }).ToList();
                }
            }
            return result;
        }

        public static PadisControllerUserAccessDataModel MapDtoPadisControllerUserAccessDataToPadisControllerUserAccessDataModel
            (DtoPadisControllerUserAccessData inputItem)
        {
            var result = new PadisControllerUserAccessDataModel();
            if (inputItem != null)
            {
                result.UserGroupNumbers = inputItem.UserGroupNumbers;
                if (inputItem.EasyPermissions.IsCollectionNotNullOrEmpty())
                {
                    result.EasyPermissions = inputItem.EasyPermissions.Select(ii => new PadisControllerEasyPermissionModel
                    {
                        CalendarNumber = ii.CalendarNumber,
                        DoorId = ii.DoorId, 
                        EndDateTime = ii.EndDateTime.ToNumericDateTime(),
                        StartDateTime = ii.StartDateTime.ToNumericDateTime(),
                    }).ToList();
                }
                if (inputItem.AttendanceLimitations.IsCollectionNotNullOrEmpty())
                {
                    result.AttendanceLimitations = inputItem.AttendanceLimitations.Select(ii => new PadisControllerUserLimitationAttendanceModel
                    {
                        DoorId = ii.DoorId,
                        EndDateTime = ii.EndDateTime.ToNumericDateTime(),
                        StartDateTime = ii.StartDateTime.ToNumericDateTime(),
                        AttendanceCount = ii.AttendanceCount,
                        IoType = ii.IoType,
                    }).ToList();
                }
                if (inputItem.TimeLimitations.IsCollectionNotNullOrEmpty())
                {
                    result.TimeLimitations = inputItem.TimeLimitations.Select(ii => new PadisControllerUserLimitationTimeModel
                    {
                        DoorId = ii.DoorId,
                        EndDateTime = ii.EndDateTime.ToNumericDateTime(),
                        StartDateTime = ii.StartDateTime.ToNumericDateTime(),
                    }).ToList();
                }
            }
            return result;
        }

        public static List<EmployeePalmModel> MapDtoEmployeePalmToEmployeePalmModel(List<DtoUserPalm> inputItems)
        {
            var result = new List<EmployeePalmModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new EmployeePalmModel
                {
                    CheckSum = model.CheckSum,
                    Index = model.Index,
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    Length = model.Length,
                    EmployeeNumber = model.UserIdOnDevice,
                });
            }

            return result;
        }

        public static List<EmployeeIrisModel> MapDtoEmployeeIrisToEmployeeIrisModel(List<DtoUserIris> inputItems)
        {
            var result = new List<EmployeeIrisModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new EmployeeIrisModel
                {
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    EmployeeNumber = model.UserIdOnDevice,
                });
            }

            return result;
        }

        public static EmployeeIrisModel MapDtoEmployeeIrisToEmployeeIrisModel(DtoUserIris inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeIrisToEmployeeIrisModel(new List<DtoUserIris>() { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static EmployeePalmModel MapDtoEmployeePalmToEmployeePalmModel(DtoUserPalm inputItem)
        {
            return MapDtoEmployeePalmToEmployeePalmModel(new List<DtoUserPalm> { inputItem }).FirstOrDefault();
        }

        public static EmployeeImageModel MapDtoEmployeeImageToEmployeeImageModel(DtoUserImage inputItem)
        {
            return new EmployeeImageModel
            {
                EmployeeNumber = inputItem.UserIdOnDevice,
                EmployeeImage = inputItem.PhotoData != null
                    ? Convert.ToBase64String(inputItem.PhotoData)
                    : null
            };
        }

        public static EmployeeModel MapDtoEmployeeToEmployeeModel(DtoUserDeviceRelatedData inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new EmployeeModel
            {
                EmployeeNumber = inputItem.UserIdOnDevice,
                Privilege = inputItem.Privilege,
                TimeZones = inputItem.TimeZones,
                SupremaSdk1AccessGroups = inputItem.SupremaSdk1AccessGroups,
                SupremaSdk2AccessGroups = inputItem.SupremaSdk2AccessGroups,
                VirdiAccessGroupCode = inputItem.VirdiAccessGroupCode,
                TimyWeekTimezoneDeviceIndex = inputItem.TimyWeekTimezoneDeviceIndex,
                ElevatorInfoInJsonFormat = inputItem.ElevatorInfoInJsonFormat,
                CabinetInfoInJsonFormat = inputItem.CabinetInfoInJsonFormat,
                VerificationStyle = inputItem.VerificationStyle,
                FaceDataList = MapDtoEmployeeFaceToEmployeeFaceModel(inputItem.FaceDataList),
                HardwareProfileImage = inputItem.HardwareProfileImage != null
                        ? Convert.ToBase64String(inputItem.HardwareProfileImage)
                        : null,
                Password = inputItem.Password,
                EndDate = inputItem.EndTime.ToNumericDateTime(),
                FingerDataList = MapDtoEmployeeFingerToEmployeeFingerModel(inputItem.FingerDataList),
                IsEnable = inputItem.IsEnable,
                PalmDataList = MapDtoEmployeePalmToEmployeePalmModel(inputItem.PalmDataList),
                IrisDataList = MapDtoEmployeeIrisToEmployeeIrisModel(inputItem.IrisDataList),
                RfCardNumbers = inputItem.RfCardNumbers,
                VisibleLightImage = inputItem.VisibleLightImage != null
                    ? Convert.ToBase64String(inputItem.VisibleLightImage)
                    : null,
                StartDate = inputItem.StartTime.ToNumericDateTime(),
                UserName = inputItem.UserName,
                PadisControllerUserAccessData = MapDtoPadisControllerUserAccessDataToPadisControllerUserAccessDataModel(inputItem.PadisControllerUserAccessData),
            };
            return result;
        }

        public static EmployeeEnrolledSettingModel MapDtoEmployeeEnrolledSettingToEmployeeEnrolledSettingModel(DtoUserEnrolledSetting inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new EmployeeEnrolledSettingModel
            {
                OverwriteDevicePassword = inputItem.OverwriteDevicePassword,
                OverwriteIsEnabled = inputItem.OverwriteIsEnabled,
                OverwritePrivilege = inputItem.OverwritePrivilege,
                OverwriteRfCardNumber = inputItem.OverwriteRfCardNumber,
                OverwriteVerificationStyle = inputItem.OverwriteVerificationStyle,
            };
            return result;
        }

        public static List<DtoUserDeviceRelatedData> MapEmployeeModelToDtoEmployee(List<EmployeeModel> inputItems)
        {
            var result = new List<DtoUserDeviceRelatedData>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserDeviceRelatedData
                {
                    UserIdOnDevice = model.EmployeeNumber,
                    Privilege = model.Privilege,
                    TimeZones = model.TimeZones,
                    SupremaSdk1AccessGroups = model.SupremaSdk1AccessGroups,
                    SupremaSdk2AccessGroups = model.SupremaSdk2AccessGroups,
                    VirdiAccessGroupCode = model.VirdiAccessGroupCode,
                    TimyWeekTimezoneDeviceIndex = model.TimyWeekTimezoneDeviceIndex,
                    ElevatorInfoInJsonFormat = model.ElevatorInfoInJsonFormat,
                    CabinetInfoInJsonFormat = model.CabinetInfoInJsonFormat,
                    VerificationStyle = model.VerificationStyle,
                    FaceDataList = MapToEmployeeFaceModelDtoEmployeeFace(model.FaceDataList),
                    HardwareProfileImage = model.HardwareProfileImage != null
                        ? Convert.FromBase64String(model.HardwareProfileImage)
                        : null,
                    Password = model.Password,
                    EndTime = model.EndDate.FromNumericDateTime(),
                    FingerDataList = MapToEmployeeFingerModelDtoEmployeeFinger(model.FingerDataList),
                    IsEnable = model.IsEnable,
                    PalmDataList = MapToEmployeePalmModelDtoEmployeePalm(model.PalmDataList),
                    IrisDataList = MapToEmployeeIrisModelDtoEmployeeIris(model.IrisDataList),
                    PadisControllerUserAccessData = MapPadisControllerUserAccessDataModelToDtoPadisControllerUserAccessData(model.PadisControllerUserAccessData),
                    RfCardNumbers = model.RfCardNumbers,
                    VisibleLightImage = model.VisibleLightImage != null
                        ? Convert.FromBase64String(model.VisibleLightImage)
                        : null,
                    StartTime = model.StartDate.FromNumericDateTime(),
                    UserName = model.UserName,
                    UserType = model.UserType,
                });
            }

            return result;
        }

        public static DtoUserDeviceRelatedData MapEmployeeModelToDtoEmployee(EmployeeModel inputItem)
        {
            return MapEmployeeModelToDtoEmployee(new List<EmployeeModel> { inputItem }).FirstOrDefault();
        }

        public static DtoZkDeviceDoor MapZkDeviceDoorModelToDtoZkDeviceDoor(ZkDeviceDoorModel inputItem)
        {
            return MapZkDeviceDoorModelToDtoZkDeviceDoor(new List<ZkDeviceDoorModel> { inputItem }).FirstOrDefault();
        }

        public static List<DtoZkDeviceDoor> MapZkDeviceDoorModelToDtoZkDeviceDoor(List<ZkDeviceDoorModel> inputItems)
        {
            var result = new List<DtoZkDeviceDoor>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoZkDeviceDoor
                {
                    DeviceNumber = model.DeviceNumber,
                    ActiveTimeZoneNumber = model.ActiveTimeZoneNumber,
                    AllowSuperuserAccessWhenLockDown = model.AllowSuperuserAccessWhenLockDown,
                    AntiPassBackDurationOfEntrance = model.AntiPassBackDurationOfEntrance,
                    CardNumberReversal = model.CardNumberReversal,
                    DisableAlarmSounds = model.DisableAlarmSounds,
                    DoorNumber = model.DoorNumber,
                    DoorSensorDelay = model.DoorSensorDelay,
                    DoorSensorType = model.DoorSensorType,
                    DuressPassword = model.DuressPassword,
                    EmergencyPassword = model.EmergencyPassword,
                    Id = model.Id,
                    IsActive = model.IsActive,
                    LockOpenDuration = model.LockOpenDuration,
                    MultiPersonOperationInterval = model.MultiPersonOperationInterval,
                    OpenDoorDelay = model.OpenDoorDelay,
                    OperateInterval = model.OperateInterval,
                    PassageDelay = model.PassageDelay,
                    PassageModeTimeZoneNumber = model.PassageModeTimeZoneNumber,
                    ReverseLockStateOnDoorClose = model.ReverseLockStateOnDoorClose,
                    Title = model.Title,
                    VerificationStyle = model.VerificationStyle,
                    WiegandFormat = model.WiegandFormat,
                });
            }

            return result;
        }

        public static List<DtoCommunicationDeviceData> MapDeviceCommunicationModelToDtoDeviceCommunication(List<DeviceCommunicationModel> inputItems)
        {
            var result = new List<DtoCommunicationDeviceData>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoCommunicationDeviceData
                {
                    DeviceNumber = model.DeviceNumber,
                    ConnectionMode = model.ConnectionMode,
                    HasFace = model.HasFace,
                    AutomaticDataCollect = model.AutomaticDataCollect,
                    IsOldVersion = model.IsOldVersion,
                    DeviceSettings = model.DeviceSettings,
                    IoType = model.IoType,
                    SendProfileImage = model.SendProfileImage,
                    ApplicationId = model.ApplicationId,
                    HasSoftwareAccessControl = model.HasSoftwareAccessControl,
                    OnlineMonitoringMode = model.OnlineMonitoringMode,
                    JustDoorControl = model.JustDoorControl,
                    BuadRate = model.BuadRate,
                    ComPort = model.ComPort,
                    CommunicationPassword = model.CommunicationPassword,
                    ConnectTimeout = model.ConnectTimeout,
                    ConnectionTypeEnum = model.ConnectionTypeEnum,
                    DeviceTypeCode = model.DeviceTypeCode,
                    DeviceTypeNumber = model.DeviceTypeNumber,
                    ProducerEnum = model.ProducerEnum,
                    SerialNumber = model.SerialNumber,
                    HasFinger = model.HasFinger,
                    SdkVersionEnum = model.SdkVersionEnum,
                    TcpPort = model.TcpPort,
                    IsHookActive = model.IsHookActive,
                    HasRfCard = model.HasRfCard,
                    PwConnectionTimeout = model.PwConnectionTimeout,
                    PwMaxRetry = model.PwMaxRetry,
                    PwPcPort = model.PwPcPort,
                    PwAcceptValidList = model.PwAcceptValidList,
                    DoorTypeEnum = model.DoorTypeEnum,
                    EnrollStandardEnum = model.EnrollStandardEnum,
                    HasAttendanceValidationCheck = model.HasAttendanceValidationCheck,
                    HasPalm = model.HasPalm,
                    HasIris = model.HasIris,
                    HasVisibleLight = model.HasVisibleLight,
                    Ip = model.Ip,
                    IsMasterDevice = model.IsMasterDevice,
                    TimeSetting = new DtoCommunicationDeviceTimeSettings
                    {
                        IsDaylightActive = model.IsDaylightActive,
                        DaylightEnd = model.DaylightEnd,
                        DaylightStart = model.DaylightStart,
                        DaylightChangeTimeInSeconds = model.DaylightChangeTimeInSeconds,
                        TimeZone = model.TimeZone,
                    },
                });
            }

            return result;
        }

        public static DtoCommunicationDeviceData MapDeviceCommunicationModelToDtoDeviceCommunication(DeviceCommunicationModel inputItem)
        {
            return MapDeviceCommunicationModelToDtoDeviceCommunication(new List<DeviceCommunicationModel> { inputItem }).FirstOrDefault();
        }

        public static List<DtoDeviceHoliday> MapDeviceHolidayModelToDtoAcDeviceHoliday(List<DeviceHolidayModel> inputItems)
        {
            var result = new List<DtoDeviceHoliday>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoDeviceHoliday
                {
                    DeviceNumber = model.DeviceNumber,
                    EndDate = model.EndDate.FromNumericDateTime(),
                    HolidayIndex = model.HolidayIndex,
                    Id = model.Id,
                    StartDate = model.StartDate.FromNumericDateTime(),
                    TimeZoneNumber = model.TimeZoneNumber
                });
            }

            return result;
        }

        public static DtoDeviceDoorBase MapDeviceDoorBaseModelToDtoAcDeviceDoorBase(DeviceDoorBaseModel inputItem)
        {
            return MapDeviceDoorBaseModelToDtoAcDeviceDoorBase(new List<DeviceDoorBaseModel> { inputItem }).FirstOrDefault();
        }

        public static List<DtoDeviceDoorBase> MapDeviceDoorBaseModelToDtoAcDeviceDoorBase(List<DeviceDoorBaseModel> inputItems)
        {
            var result = new List<DtoDeviceDoorBase>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoDeviceDoorBase
                {
                    DeviceNumber = model.DeviceNumber,
                    OpenDoorDelay = model.OpenDoorDelay,
                    DoorNumber = model.DoorNumber,
                    IsActive = model.IsActive,
                    Id = model.Id,
                    Title = model.Title,
                });
            }

            return result;
        }

        public static List<DtoSupremaSdk1Timezone> MapSupremaSdk1TimezoneModelToDtoSupremaSdk1Timezone(List<SupremaSdk1TimezoneModel> inputItems)
        {
            var result = new List<DtoSupremaSdk1Timezone>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk1Timezone
                {
                    Elements = new List<DtoSupremaSdk1TimezoneElement>(),
                    HolidayGroupNumber1 = model.HolidayGroupNumber1,
                    HolidayGroupNumber2 = model.HolidayGroupNumber2,
                    TimezoneDescription = model.TimezoneDescription,
                    TimezoneNumber = model.TimezoneNumber,
                    TimezoneTitle = model.TimezoneTitle,
                };
                if (model.Elements.IsCollectionNotNullOrEmpty())
                {
                    foreach (var element in model.Elements)
                    {
                        current.Elements.Add(new DtoSupremaSdk1TimezoneElement
                        {
                            ElementCode = element.ElementCode,
                            EndTime = element.EndTime,
                            TimezoneNumber = element.TimezoneNumber,
                            Id = element.Id,
                            StartTime = element.StartTime
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoSupremaSdk1DeviceHolidayGroup> MapDeviceHolidayModelToDtoAcDeviceHoliday(List<SupremaSdk1DeviceHolidayGroupModel> inputItems)
        {
            var result = new List<DtoSupremaSdk1DeviceHolidayGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk1DeviceHolidayGroup
                {
                    GroupDescription = model.GroupDescription,
                    GroupName = model.GroupName,
                    GroupNumber = model.GroupNumber,
                };
                if (model.Holidays.IsCollectionNotNullOrEmpty())
                {
                    current.Holidays = new List<DtoSupremaSdk1DeviceHoliday>();
                    foreach (var holidayModel in model.Holidays)
                    {
                        current.Holidays.Add(new DtoSupremaSdk1DeviceHoliday
                        {
                            HollidayDuration = holidayModel.HollidayDuration,
                            HolidayDate = holidayModel.HolidayDate.FromNumericDateTime(),
                            Id = holidayModel.Id,
                            IsRepeatYearly = holidayModel.IsRepeatYearly,
                            HolidayGroupNumber = holidayModel.HolidayGroupNumber,
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoSupremaSdk1AccessGroup> MapSupremaSdk1AccessGroupModelToDtoSupremaSdk1AccessGroup(List<SupremaSdk1AccessGroupModel> inputItems)
        {
            var result = new List<DtoSupremaSdk1AccessGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk1AccessGroup
                {
                    DoorTimezones = new List<DtoSupremaSdk1AccessGroupDoorTimezone>(),
                    AccessGroupNumber = model.AccessGroupNumber,
                    Description = model.Description,
                    Title = model.Title,
                };
                if (model.DoorTimezones.IsCollectionNotNullOrEmpty())
                {
                    foreach (var timezone in model.DoorTimezones)
                    {
                        current.DoorTimezones.Add(new DtoSupremaSdk1AccessGroupDoorTimezone
                        {
                            TimezoneNumber = timezone.TimezoneNumber,
                            DeviceDoorId = timezone.DeviceDoorId,
                        });
                    }
                }
            }

            return result;
        }

        public static DtoSupremaSdk1DeviceDoor MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(SupremaSdk1DeviceDoorModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoSupremaSdk1DeviceDoor()
            {
                Rte = inputItem.Rte,
                UseRteEx = inputItem.UseRteEx,
                RteType = inputItem.RteType,
                AlarmStatus = inputItem.AlarmStatus,
                DeviceNumber = inputItem.DeviceNumber,
                DoorNumber = inputItem.DoorNumber,
                DoorSensor = inputItem.DoorSensor,
                ForcedCloseSchedule = inputItem.ForcedCloseSchedule,
                ForcedOpenSchedule = inputItem.ForcedOpenSchedule,
                HeldOpenTime = inputItem.HeldOpenTime,
                Id = inputItem.Id,
                IsActive = inputItem.IsActive,
                OpenDoorDelay = inputItem.OpenDoorDelay,
                OpenEvent = inputItem.OpenEvent,
                OpenOnce = inputItem.OpenOnce,
                OpenTime = inputItem.OpenTime,
                Reader1 = inputItem.Reader1,
                Reader2 = inputItem.Reader2,
                Relay = inputItem.Relay,
                RelayDeviceId = inputItem.RelayDeviceId,
                SensorType = inputItem.SensorType,
                Title = inputItem.Title,
                UseDoorSensorEx = inputItem.UseDoorSensorEx,
                UseSoundForcedOpen = inputItem.UseSoundForcedOpen,
                UseSoundHeldOpen = inputItem.UseSoundHeldOpen,
            };

        }

        public static List<DtoSupremaSdk2AccessSchedule> MapSupremaSdk2AccessScheduleModelToDtoSupremaSdk2AccessSchedule(List<SupremaSdk2AccessScheduleModel> inputItems)
        {
            var result = new List<DtoSupremaSdk2AccessSchedule>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk2AccessSchedule
                {
                    Elements = new List<DtoSupremaSdk2AccessScheduleElement>(),
                    HolidayGroupNumber1 = model.HolidayGroupNumber1,
                    HolidayGroupNumber2 = model.HolidayGroupNumber2,
                    HolidayGroupNumber3 = model.HolidayGroupNumber3,
                    HolidayGroupNumber4 = model.HolidayGroupNumber4,
                    TimezoneDescription = model.TimezoneDescription,
                    AccessScheduleNumber = model.AccessScheduleNumber,
                    Title = model.Title,
                };
                if (model.Elements.IsCollectionNotNullOrEmpty())
                {
                    foreach (var element in model.Elements)
                    {
                        current.Elements.Add(new DtoSupremaSdk2AccessScheduleElement
                        {
                            ElementCode = element.ElementCode,
                            EndTime = element.EndTime,
                            AccessScheduleNumber = element.AccessScheduleNumber,
                            Id = element.Id,
                            StartTime = element.StartTime
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoSupremaSdk2DeviceHolidayGroup> MapDeviceHolidayModelToDtoAcDeviceHoliday(List<SupremaSdk2DeviceHolidayGroupModel> inputItems)
        {
            var result = new List<DtoSupremaSdk2DeviceHolidayGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk2DeviceHolidayGroup
                {
                    GroupDescription = model.GroupDescription,
                    GroupName = model.GroupName,
                    GroupNumber = model.GroupNumber,
                };
                if (model.Holidays.IsCollectionNotNullOrEmpty())
                {
                    current.Holidays = new List<DtoSupremaSdk2DeviceHoliday>();
                    foreach (var holidayModel in model.Holidays)
                    {
                        current.Holidays.Add(new DtoSupremaSdk2DeviceHoliday
                        {
                            HolidayDate = holidayModel.HolidayDate.FromNumericDateTime(),
                            Id = holidayModel.Id,
                            IsRepeatYearly = holidayModel.IsRepeatYearly,
                            HolidayGroupNumber = holidayModel.HolidayGroupNumber,
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoTimyDayTimezoneGroup> MapTimyDayTimezoneGroupModelToDtoTimyDayTimezoneGroup(List<TimyDayTimezoneGroupModel> inputItems)
        {
            var result = new List<DtoTimyDayTimezoneGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoTimyDayTimezoneGroup
                {
                    Id = model.Id,
                    DeviceIndex = model.DeviceIndex,
                    Title = model.Title,
                    DayTimezoneIntervals = new List<DtoTimyDayTimezoneInterval>()
                };
                if (model.DayTimezoneIntervals.IsCollectionNotNullOrEmpty())
                {
                    foreach (var timezone in model.DayTimezoneIntervals)
                    {
                        current.DayTimezoneIntervals.Add(new DtoTimyDayTimezoneInterval
                        {
                            StartHourMinute = timezone.StartHourMinute,
                            EndHourMinute = timezone.EndHourMinute
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoTimyWeekTimezoneGroup> MapTimyWeekTimezoneGroupModelToDtoTimyWeekTimezoneGroup(List<TimyWeekTimezoneGroupModel> inputItems)
        {
            var result = new List<DtoTimyWeekTimezoneGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoTimyWeekTimezoneGroup
                {
                    Id = model.Id,
                    DeviceIndex = model.DeviceIndex,
                    Title = model.Title,
                    Timezones = new List<DtoTimyWeekTimezone>()
                };
                if (model.Timezones.IsCollectionNotNullOrEmpty())
                {
                    foreach (var timezone in model.Timezones)
                    {
                        current.Timezones.Add(new DtoTimyWeekTimezone
                        {
                            WeekTimezoneGroupId = timezone.WeekTimezoneGroupId,
                            WeekDay = timezone.WeekDay,
                            DayTimezoneIndex = timezone.DayTimezoneIndex
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoTimyHoliday> MapTimyHolidayModelToDtoTimyHoliday(List<TimyHolidayModel> inputItems)
        {
            var result = new List<DtoTimyHoliday>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoTimyHoliday
                {
                    Title = model.Title,
                    StartDate = model.StartDate.FromNumericDateTime(),
                    EndDate = model.EndDate.FromNumericDateTime(),
                    DayTimezoneIndex = model.DayTimezoneIndex
                });
            }

            return result;
        }

        public static List<DtoSupremaSdk2AccessLevel> MapSupremaSdk2AccessLevelModelToDtoSupremaSdk2AccessLevel(List<SupremaSdk2AccessLevelModel> inputItems)
        {
            var result = new List<DtoSupremaSdk2AccessLevel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk2AccessLevel
                {
                    DoorAccessSchedules = new List<DtoSupremaSdk2AccessLevelDoorAccessSchedule>(),
                    AccessLevelNumber = model.AccessLevelNumber,
                    Description = model.Description,
                    Title = model.Title,
                };
                if (model.DoorAccessSchedules.IsCollectionNotNullOrEmpty())
                {
                    foreach (var accessSchedule in model.DoorAccessSchedules)
                    {
                        current.DoorAccessSchedules.Add(new DtoSupremaSdk2AccessLevelDoorAccessSchedule
                        {
                            AccessLevelNumber = accessSchedule.AccessLevelNumber,
                            DoorId = accessSchedule.DeviceDoorId,
                            AccessScheduleNumber = accessSchedule.AccessScheduleNumber,
                            Id = accessSchedule.Id,
                        });
                    }
                }
                result.Add(current);
            }

            return result;
        }

        public static List<DtoSupremaSdk2AccessGroup> MapSupremaSdk2AccessGroupModelToDtoSupremaSdk2AccessGroup(List<SupremaSdk2AccessGroupModel> inputItems)
        {
            var result = new List<DtoSupremaSdk2AccessGroup>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                var current = new DtoSupremaSdk2AccessGroup
                {
                    AccessLevelNumbers = new List<int>(),
                    AccessGroupNumber = model.AccessGroupNumber,
                    Description = model.Description,
                    Title = model.Title,
                };
                if (model.AccessLevels.IsCollectionNotNullOrEmpty())
                {
                    current.AccessLevelNumbers.AddRange(model.AccessLevels);
                }
                result.Add(current);
            }

            return result;
        }

        public static DtoSupremaSdk2DeviceDoor MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(SupremaSdk2DeviceDoorModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoSupremaSdk2DeviceDoor()
            {
                IsActive = inputItem.IsActive,
                ApbUseDoorSensor = inputItem.ApbUseDoorSensor,
                DeviceDoorId = inputItem.DeviceDoorId,
                AutoLockTimeout = inputItem.AutoLockTimeout,
                UnconditionalLock = inputItem.UnconditionalLock,
                HasExitButton = inputItem.HasExitButton,
                DeviceNumber = inputItem.DeviceNumber,
                DoorNumber = inputItem.DoorNumber,
                DoorSensor = inputItem.DoorSensor,
                ExitButton = inputItem.ExitButton,
                ExitButtonType = inputItem.ExitButtonType,
                ForceOpenAlarmType = inputItem.ForceOpenAlarmType,
                HasDoorSensor = inputItem.HasDoorSensor,
                HasRelay = inputItem.HasRelay,
                HeldOpenAlarmType = inputItem.HeldOpenAlarmType,
                HeldOpenTimeout = inputItem.HeldOpenTimeout,
                Id = inputItem.Id,
                InstantLock = inputItem.InstantLock,
                LockFlags = inputItem.LockFlags,
                OpenDoorDelay = inputItem.OpenDoorDelay,
                Relay = inputItem.Relay,
                SensorType = inputItem.SensorType,
                Title = inputItem.Title,
                UnlockFlag = inputItem.UnlockFlag,
            };

        }

        public static DtoPadisControllerDeviceDoor MapPadisControllerDoorModelToDtoPadisControllerDoor(PadisControllerDeviceDoorModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoPadisControllerDeviceDoor()
            {
                IsActive = inputItem.IsActive,
                CombinationAccessGroupNumbersInJson = inputItem.CombinationAccessGroupNumbersInJson,
                DeviceTitle = inputItem.DeviceTitle,
                DoorNumberOnDevice = inputItem.DoorNumberOnDevice,
                OpenTimeCalendarNumber = inputItem.OpenTimeCalendarNumber,
                PassVerificationIoPortId = inputItem.PassVerificationIoPortId,
                ReaderDeviceNumber = inputItem.ReaderDeviceNumber,
                ReaderIoType = inputItem.ReaderIoType,
                WiegandId = inputItem.WiegandId,
                DeviceNumber = inputItem.DeviceNumber,
                DoorNumber = inputItem.DoorNumber,
                Id = inputItem.Id,
                OpenDoorDelay = inputItem.OpenDoorDelay,
                Title = inputItem.Title
            };
        }

        public static DtoPadisControllerWiegand MapPadisControllerWiegandModelToDtoPadisControllerWiegand(PadisControllerWiegandModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoPadisControllerWiegand()
            {
                IsActive = inputItem.IsActive,
                WiegandDataType = inputItem.WiegandDataType,
                WiegandFormat = inputItem.WiegandFormat,
                WiegandNumber = inputItem.WiegandNumber,
                Id = inputItem.Id,
                Title = inputItem.Title,
                DeviceNumber = inputItem.DeviceNumber,
            };
        }

        public static DtoPadisControllerRelay MapPadisControllerRelayModelToDtoPadisControllerRelay(PadisControllerRelayModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoPadisControllerRelay()
            {
                IsActive = inputItem.IsActive,
                Id = inputItem.Id,
                Title = inputItem.Title,
                DeviceNumber = inputItem.DeviceNumber,
                RelayNumber = inputItem.RelayNumber,
                RelayType = inputItem.RelayType,
            };
        }

        public static DtoPadisControllerIoPort MapPadisControllerIoPortModelToDtoPadisControllerIoPort(PadisControllerIoPortModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            return new DtoPadisControllerIoPort()
            {
                IsActive = inputItem.IsActive,
                Id = inputItem.Id,
                Title = inputItem.Title,
                DeviceNumber = inputItem.DeviceNumber,
                IoNumber = inputItem.IoNumber,
            };
        }

        public static DtoPadisControllerCalendar MapPadisControllerCalendarModelToDtoPadisControllerCalendar(PadisControllerCalendarModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new DtoPadisControllerCalendar()
            {
                IsActive = inputItem.IsActive,
                Title = inputItem.Title,
                CalendarNumber = inputItem.CalendarNumber,
                Details = new List<DtoPadisControllerCalendarDetails>()
            };

            if (inputItem.Details.IsCollectionNotNullOrEmpty())
            {
                result.Details = inputItem.Details.Select(ii => new DtoPadisControllerCalendarDetails() {
                    Date = ii.Date.FromNumericDateTime(),
                    EndTime = ii.EndTime,
                    Id = ii.Id,
                    PadisControllerCalendarNumber = ii.PadisControllerCalendarNumber,
                    StartTime = ii.StartTime,
                }).ToList();
            }
            return result;
        }

        public static DtoPadisControllerAccessLevel MapPadisControllerAccessLevelModelToDtoPadisControllerAccessLevel(PadisControllerAccessLevelModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new DtoPadisControllerAccessLevel()
            {
                IsActive = inputItem.IsActive,
                Title = inputItem.Title,
                AccessLevelNumber = inputItem.AccessLevelNumber,
                Doors = new List<DtoPadisControllerAccessLevelDoor>()
            };

            if (inputItem.Doors.IsCollectionNotNullOrEmpty())
            {
                result.Doors = inputItem.Doors.Select(ii => new DtoPadisControllerAccessLevelDoor() {
                    AccessLevelNumber = ii.AccessLevelNumber,
                    DoorId = ii.DoorId,
                }).ToList();
            }
            return result;
        }

        public static DtoPadisControllerAccessGroup MapPadisControllerAccessGroupModelToDtoPadisControllerAccessGroup(PadisControllerAccessGroupModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new DtoPadisControllerAccessGroup()
            {
                IsActive = inputItem.IsActive,
                Title = inputItem.Title,
                AccessGroupNumber = inputItem.AccessGroupNumber,
                AccessData = new List<DtoPadisControllerAccessGroupAccessData>()
            };

            if (inputItem.AccessData.IsCollectionNotNullOrEmpty())
            {
                result.AccessData = inputItem.AccessData.Select(ii => new DtoPadisControllerAccessGroupAccessData() {
                    AccessGroupNumber = ii.AccessGroupNumber,
                    AccessLevelNumber = ii.AccessLevelNumber,
                    CalendarNumber = ii.CalendarNumber,
                }).ToList();
            }
            return result;
        }

        public static List<DtoTimezone> MapTimezoneModelToDtoTimezone(List<TimezoneModel> inputItems)
        {
            var result = new List<DtoTimezone>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoTimezone
                {
                    TimeZoneNumber = model.TimeZoneNumber,
                    Description = model.Description,
                    Intervals = MapTimeZoneIntervalModelToDtoTimeZoneInterval(model.Intervals),
                    Title = model.Title
                });
            }

            return result;
        }

        public static DtoTimezone MapTimezoneModelToDtoTimezone(TimezoneModel inputItem)
        {
            return MapTimezoneModelToDtoTimezone(new List<TimezoneModel> { inputItem }).FirstOrDefault();
        }

        public static List<DtoTimeZoneInterval> MapTimeZoneIntervalModelToDtoTimeZoneInterval(List<TimeZoneIntervalModel> inputItems)
        {
            var result = new List<DtoTimeZoneInterval>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoTimeZoneInterval
                {
                    TimeZoneNumber = model.TimeZoneNumber,
                    DayType = model.DayType,
                    EndTime = model.EndTime,
                    Id = model.Id,
                    StartTime = model.StartTime
                });
            }

            return result;
        }

        public static List<DeviceCommandModel> MapDtoDeviceCommandToDeviceCommandModel(List<DtoDeviceCommandWithoutContent> inputItems)
        {
            var result = new List<DeviceCommandModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DeviceCommandModel
                {
                    EmployeeNumber = model.EmployeeNumber,
                    CommandType = model.CommandType,
                    CommitTime = model.CommitTime.ToNumericDateTime(),
                    DeviceSerialNumber = model.DeviceSerialNumber,
                    Id = model.Id,
                    MaxRetry = model.MaxRetry,
                    Priority = model.Priority,
                    ResponseTime = model.ResponseTime.ToNumericDateTime(),
                    ResponseValue = model.ResponseValue,
                    RetryCount = model.RetryCount,
                    SendTime = model.SendTime.ToNumericDateTime(),
                });
            }

            return result;
        }

        public static PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration> MapDeviceCommandSearchModelToSearchInfo(DeviceCommandSearchModel inputItem)
        {
            var result = new PagingData<DeviceCommandFilter, DeviceCommandSortEnumeration>
            {
                Filter = new DeviceCommandFilter()
            };
            if (inputItem == null)
            {
                return result;
            }

            if (inputItem.Filter != null)
            {
                result.Filter = new DeviceCommandFilter();
                if (inputItem.Filter.DeviceNumbers.IsCollectionNotNullOrEmpty())
                {
                    result.Filter.DeviceNumbers = inputItem.Filter.DeviceNumbers;
                }
                if (inputItem.Filter.DeviceSerialNumbers.IsCollectionNotNullOrEmpty())
                {
                    result.Filter.DeviceSerialNumbers = inputItem.Filter.DeviceSerialNumbers;
                }
                if (inputItem.Filter.Ids.IsCollectionNotNullOrEmpty())
                {
                    result.Filter.Ids = inputItem.Filter.Ids;
                }
                if (inputItem.Filter.EmployeeNumbers.IsCollectionNotNullOrEmpty())
                {
                    result.Filter.EmployeeNumbersOnDevice = inputItem.Filter.EmployeeNumbers;
                }
                result.Filter.EmployeeNumberLike = inputItem.Filter.EmployeeNumberLike;
                result.Filter.CommitTimeFrom = inputItem.Filter.CommitTimeFrom.FromNumericDateTime();
                result.Filter.CommitTimeTo = inputItem.Filter.CommitTimeTo.FromNumericDateTime();
                result.Filter.IsSend = inputItem.Filter.IsSend;
                result.Filter.CommandType = inputItem.Filter.CommandType;
                result.Filter.Priority = inputItem.Filter.Priority;
                result.Filter.VisiblilityTimeFrom = inputItem.Filter.VisiblilityTimeFrom.FromNumericDateTime();
                result.Filter.VisiblilityTimeTo = inputItem.Filter.VisiblilityTimeTo.FromNumericDateTime();
                result.Filter.VisiblilityTimeHasValue = inputItem.Filter.VisiblilityTimeHasValue;

            }

            if (inputItem.CurrentPage != null)
            {
                result.CurrentPage = new CurrentPageInfo
                {
                    ItemPerPage = inputItem.CurrentPage.ItemPerPage,
                    PageNumber = inputItem.CurrentPage.PageNumber
                };
            }

            if (inputItem.SortInfos.IsCollectionNotNullOrEmpty())
            {
                result.SortItems = new List<SortInfo<DeviceCommandSortEnumeration>>();
                foreach (var sortInfo in inputItem.SortInfos)
                {
                    result.SortItems.Add(new SortInfo<DeviceCommandSortEnumeration>
                    {
                        SortItemEnum = sortInfo.SortEnum,
                        SortType = sortInfo.SortType,
                    });
                }
            }
            return result;
        }

        public static List<NotSendCommandsStatisticsModel> MapDtoNotSendStatisticsToNotSendStatisticsModel(List<DtoFailedCommandStatistics> inputItems)
        {
            var result = new List<NotSendCommandsStatisticsModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new NotSendCommandsStatisticsModel
                {
                    MaxAttemptCommandCount = model.MaxAttemptCommandCount,
                    NotSendCommandCounts = model.NotSendCommandCounts,
                    DeviceNumber = model.DeviceNumber,
                });
            }

            return result;
        }

        public static SystemConfigModel MapDtoSystemConfigToSystemConfigModel(DtoSystemConfig inputItem)
        {
            return new SystemConfigModel
            {
                AttendanceRegisterInterval = inputItem.AttendanceRegisterInterval,
                AttendanceRegisterIntervalForParking = inputItem.AttendanceRegisterIntervalForParking,
                AttendanceRegisterIntervalForTimeAttendance = inputItem.AttendanceRegisterIntervalForTimeAttendance,
                AttendanceRegisterIntervalForAccessControl = inputItem.AttendanceRegisterIntervalForAccessControl,
                OnlineDeviceTimerInterval = inputItem.OnlineDeviceTimerInterval,
                ResetRetryCountDayBefore = inputItem.ResetRetryCountDayBefore,
                KeepCommandAfterResponse = inputItem.KeepCommandAfterResponse,
                OnlineMonitoringDevicesIntervalFromLastDataToReset = inputItem.OnlineMonitoringDevicesIntervalFromLastDataToReset,
                OnlineMonitoringDevicesSleepAfterPingInSecond = inputItem.OnlineMonitoringDevicesSleepAfterPingInSecond,
                OnlineMonitoringDevicesPingTimeoutInMillisecond = inputItem.OnlineMonitoringDevicesPingTimeoutInMillisecond,
                OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = inputItem.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond,
                IsNetworkPingActive = inputItem.IsNetworkPingActive,
                AttendanceHookTimerInterval = inputItem.AttendanceHookTimerInterval,
                AttendanceSendToKarnamaTimerInterval = inputItem.AttendanceSendToKarnamaTimerInterval,
                AttendanceSendToKarnamaTimerRecordCount = inputItem.AttendanceSendToKarnamaTimerRecordCount,
                AttendanceHookTimerRecordCount = inputItem.AttendanceHookTimerRecordCount,
                AutomaticCollectAttendanceTimerInterval = inputItem.AutomaticCollectAttendanceTimerInterval,
                KarnamaAuthorizationToken = inputItem.KarnamaAuthorizationToken,
                KarnamaServiceUrl = inputItem.KarnamaServiceUrl,




                MaxZkCommandCount = inputItem.MaxZkCommandCount,
                ZkDeadlineInMinutes = inputItem.ZkDeadlineInMinutes,
                ZkPushServerPort = inputItem.ZkPushServerPort,
                ZkPushServerIp = inputItem.ZkPushServerIp,
                MaxRetryForZkOtherCommand = inputItem.MaxRetryForZkOtherCommand,
                MaxRetryForZkUserCommands = inputItem.MaxRetryForZkUserCommands,

                MaxRetryForPwOtherCommands = inputItem.MaxRetryForPwOtherCommands,
                MaxRetryForPwUserCommands = inputItem.MaxRetryForPwUserCommands,
                PwDeadlineInMinutes = inputItem.PwDeadlineInMinutes,

                SupremaSdk1ServerPort = inputItem.SupremaSdk1ServerPort,
                SupremaSdk1ServerMaxConnection = inputItem.SupremaSdk1ServerMaxConnection,
                MaxRetryForSupremaSdk1OtherCommands = inputItem.MaxRetryForSupremaSdk1OtherCommands,
                MaxRetryForSupremaSdk1UserCommands = inputItem.MaxRetryForSupremaSdk1UserCommands,
                SupremaSdk1DeadlineInMinutes = inputItem.SupremaSdk1DeadlineInMinutes,

                SupremaSdk2ServerPort = inputItem.SupremaSdk2ServerPort,
                SupremaSdk2ServerReconnectTimerInterval = inputItem.SupremaSdk2ServerReconnectTimerInterval,
                MaxRetryForSupremaSdk2OtherCommands = inputItem.MaxRetryForSupremaSdk2OtherCommands,
                MaxRetryForSupremaSdk2UserCommands = inputItem.MaxRetryForSupremaSdk2UserCommands,
                SupremaSdk2DeadlineInMinutes = inputItem.SupremaSdk2DeadlineInMinutes,

                MaxRetryForVirdiOtherCommands = inputItem.MaxRetryForVirdiOtherCommands,
                MaxRetryForVirdiUserCommands = inputItem.MaxRetryForVirdiUserCommands,
                VirdiDeadlineInMinutes = inputItem.VirdiDeadlineInMinutes,
                VirdiServerPort = inputItem.VirdiServerPort,


            };

        }

        public static DtoSystemConfig MapSystemConfigModelToDtoSystemConfig(SystemConfigModel inputItem)
        {
            return new DtoSystemConfig
            {
                AttendanceRegisterInterval = inputItem.AttendanceRegisterInterval,
                AttendanceRegisterIntervalForParking = inputItem.AttendanceRegisterIntervalForParking,
                AttendanceRegisterIntervalForTimeAttendance = inputItem.AttendanceRegisterIntervalForTimeAttendance,
                AttendanceRegisterIntervalForAccessControl = inputItem.AttendanceRegisterIntervalForAccessControl,
                OnlineDeviceTimerInterval = inputItem.OnlineDeviceTimerInterval,
                ResetRetryCountDayBefore = inputItem.ResetRetryCountDayBefore,
                KeepCommandAfterResponse = inputItem.KeepCommandAfterResponse,
                SupremaSdk1ServerMaxConnection = inputItem.SupremaSdk1ServerMaxConnection,
                AttendanceSendToKarnamaTimerInterval = inputItem.AttendanceSendToKarnamaTimerInterval,
                AttendanceHookTimerInterval = inputItem.AttendanceHookTimerInterval,
                AttendanceSendToKarnamaTimerRecordCount = inputItem.AttendanceSendToKarnamaTimerRecordCount,
                AttendanceHookTimerRecordCount = inputItem.AttendanceHookTimerRecordCount,
                AutomaticCollectAttendanceTimerInterval = inputItem.AutomaticCollectAttendanceTimerInterval,
                KarnamaServiceUrl = inputItem.KarnamaServiceUrl,
                OnlineMonitoringDevicesIntervalFromLastDataToReset = inputItem.OnlineMonitoringDevicesIntervalFromLastDataToReset,
                OnlineMonitoringDevicesSleepAfterPingInSecond = inputItem.OnlineMonitoringDevicesSleepAfterPingInSecond,
                OnlineMonitoringDevicesPingTimeoutInMillisecond = inputItem.OnlineMonitoringDevicesPingTimeoutInMillisecond,
                OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = inputItem.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond,

                ZkPushServerIp = inputItem.ZkPushServerIp,
                MaxZkCommandCount = inputItem.MaxZkCommandCount,
                ZkPushServerPort = inputItem.ZkPushServerPort,
                MaxRetryForZkOtherCommand = inputItem.MaxRetryForZkOtherCommand,
                MaxRetryForZkUserCommands = inputItem.MaxRetryForZkUserCommands,
                ZkDeadlineInMinutes = inputItem.ZkDeadlineInMinutes,

                MaxRetryForPwOtherCommands = inputItem.MaxRetryForPwOtherCommands,
                MaxRetryForPwUserCommands = inputItem.MaxRetryForPwUserCommands,
                PwDeadlineInMinutes = inputItem.PwDeadlineInMinutes,

                SupremaSdk1ServerPort = inputItem.SupremaSdk1ServerPort,
                MaxRetryForSupremaSdk1OtherCommands = inputItem.MaxRetryForSupremaSdk1OtherCommands,
                MaxRetryForSupremaSdk1UserCommands = inputItem.MaxRetryForSupremaSdk1UserCommands,
                SupremaSdk1DeadlineInMinutes = inputItem.SupremaSdk1DeadlineInMinutes,

                SupremaSdk2ServerReconnectTimerInterval = inputItem.SupremaSdk2ServerReconnectTimerInterval,
                SupremaSdk2ServerPort = inputItem.SupremaSdk2ServerPort,
                MaxRetryForSupremaSdk2OtherCommands = inputItem.MaxRetryForSupremaSdk2OtherCommands,
                MaxRetryForSupremaSdk2UserCommands = inputItem.MaxRetryForSupremaSdk2UserCommands,
                SupremaSdk2DeadlineInMinutes = inputItem.SupremaSdk2DeadlineInMinutes,

                MaxRetryForVirdiOtherCommands = inputItem.MaxRetryForVirdiOtherCommands,
                MaxRetryForVirdiUserCommands = inputItem.MaxRetryForVirdiUserCommands,
                VirdiDeadlineInMinutes = inputItem.VirdiDeadlineInMinutes,
                VirdiServerPort = inputItem.VirdiServerPort,
            };

        }

        public static DeviceStatisticsModel MapDtoDeviceStatisticsToDeviceStatisticsModel(DtoDeviceStatistics inputItem)
        {
            return new DeviceStatisticsModel
            {
                CountOfUsers = inputItem.CountOfUsers,
                CountOfFingers = inputItem.CountOfFingers,
                CountOfFaces = inputItem.CountOfFaces,
                CountOfUnreadAttendance = inputItem.CountOfUnreadAttendance,
                IsConnected = inputItem.IsConnected,
            };
        }

        public static DtoUserAndDeviceParam MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(EmployeeAndDeviceModel inputItem)
        {
            return new DtoUserAndDeviceParam
            {
                DeviceId = MapDeviceCommunicationModelToDtoDeviceCommunication(inputItem.DeviceInfo),
                UserInfo = MapEmployeeModelToDtoEmployee(inputItem.EmployeeData),
                CommandPriority = inputItem.CommandPriority,
                CommandIdentifier = inputItem.CommandIdentifier,
            };
        }

        public static List<DtoUserAndDeviceParam> MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(List<EmployeeAndDeviceModel> inputItems)
        {
            var result = new List<DtoUserAndDeviceParam>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(new DtoUserAndDeviceParam
                {
                    DeviceId = MapDeviceCommunicationModelToDtoDeviceCommunication(model.DeviceInfo),
                    UserInfo = MapEmployeeModelToDtoEmployee(model.EmployeeData),
                    CommandPriority = model.CommandPriority,
                    CommandIdentifier = model.CommandIdentifier,
                });
            }
            return result;
        }

        public static List<EmployeeAndDeviceResultModel> MapDtoEmployeeAndDeviceResultToEmployeeAndDeviceResultModel(List<DtoUserAndDeviceResult> inputItems)
        {
            var result = new List<EmployeeAndDeviceResultModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(new EmployeeAndDeviceResultModel
                {
                    DeviceNumber = model.DeviceNumber,
                    EmployeeNumber = model.EmployeeId,
                    Result = model.Result
                });
            }
            return result;
        }

        public static DtoSelfBillInfo MapSelfBillInfoModelToDtoSelfBillInfo(SelfBillInfoModel selfBillInfoModel)
        {
            var result = new DtoSelfBillInfo
            {
                IssueDate = selfBillInfoModel.IssueDate.FromNumericDateTime(),
                FishNumber = selfBillInfoModel.FishNumber,
                EmployeeTitle = selfBillInfoModel.EmployeeTitle,
                PrinterName = selfBillInfoModel.PrinterName,
                ManualBill = selfBillInfoModel.ManualBill,
                Id = selfBillInfoModel.Id,
                DeviceNumber = selfBillInfoModel.DeviceNumber,
                PrinterIp = selfBillInfoModel.PrinterIp,
                //RestaurantTitle = selfBillInfoModel.RestaurantTitle,
                AdditionalData = selfBillInfoModel.AdditionalData,
            };
            if (selfBillInfoModel.HeaderTitles.IsCollectionNotNullOrEmpty())
            {
                result.HeaderTitles = new List<string>();
                result.HeaderTitles.AddRange(selfBillInfoModel.HeaderTitles);
            }

            if (selfBillInfoModel.FoodInfos.IsCollectionNotNullOrEmpty())
            {
                result.FoodInfos = new List<DtoSelfBillFoodInfo>();
                foreach (var item in selfBillInfoModel.FoodInfos)
                {
                    result.FoodInfos.Add(new DtoSelfBillFoodInfo
                    {
                        FoodTitle = item.FoodTitle,
                        Description = item.Description,
                        FishCount = item.FishCount,
                        FoodType = item.FoodType,
                        IsAllowed = item.IsAllowed,
                        Price = item.Price,
                        Separator = item.Separator,
                    });
                }
            }

            return result;
        }

        public static SelfPrintResultModel MapDtoSelfPrintResultToSelfPrintResultModel(DtoSelfPrintResult inputItem)
        {
            return new SelfPrintResultModel
            {
                ErrorMessage = inputItem.ErrorMessage,
                Id = inputItem.Id,
                IsSuccessful = inputItem.IsSuccessful,
                AdditionalData = inputItem.AdditionalData,
            };
        }

        public static List<DtoMetalDetectorGate> MapMetalDetectorGateModelToDtoMetalDetectorGate(List<MetalDetectorGateModel> inputItems)
        {
            var result = new List<DtoMetalDetectorGate>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(new DtoMetalDetectorGate
                {
                    DeviceId = model.DeviceId,
                    AreaNumber = model.AreaNumber,
                    ConnectionMode = model.ConnectionMode,
                    DeviceIp = model.DeviceIp,
                    Id = model.Id,
                    IsActive = model.IsActive,
                    DeviceType = model.DeviceType,
                    TcpPort = model.TcpPort,
                    Title = model.Title,
                    ZoneCount = model.ZoneCount,
                });
            }
            return result;
        }


        public static DeviceAttendanceImageModel MapDtoDeviceAttendanceImageToDeviceAttendanceImageModel(DtoDeviceAttendanceImage inputItem)
        {
            return new DeviceAttendanceImageModel
            {
                DeviceNumber = inputItem.DeviceId,
                AttendanceDateTime = inputItem.AttendanceDateTime.ToNumericDateTime(),
                Image = Convert.ToBase64String(inputItem.Image),
                UserId = inputItem.UserIdOnDevice,
            };
        }

        public static DeviceUnauthorizedAttendanceImageModel MapDtoDeviceUnauthorizedAttendanceImageToDeviceUnauthorizedAttendanceImageModel(DtoDeviceUnauthorizedAttendanceImage inputItem)
        {
            return new DeviceUnauthorizedAttendanceImageModel
            {
                DeviceNumber = inputItem.DeviceId,
                AttendanceDateTime = inputItem.AttendanceDateTime.ToNumericDateTime(),
                Image = Convert.ToBase64String(inputItem.Image),
                EmployeeNumber = inputItem.UserIdInDevice
            };
        }

        public static DtoVirdiAccessControlData MapVirdiAccessControlDataModelToDtoVirdiAccessControlData(VirdiAccessControlDataModel inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new DtoVirdiAccessControlData
            {
                AccessDataType = inputItem.AccessDataType,
                AccessGroups = new List<DtoVirdiAccessGroup>(),
                AccessTimes = new List<DtoVirdiAccessTime>(),
                Holidays = new List<DtoVirdiHolidayGroup>(),
                Timezones = new List<DtoVirdiTimeZone>()
            };
            if (inputItem.AccessGroups.IsCollectionNotNullOrEmpty())
            {
                result.AccessGroups = inputItem.AccessGroups.Select(e => new DtoVirdiAccessGroup
                {
                    AccessTimeCode = e.AccessTimeCode,
                    Code = e.Code,
                    Index = e.Index
                }).ToList();
            }
            if (inputItem.AccessTimes.IsCollectionNotNullOrEmpty())
            {
                result.AccessTimes = inputItem.AccessTimes.Select(e => new DtoVirdiAccessTime
                {
                    Code = e.Code,
                    DayOfWeekEnum = e.DayOfWeekEnum,
                    HolidayCode = e.HolidayCode,
                    TimezoneCode = e.TimezoneCode,
                }).ToList();
            }
            if (inputItem.Holidays.IsCollectionNotNullOrEmpty())
            {
                result.Holidays = inputItem.Holidays.Select(e => new DtoVirdiHolidayGroup
                {
                    Index = e.Index,
                    Day = e.Day,
                    GroupCode = e.GroupCode,
                    Month = e.Month,
                }).ToList();
            }
            if (inputItem.Timezones.IsCollectionNotNullOrEmpty())
            {
                result.Timezones = inputItem.Timezones.Select(e => new DtoVirdiTimeZone
                {
                    Index = e.Index,
                    Code = e.Code,
                    EndHour = e.EndHour,
                    EndMinute = e.EndMinute,
                    StartHour = e.StartHour,
                    StartMinute = e.StartMinute,
                }).ToList();
            }

            return result;
        }


        public static List<UserInfoDefinedOnDeviceModel> MapDtoUserInfoOnDeviceToUserInfoOnDeviceModel(List<DtoUserInfoDefinedOnDevice> inputItems)
        {
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<UserInfoDefinedOnDeviceModel>();
            }
            return inputItems.Select(ii => new UserInfoDefinedOnDeviceModel
            {
                EmployeeNumber = ii.EmployeeNumber,
                Name = ii.Name,
                Privilege = ii.Privilege,
            }).ToList();
        }



    }
}

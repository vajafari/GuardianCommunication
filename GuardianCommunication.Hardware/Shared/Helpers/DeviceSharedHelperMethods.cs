using System;
using System.Text;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.PadisController.Definition;
using GuardianCommunication.Hardware.Pw.PwConcepts;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;
using GuardianCommunication.Hardware.Timy.TimyConcepts;
using GuardianCommunication.Hardware.Zk;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.Shared.Helpers
{
    internal static class DeviceSharedHelperMethods
    {

        public static byte[] ConvertStringToArray(string val)
        {
            return Encoding.Default.GetBytes(val);
        }

        public static string ConvertArrayToString(byte[] val)
        {
            return Encoding.UTF8.GetString(val);
        }

        public static double ConvertToTimestamp(this DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static OperationResultEnumeration MapToOperationResult(int result, DtoDevice deviceInfo)
        {
            return MapToOperationResult(result, deviceInfo.ProducerNumber, deviceInfo.SdkVersion);
        }

        public static OperationResultEnumeration MapToOperationResult(int result, ProducerEnumeration producerEnum, SdkVersionEnumeration sdkVersion)
        {
            var error = OperationResultEnumeration.CommunicationStatusUnknownError;
            switch (producerEnum)
            {
                case ProducerEnumeration.Zk:
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ErrorCodes))
                    {
                        LoggingSystem.LogInfo("ZK errors", result);
                    }
                    switch (result)
                    {
                        case (int)ZkErrorEnum.Successful:
                            error = OperationResultEnumeration.CommunicationStatusSuccessful;
                            break;
                        case (int)ZkErrorEnum.ErrorCode1:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCode1;
                            break;
                        case (int)ZkErrorEnum.ErrorCode2:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCode2;
                            break;
                        case (int)ZkErrorEnum.ErrorCode101:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCode101;
                            break;
                        case (int)ZkErrorEnum.ErrorCode102:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCode102;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative1:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative1;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative3:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative3;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative5:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative5;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative6:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative6;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative7:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative7;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative8:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative8;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative10:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative10;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative100:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative100;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative102:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative102;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative103:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative103;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative201:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative201;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative307:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative307;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2001:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2001;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2002:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2002;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2003:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2003;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2004:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2004;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative2005:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative2005;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4982:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4982;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4983:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4983;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4984:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4984;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4985:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4985;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4986:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4986;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4987:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4987;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4988:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4988;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4989:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4989;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4990:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4990;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4991:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4991;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4992:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4992;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4993:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4993;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4994:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4994;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4995:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4995;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4996:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4996;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4997:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4997;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4998:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4998;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative4999:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative4999;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12001:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12001;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12002:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12002;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12003:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12003;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12004:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12004;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12005:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12005;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12006:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12006;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12007:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12007;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative12008:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative12008;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13009:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13009;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13010:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13010;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13011:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13011;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13012:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13012;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13013:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13013;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13014:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13014;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13015:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13015;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13016:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13016;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13017:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13017;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13018:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13018;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13019:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13019;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13020:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13020;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative13021:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative13021;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15001:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15001;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15002:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15002;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15003:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15003;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15100:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15100;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15101:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15101;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15102:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15102;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15103:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15103;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15104:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15104;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15105:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15105;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15106:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15106;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative15108:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative15108;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative5113:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative5113;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative5114:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative5114;
                            break;
                        case (int)ZkErrorEnum.ErrorCodeNegative5115:
                            error = OperationResultEnumeration.CommunicationStatusZkErrorCodeNegative5115;
                            break;
                        default:
                            error = OperationResultEnumeration.CommunicationStatusUnknownError;
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ErrorCodes))
                    {
                        LoggingSystem.LogInfo("Timy errors", result);
                    }
                    switch (result)
                    {
                        case (int)TimyErrorEnum.Successful:
                            error = OperationResultEnumeration.CommunicationStatusSuccessful;
                            break;
                        case (int)TimyErrorEnum.ErrorCommunication:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorCommunication;
                            break;
                        case (int)TimyErrorEnum.ErrorWriteFail:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorWriteFail;
                            break;
                        case (int)TimyErrorEnum.ErrorReadFail:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorReadFail;
                            break;
                        case (int)TimyErrorEnum.ErrorInvalidParam:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorInvalidParam;
                            break;
                        case (int)TimyErrorEnum.ErrorCarryOut:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorCarryOut;
                            break;
                        case (int)TimyErrorEnum.ErrorLogEnd:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorLogEnd;
                            break;
                        case (int)TimyErrorEnum.ErrorMemory:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorMemory;
                            break;
                        case (int)TimyErrorEnum.ErrorMultiUser:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorMultiUser;
                            break;
                        case (int)TimyErrorEnum.ErrorCustomVisibleLightImageLengthIsTooLong:
                            error = OperationResultEnumeration.CommunicationStatusTimyErrorVisibleLightImageLengthIsTooLong;
                            break;
                        default:
                            error = OperationResultEnumeration.CommunicationStatusTimyUnknownError;
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {

                        switch (sdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ErrorCodes))
                                    {
                                        LoggingSystem.LogInfo("Suprema 1 errors", result);
                                    }
                                    switch (result)
                                    {
                                        case BSSDK.BS_SUCCESS:
                                            error = OperationResultEnumeration.CommunicationStatusSuccessful;
                                            break;
                                        case BSSDK.BS_ERR_NO_AVAILABLE_CHANNEL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative100;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_COMM_HANDLE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative101;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_WRITE_CHANNEL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative102;
                                            break;
                                        case BSSDK.BS_ERR_WRITE_CHANNEL_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative103;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_READ_CHANNEL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative104;
                                            break;
                                        case BSSDK.BS_ERR_READ_CHANNEL_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative105;
                                            break;
                                        case BSSDK.BS_ERR_CHANNEL_OVERFLOW:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative106;
                                            break;
                                        case BSSDK.BS_ERR_CHANNEL_CLOSED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative107;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_INIT_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative200;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_OPEN_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative201;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_CONNECT_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative202;
                                            break;
                                        case BSSDK.BS_ERR_SOCKET_CLOSED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative203;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_OPEN_SERIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative220;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_OPEN_USB:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative240;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_USB_MEMORY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative260;
                                            break;
                                        case BSSDK.BS_ERR_NO_MORE_USB_MEMORY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative261;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_FIND_USB_MEMORY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative262;
                                            break;
                                        case BSSDK.BS_ERR_VT_EXIST_IN_MEMORY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative263;
                                            break;
                                        case BSSDK.BS_ERR_USB_MEMORY_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative264;
                                            break;
                                        case BSSDK.BS_ERR_NO_MORE_VT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative265;
                                            break;
                                        case BSSDK.BS_ERR_BUSY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative300;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative301;
                                            break;
                                        case BSSDK.BS_ERR_CHECKSUM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative302;
                                            break;
                                        case BSSDK.BS_ERR_UNSUPPORTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative303;
                                            break;
                                        case BSSDK.BS_ERR_FILE_IO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative304;
                                            break;
                                        case BSSDK.BS_ERR_DISK_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative305;
                                            break;
                                        case BSSDK.BS_ERR_NOT_FOUND:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative306;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_PARAM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative307;
                                            break;
                                        case BSSDK.BS_ERR_RTC:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative308;
                                            break;
                                        case BSSDK.BS_ERR_MEM_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative309;
                                            break;
                                        case BSSDK.BS_ERR_DB_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative310;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative311;
                                            break;
                                        case BSSDK.BS_ERR_USB_DISABLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative312;
                                            break;
                                        case BSSDK.BS_ERR_COM_DISABLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative313;
                                            break;
                                        case BSSDK.BS_ERR_WRONG_PASSWORD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative314;
                                            break;
                                        case BSSDK.BS_ERR_TRY_AGAIN:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative315;
                                            break;
                                        case BSSDK.BS_ERR_EXIST_FINGER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative316;
                                            break;
                                        case BSSDK.BS_ERR_NO_USER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative320;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_CHANGE_IMG_VIEW:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative321;
                                            break;
                                        case BSSDK.BS_ERR_NO_MORE_TERMINAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative400;
                                            break;
                                        case BSSDK.BS_ERR_TERMINAL_NOT_FOUND:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative401;
                                            break;
                                        case BSSDK.BS_ERR_TERMINAL_COMM_ERROR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative402;
                                            break;
                                        case BSSDK.BS_ERR_TERMINAL_NOT_AUTHORIZED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative403;
                                            break;
                                        case BSSDK.BS_ERR_TERMINAL_BUSY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative404;
                                            break;
                                        case BSSDK.BS_ERR_DB_NOT_EXIST:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative500;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_CONNECT_TO_DB:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative501;
                                            break;
                                        case BSSDK.BS_ERR_DB_INTERNAL_ERROR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative502;
                                            break;
                                        case BSSDK.BS_ERR_CANNOT_INIT_SSL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative601;
                                            break;
                                        case BSSDK.BS_ERR_SSL_INVALID_CTX:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative602;
                                            break;
                                        case BSSDK.BS_ERR_SSL_INVALID_CERTFILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative603;
                                            break;
                                        case BSSDK.BS_ERR_SSL_INVALID_KEYFILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative604;
                                            break;
                                        case BSSDK.BS_ERR_SSL_INVALID_CAFILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative605;
                                            break;
                                        case BSSDK.BS_ERR_SSL_INVALID_PATH:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative606;
                                            break;
                                        case BSSDK.BS_ERR_SSL_CANNOT_CONNECT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative607;
                                            break;
                                        case BSSDK.BS_ERR_INVALID_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative608;
                                            break;
                                        case BSSDK.BS_ERR_UNKNOWN:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative9999;
                                            break;
                                        default:
                                            error = OperationResultEnumeration.CommunicationStatusUnknownError;
                                            break;
                                    }
                                    break;
                                }
                            case SdkVersionEnumeration.SdkVersion2:
                                {
                                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ErrorCodes))
                                    {
                                        LoggingSystem.LogInfo("Suprema 2 errors", result);
                                    }
                                    switch ((BS2ErrorCode)result)
                                    {
                                        case BS2ErrorCode.BS_SDK_ERROR_FROM_DEVICE_DRIVER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ErrorFromDeviceDriver;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotConnectSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_LISTEN_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotListenSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ACCEPT_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotAcceptSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_WRITE_SOCKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotWriteSocket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOCKET_IS_NOT_CONNECTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SocketIsNotConnected;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOCKET_IS_NOT_OPEN:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SocketIsNotOpened;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOCKET_IS_NOT_LISTENED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SocketIsNotListened;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOCKET_IN_PROGRESS:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SocketInProgress;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_IPV4_IS_NOT_ENABLE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Ip4IsNotEnabled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_IPV6_IS_NOT_ENABLE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Ip6IsNotEnabled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_SUPPORTED_SPECIFIED_DEVICE_INFO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotSupportedSpecifiedDeviceInfo;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_ENOUGTH_BUFFER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotEnoughBuffer;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_SUPPORTED_IPV6:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Ip6IsNotEnabled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_ADDRESS:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidAddress;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidParam;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_DEVICE_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidDeviceId;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_DEVICE_TYPE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidDeviceType;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_PACKET_CHECKSUM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPacketChecksum;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_PACKET_INDEX:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPacketIndex;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_PACKET_COMMAND:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPacketCommand;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_PACKET_SEQUENCE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPacketSequence;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_CODE_SIGN:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidCodeSign;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ExtractionFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_VERIFY_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2VerifyFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_IDENTIFY_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2IdentifyFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_IDENTIFY_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2IdentifyTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FINGERPRINT_CAPTURE_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintCaptureFailed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FINGERPRINT_SCAN_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintScanTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FINGERPRINT_SCAN_CANCELLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintScanCanceled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_SAME_FINGERPRINT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotSameFingerPrint;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ExtractionLowQuality;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CaptureLowQuality;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_FINGERPRINT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindFingerPrint;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FAKE_FINGER_DETECTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FakeFingerPrintDetected;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FAKE_FINGER_TRY_AGAIN:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FakeFingerPrintTryAgain;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FAKE_FINGER_SENSOR_ERROR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FakeFingerPrintSensor;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FACE_CAPTURE_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceCaptureFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FACE_SCAN_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceScanTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FACE_SCAN_CANCELLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceScanCancelled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FACE_SCAN_FAILED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceScanFailed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_UNMASKED_FACE_DETECTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UnmaskedFaceDetected;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FAKE_FACE_DETECTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FakeFaceDetected;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ESTIMATE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotEstimate;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NORMALIZE_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NormalizeFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SMALL_DETECTION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SmallDetection;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_LARGE_DETECTION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2LargeDetection;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_BIASED_DETECTION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2BiasedDetection;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ROTATED_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2RotatedFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_OVERLAPPED_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2OverlappedFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_UNOPENED_EYES:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UnopenedFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_LOOKING_FRONT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotLookingFront;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_OCCLUDED_MOUTH:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2OccludedMouth;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_MATCH_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2MatchFailed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_DIR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenDir;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_WRITE_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotWriteFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SEEK_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSeekFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_GET_STAT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotGetStat;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_GET_SYSINFO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotGetSystemInfo;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DATA_MISMATCH:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DataMismatch;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ALREADY_OPEN_DIR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AlreadyOpenedDir;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_RELAY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidRelay;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_WRITE_IO_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotWriteIoPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_IO_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadIoPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_INPUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadInput;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_READ_INPUT_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ReadInputTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ENABLE_INPUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotEnableInput;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SET_INPUT_DURATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSetInputDuration;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_PORT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPort;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_INTERPHONE_TYPE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidInterPhoneType;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_LCD_PARAM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidLcdParams;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_WRITE_LCD_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotWriteLcdPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_LCD_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadLcdPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_LCD_PACKET:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidLcdPacket;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INPUT_QUEUE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InputQueueFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_WIEGAND_QUEUE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2WiegandQueueFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_MISC_INPUT_QUEUE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2MISCInputQueueFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_WIEGAND_DATA_QUEUE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2WiegandDataQueueFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_WIEGAND_DATA_QUEUE_EMPTY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2WiegandDataQueueEmpty;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_SUPPORTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SdkNotSupported;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Timeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SET_TIME:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSetTime;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_DATA_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidDataFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TOO_LARGE_DATA_FOR_SLOT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ToLargeDataForSlot;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_SLOT_NO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidSlotNumber;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_SLOT_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidSlotData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_INIT_DB:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotInitDb;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUPLICATE_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DuplicateId;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UserFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUPLICATE_TEMPLATE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DuplicateTemplate;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FINGERPRINT_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUPLICATE_CARD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DuplicateCard;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_VALID_HDR_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotValidHdrFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_LOG_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidLogFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_USER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindUser;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ACCESS_LEVEL_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AccessLevelFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_USER_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidUserId;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_BLACKLIST_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2BlacklistFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_NAME_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UsernameFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_IMAGE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UserImageFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_IMAGE_SIZE_TOO_BIG:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UserImageIsTooBig;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SLOT_DATA_CHECKSUM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SlotDataCheckSum;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_UPDATE_FINGERPRINT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotUpdateFingerPrint;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TEMPLATE_FORMAT_MISMATCH:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2TemplateFormatMismatch;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_ADMIN_USER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotAdminUser;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_LOG:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindLog;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DOOR_SCHEDULE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DoorScheduleFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DB_SLOT_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DbSlotFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ACCESS_GROUP_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AccessGroupFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FLOOR_LEVEL_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FloorLevelFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ACCESS_SCHEDULE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AccessScheduleFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HOLIDAY_GROUP_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HolidayGroupFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HOLIDAY_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HolidayFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TIME_PERIOD_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2TimePeriodFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_BIOMETRIC_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoBiometricCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_CARD_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoCardCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_PIN_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoPinCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_BIOMETRIC_PIN_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoBiometricPinCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_USER_NAME:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoUsername;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_USER_IMAGE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoUserImage;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_READER_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ReaderFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CACHE_MISSED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CacheMissed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_OPERATOR_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2OperatorFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_LINK_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidLinkId;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TIMER_CANCELED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2TimerCanceled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_JOB_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UserJobFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_UPDATE_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotUpdateFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FACE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FLOOR_SCHEDULE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FloorScheduleFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_AUTH_GROUP:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindAuthGroup;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_GROUP_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthGroupFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_PHRASE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UserPhraseFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DST_SCHEDULE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DstPhraseFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_DST_SCHEDULE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindDstModule;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_SCHEDULE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidSchedule;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_OPERATOR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindOperator;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUPLICATE_FINGERPRINT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DuplicateFingerPrint;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUPLICATE_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DuplicateFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_FACE_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotFaceCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_FINGERPRINT_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoFingerPrintCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_FACE_PIN_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoFacePinCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NO_FINGERPRINT_PIN_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NoFingerPrintPinCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_USER_IMAGE_EX_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2userImageExFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_CONFIG:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidConfig;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_CONFIG_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenConfigFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_READ_CONFIG_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReadConfigFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_CONFIG_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidConfigFile;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_CONFIG_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidConfigData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_WRITE_CONFIG_FILE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotWriteConfigData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_CONFIG_INDEX:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidConfigIndex;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SCAN_FINGER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotScanFinger;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SCAN_CARD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotScanCard;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_RTC:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenRtc;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SET_RTC:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSetRtc;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_GET_RTC:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotGetRtc;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SET_LED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSetLed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_DEVICE_DRIVER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotOpenDeviceDriver;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_DEVICE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindDevice;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SCAN_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotScanFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SLAVE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SlaveFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ADD_DEVICE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotAddDevice;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_DOOR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindDoor;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DOOR_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DoorFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_LOCK_DOOR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotLockDoor;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_UNLOCK_DOOR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotUnlockDoor;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_RELEASE_DOOR:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotReleaseDoor;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_LIFT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindLift;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_LIFT_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2LiftFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ACCESS_RULE_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AccessRuleViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DISABLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SdkErrorDisabled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NOT_YET_VALID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NotYetValid;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_EXPIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Expired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_BLACKLIST:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Blacklist;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_ACCESS_GROUP:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindAccessGroup;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_ACCESS_LEVEL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindAccessLevel;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_ACCESS_SCHEDULE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindAccessSchedule;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_HOLIDAY_GROUP:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindHolidayGroup;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_BLACKLIST:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindBlackList;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUAL_AUTH_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DualAuthTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_AUTH_MODE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidAuthMode;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_UNEXPECTED_USER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthUnexpectedUser;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_UNEXPECTED_CREDENTIAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthUnexpectedCredential;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DUAL_AUTH_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DualAuthFailed;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_BIOMETRIC_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2BiometricAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_PIN_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2PinAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_BIOMETRIC_OR_PIN_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2BiometricOrPinAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TNA_CODE_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2TnaCodeRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_SERVER_MATCH_REFUSAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthServerMatchRefusal;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_FLOOR_LEVEL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindFloorLevel;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_GROUP_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthGroupRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_IDENTIFICATION_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2IdentificationRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ANTI_TAILGATE_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AntiTailgateViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HIGH_TEMPERATURE_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HighTemperatureViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_MEASURE_TEMPERATURE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotMeasureTemperature;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_UNMASKED_FACE_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UnmaskedFaceViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_MASK_CHECK_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2MaskCheckRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_THERMAL_CHECK_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2TerminalCheckRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_FACE_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_FINGERPRINT_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_FACE_OR_PIN_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FaceOrOrPinAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_FINGERPRINT_OR_PIN_AUTH_REQUIRED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FingerPrintOrPinAuthRequired;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_ZONE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindZone;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HARD_APB_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HardApbViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SoftApbViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HARD_TIMED_APB_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HardTimedApbViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SoftTimedApbViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SCHEDULED_LOCK_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ScheduledLockViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_TIMED_APB_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ApbZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FIRE_ALARM_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FireAlarmZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SCHEDULED_LOCK_UNLOCK_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ScheduledLockUnlockZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INACTIVE_ZONE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InactiveZone;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INTRUSION_ALARM_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2IntrusionAlarmZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ARM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotArm;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_DISARM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotDisarm;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_ARM_CARD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindArmCard;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_COUNT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HardEntranceLimitCountViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_COUNT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SoftEntranceLimitCountViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_TIME_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HardEntranceLimitTimeViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_TIME_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SoftEntranceLimitTimeViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INTERLOCK_ZONE_DOOR_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InterlockZoneDoorViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INTERLOCK_ZONE_INPUT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2IntervalZoneInputViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INTERLOCK_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InterlockZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_LIMIT_SCHEDULE_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthLimitScheduleViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_LIMIT_COUNT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthLimitCountViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_AUTH_LIMIT_USER_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AuthLimitUserViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_AUTH_LIMIT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SoftAuthLimitViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_HARD_AUTH_LIMIT_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2HardAuthLimitViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_LIFT_LOCK_UNLOCK_ZONE_FULL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2LiftLockUnlockZoneFull;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_LIFT_LOCK_VIOLATION:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2LiftLockViolation;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_IO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardIo;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_INIT_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardInitFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_NOT_ACTIVATED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardNotActivated;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_CANNOT_READ_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardCannotReadData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_CIS_CRC:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardCisCrc;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_CANNOT_WRITE_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardCannotWriteData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_READ_TIMEOUT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardReadTimeout;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_READ_CANCELLED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardReadCanceled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CARD_CANNOT_SEND_DATA:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CardCannotSendData;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_CARD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotFindCard;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_PASSWORD:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidPassword;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CAMERA_INIT_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CameraInitFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_JPEG_ENCODER_INIT_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2JpegEncoderInitFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_ENCODE_JPEG:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotEncodeJpeg;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_JPEG_ENCODER_NOT_INITIALIZED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2JpegEncoderNotInitialized;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_JPEG_ENCODER_DEINIT_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2JpegEncoderUninitFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CAMERA_CAPTURE_FAIL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CameraCaptureFail;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_DETECT_FACE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotDetectFace;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_FILE_IO:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2FileIo;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ALLOC_MEM:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2AllocMemory;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_UPGRADE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotUpgrade;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DEVICE_LOCKED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DeviceLocked;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_SEND_TO_SERVER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotSendToServer;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_INIT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslInit;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_NOT_SUPPORTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslNotSupported;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_CANNOT_CONNECT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslCannotConnect;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_ALREADY_CONNECTED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslAlreadyConnected;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_INVALID_CERT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslInvalidCertification;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_VERIFY_CERT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslVerifyCertification;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_INVALID_KEY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslInvalidKey;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_SSL_VERIFY_KEY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2SslVerifyKey;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_MOBILE_PORTAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2MobilePortal;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_NULL_POINTER:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2NullPointer;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_UNINITIALIZED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2ErrorUninitilized;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANNOT_RUN_SERVICE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2CannotRunService;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_CANCELED:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Canceled;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_EXIST:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Exist;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_ENCRYPT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Encrypt;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DECRYPT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Decrypt;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_DEVICE_BUSY:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2DeviceBusy;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INTERNAL:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2Internal;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_FILE_FORMAT:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidFileFormat;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_INVALID_SCHEDULE_ID:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2InvalidScheduleId;
                                            break;
                                        case BS2ErrorCode.BS_SDK_ERROR_UNKNOWN_FINGER_TEMPLATE:
                                            error = OperationResultEnumeration.CommunicationStatusSupremaSdk2UnknownFingerTemplate;
                                            break;
                                        default:
                                            error = OperationResultEnumeration.CommunicationStatusUnknownError;
                                            break;
                                    }
                                }
                                break;

                        }
                    }
                    break;
            }
            return error;
        }

        public static DateTime GetEndDate(DateTime? endDate, ProducerEnumeration producerEnum, SdkVersionEnumeration sdkVersion)
        {
            switch (producerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        var newEndDate = new DateTime(2029, 01, 01);
                        if (endDate.HasValue && endDate.Value.Date < newEndDate)
                        {
                            newEndDate = endDate.Value;
                        }
                        return newEndDate;
                    }
                case ProducerEnumeration.Zk:
                    {
                        var newEndDate = new DateTime(2029, 01, 01);
                        if (endDate.HasValue && endDate.Value.Date < newEndDate)
                        {
                            newEndDate = endDate.Value;
                        }
                        return newEndDate;
                    }
                case ProducerEnumeration.Virdi:
                    return endDate.HasValue ? endDate.Value.Date.AddDays(1) : DateTime.Now.AddYears(20);
                default:
                    return endDate ?? DateTime.Now.AddYears(20);
            }
        }



        private static DateTime UnixBaseDate = new DateTime
            (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertUnixTimestampToServerLocalTime
            (
                uint timestamp,
                DtoCommunicationDeviceTimeSettings timeSetting
            )
        {
            if (timeSetting == null)
            {
                // No timezone/DST settings available: return the UTC-based time without adjustments
                // instead of throwing an NRE (which would drop the attendance record).
                return UnixBaseDate.AddSeconds(timestamp);
            }
            var result = UnixBaseDate.AddSeconds(timestamp).AddSeconds
                (DateTimeHelper.ConvertToTimeZoneTotalSecond(timeSetting.TimeZone));
            if (timeSetting.IsDaylightActive)
            {
                if (result >= timeSetting.CurrentYearDaylightStart && result <= timeSetting.CurrentYearDaylightEnd)
                {
                    result = result.AddSeconds(timeSetting.DaylightChangeTimeInSeconds);
                }
            }
            return result;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            var diff = date.ToUniversalTime() - UnixBaseDate.ToUniversalTime();
            return Math.Floor(diff.TotalSeconds);
        }


        public static double ConvertToUnixTimestampAndConsiderDateAsUtc(DateTime date)
        {
            var currDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
            return currDate.Subtract(UnixBaseDate).TotalSeconds;
        }



    }
}

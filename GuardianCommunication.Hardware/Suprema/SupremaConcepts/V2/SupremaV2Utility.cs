using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GuardianCommunication.Hardware.Shared.Helpers;

namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2
{
    public class SupremaV2Utility
    {
        public static bool IsUserDefinedInDevice(BS2UserFaceExBlob userBlob)
        {
            return Encoding.ASCII.GetString(userBlob.user.userID).TrimEnd('\0').IsNotNullOrEmpty();
        }

        public static bool IsUserDefinedInDevice(BS2UserBlobEx userBlob)
        {
            return Encoding.ASCII.GetString(userBlob.user.userID).TrimEnd('\0').IsNotNullOrEmpty();
        }

        public static bool IsUserDefinedInDevice(BS2UserBlob userBlob)
        {
            return Encoding.ASCII.GetString(userBlob.user.userID).TrimEnd('\0').IsNotNullOrEmpty();
        }

        public static long GetUserId(byte[] userIfBytes)
        {
            var userIdString = Encoding.ASCII.GetString(userIfBytes).TrimEnd('\0');
            var digits = new string(userIdString.TakeWhile(char.IsDigit).ToArray());
            return digits.ToInt64();
        }

        public static long GetUserIdOrDefault(byte[] userIfBytes)
        {
            var userIdString = Encoding.ASCII.GetString(userIfBytes).TrimEnd('\0');
            var digits = new string(userIdString.TakeWhile(char.IsDigit).ToArray());
            return digits.TryConvertToInt64(0);
        }

        public static DtoAttendance ConvertBs2EventToDtoAttendance(BS2Event record, int deviceNumber, DtoCommunicationDeviceTimeSettings timeSetting, DeviceAttendanceIoRetrieveTypeEnumeration retrieveType)
        {
            return new DtoAttendance
            {
                Id = (int)record.id,
                EmployeeNumber = Encoding.ASCII.GetString(record.userID).ToInt64(),
                AttendanceDateTime = DeviceSharedHelperMethods.ConvertUnixTimestampToServerLocalTime(record.dateTime, timeSetting),
                DeviceNumber = deviceNumber,
                CameraId = null,
                VerificationStyle = (int)GetVerificationStyle(record.code),
                AttendanceSource = AttendanceSourceEnumeration.Device,
                DeviceAttendanceIoRetrieveType = retrieveType,
                IsInvalid = false,
                IsSent = false,
                StatusCode = record.param,
                RfCardNumber = null,
            };
        }

        public static DtoDeviceEventLog ConvertBs2EventToDtoDeviceEventLog(BS2Event record, int deviceNumber, DtoCommunicationDeviceTimeSettings timeSetting)
        {
            //Convert.ToBoolean(eventLog.param) ? "Device" : "Server"
            return new DtoDeviceEventLog
            {
                Id = (int)record.id,
                EmployeeNumber = GetUserIdOrDefault(record.userID),
                EventDateTime = DeviceSharedHelperMethods.ConvertUnixTimestampToServerLocalTime(record.dateTime, timeSetting),
                DeviceNumber = deviceNumber,
                EventCode = record.code,
                SdkVersion = SdkVersionEnumeration.SdkVersion2,
                IsFromDevice = Convert.ToBoolean(record.param),
                Producer = ProducerEnumeration.Suprema
            };
        }

        public static SupremaSdk2EventTypeEnumeration GetEventType(int code)
        {
            if (code >= (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS &&
                code <= (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE_PIN)
            {
                return SupremaSdk2EventTypeEnumeration.VerifySuccess;
            }
            if (code >= (ushort)BS2EventCodeEnum.VERIFY_SUCCESS &&
                code <= (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_AOC_FINGER_PIN)
            {
                return SupremaSdk2EventTypeEnumeration.VerifySuccess;
            }

            if (code >= (ushort)BS2EventCodeEnum.VERIFY_FAIL &&
                code <= (ushort)BS2EventCodeEnum.VERIFY_FAIL_AOC_FINGER)
            {
                return SupremaSdk2EventTypeEnumeration.VerifyFailed;
            }
            if (code >= (ushort)BS2EventCodeEnum.IDENTIFY_FAIL &&
                code <= (ushort)BS2EventCodeEnum.IDENTIFY_FAIL_AOC_FINGER)
            {
                return SupremaSdk2EventTypeEnumeration.VerifyFailed;
            }
            if (code >= (ushort)BS2EventCodeEnum.VERIFY_DURESS &&
                code <= (ushort)BS2EventCodeEnum.VERIFY_DURESS_AOC_FINGER_PIN)
            {
                return SupremaSdk2EventTypeEnumeration.DuressVerify;
            }
            if (code >= (ushort)BS2EventCodeEnum.IDENTIFY_DURESS &&
                code <= (ushort)BS2EventCodeEnum.IDENTIFY_DURESS_FACE_PIN)
            {
                return SupremaSdk2EventTypeEnumeration.DuressVerify;
            }
            if (code == (ushort)BS2EventCodeEnum.USER_UPDATE_SUCCESS ||
                code == (ushort)BS2EventCodeEnum.USER_ENROLL_SUCCESS ||
                code == (ushort)BS2EventCodeEnum.USER_UPDATE_PARTIAL_SUCCESS)
            {
                return SupremaSdk2EventTypeEnumeration.UserChanged;
            }

            return SupremaSdk2EventTypeEnumeration.OtherEvents;
        }

        public static AttendanceVerificationStyleEnumeration GetVerificationStyle(int code)
        {
            switch (code)
            {
                case (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS:
                    return AttendanceVerificationStyleEnumeration.Unknown;
                case (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FINGER:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FINGER_PIN:
                    return AttendanceVerificationStyleEnumeration.FingerAndPassword;
                case (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE:
                    return AttendanceVerificationStyleEnumeration.Face;
                case (ushort)BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE_PIN:
                    return AttendanceVerificationStyleEnumeration.FaceAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS:
                    return AttendanceVerificationStyleEnumeration.Unknown;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_PIN:
                    return AttendanceVerificationStyleEnumeration.IdAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_FINGER:
                    return AttendanceVerificationStyleEnumeration.IdAndFinger;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_FINGER_PIN:
                    return AttendanceVerificationStyleEnumeration.IdAndFingerAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_FACE:
                    return AttendanceVerificationStyleEnumeration.IdAndFace;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_ID_FACE_PIN:
                    return AttendanceVerificationStyleEnumeration.IdAndFaceAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD:
                    return AttendanceVerificationStyleEnumeration.Card;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD_PIN:
                    return AttendanceVerificationStyleEnumeration.CardAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FINGER:
                    return AttendanceVerificationStyleEnumeration.CardAndFinger;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FINGER_PIN:
                    return AttendanceVerificationStyleEnumeration.CardAndFingerAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FACE:
                    return AttendanceVerificationStyleEnumeration.CardAndFace;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FACE_PIN:
                    return AttendanceVerificationStyleEnumeration.CardAndFaceAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_AOC:
                    return AttendanceVerificationStyleEnumeration.Unknown;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_AOC_PIN:
                    return AttendanceVerificationStyleEnumeration.IdAndPassword;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_AOC_FINGER:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case (ushort)BS2EventCodeEnum.VERIFY_SUCCESS_AOC_FINGER_PIN:
                    return AttendanceVerificationStyleEnumeration.FingerAndPassword;
                default:
                    return AttendanceVerificationStyleEnumeration.Unknown;
            }

        }

        public static T BytesToStruct<T>(ref byte[] source)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(source, 0, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, structType);
            Marshal.FreeHGlobal(buffer);

            return instance;
        }

        public static T BytesToStruct<T>(ref byte[] source, int startIndex)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(source, startIndex, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, structType);
            Marshal.FreeHGlobal(buffer);

            return instance;
        }


        public static byte[] StructToBytes<T>(ref T source)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.StructureToPtr(source, buffer, true);
            byte[] output = new byte[structSize];
            Marshal.Copy(buffer, output, 0, structSize);
            Marshal.FreeHGlobal(buffer);

            return output;
        }

        public static void TranslatePrimitive<TSource, TOutput>(ref TSource src, ref TOutput output)
        {
            Type typeSrc = typeof(TSource);
            Type typeOut = typeof(TOutput);

            TypedReference trSrc = __makeref(src);
            TypedReference trOut = __makeref(output);
            FieldInfo[] srcInfos = typeSrc.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo[] outInfos = typeOut.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo srcInfo in srcInfos)
            {
                IEnumerable<FieldInfo> matchs = outInfos.Where(x => x.Name == srcInfo.Name && x.FieldType == srcInfo.FieldType);
                foreach (FieldInfo outInfo in matchs)
                {
                    outInfo.SetValueDirect(trOut, srcInfo.GetValueDirect(trSrc));
                    break;
                }
            }
        }

        public delegate int FUNC_BS2_GetAll(IntPtr context, UInt32 deviceId, out IntPtr obj, out UInt32 numItem);
        public static BS2ErrorCode CSP_BS2_GetAll<CSP_T, CXX_T>(IntPtr context, UInt32 deviceId, out CSP_T[] ItemsObj, out UInt32 numItem, FUNC_BS2_GetAll func)
        {
            IntPtr _itemsObj = IntPtr.Zero;
            UInt32 _itemNum = 0;
            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, out _itemsObj, out _itemNum);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                numItem = 0;
                ItemsObj = AllocateStructureArray<CSP_T>(0);
                return result;
            }

            CSP_T[] _items = AllocateStructureArray<CSP_T>((int)_itemNum);

            Translator<CXX_T, CSP_T> transItem = new Translator<CXX_T, CSP_T>();
            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr curItemObj = _itemsObj;
            for (int idx = 0; idx < _itemNum; ++idx)
            {
                CXX_T item = (CXX_T)Marshal.PtrToStructure(curItemObj, typeof(CXX_T));
                transItem.Translate(ref item, ref _items[idx]);
                curItemObj += ItemSize;
            }

            ApiV2.BS2_ReleaseObject(_itemsObj);

            numItem = _itemNum;
            ItemsObj = _items;
            return result;
        }

        public delegate int FUNC_BS2_GetItems(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount, out IntPtr obj, out UInt32 numItem);
        public static BS2ErrorCode CSP_BS2_GetItems<CSP_ID_T, CSP_T, CXX_ID_T, CXX_T>(IntPtr context, UInt32 deviceId, CSP_ID_T[] Ids, UInt32 idCount, out CSP_T[] ItemsObj, out UInt32 numItem, FUNC_BS2_GetItems func)
        {
            Translator<CSP_ID_T, CXX_ID_T> transID = new Translator<CSP_ID_T, CXX_ID_T>();
            CXX_ID_T Id = SupremaV2Utility.AllocateStructure<CXX_ID_T>();

            int IdSize = Marshal.SizeOf(typeof(CXX_ID_T));
            IntPtr _IDObj = Marshal.AllocHGlobal(IdSize * (int)idCount);
            IntPtr _curIDObj = _IDObj;
            for (int idx = 0; idx < idCount; ++idx)
            {
                transID.Translate(ref Ids[idx], ref Id);
                Marshal.StructureToPtr(Id, _curIDObj, true);
                _curIDObj += IdSize;
            }

            IntPtr _itemsObj = IntPtr.Zero;
            UInt32 _itemNum = 0;
            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _IDObj, idCount, out _itemsObj, out _itemNum);
            Marshal.FreeHGlobal(_IDObj);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                numItem = 0;
                ItemsObj = new CSP_T[0];
                return result;
            }

            CSP_T[] _items = AllocateStructureArray<CSP_T>((int)_itemNum);

            Translator<CXX_T, CSP_T> transItem = new Translator<CXX_T, CSP_T>();
            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr curItemObj = _itemsObj;
            for (int idx = 0; idx < _itemNum; ++idx)
            {
                CXX_T item = (CXX_T)Marshal.PtrToStructure(curItemObj, typeof(CXX_T));
                transItem.Translate(ref item, ref _items[idx]);
                curItemObj += ItemSize;
            }

            ApiV2.BS2_ReleaseObject(_itemsObj);

            numItem = _itemNum;
            ItemsObj = _items;
            return result;
        }

        public delegate int FUNC_BS2_RemoveItems(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount);
        public static BS2ErrorCode CSP_BS2_RemoveItems<CSP_ID_T, CXX_ID_T>(IntPtr context, UInt32 deviceId, CSP_ID_T[] Ids, UInt32 idCount, FUNC_BS2_RemoveItems func)
        {
            Translator<CSP_ID_T, CXX_ID_T> transID = new Translator<CSP_ID_T, CXX_ID_T>();
            CXX_ID_T Id = SupremaV2Utility.AllocateStructure<CXX_ID_T>();

            int IdSize = Marshal.SizeOf(typeof(CXX_ID_T));
            IntPtr _IDObj = Marshal.AllocHGlobal(IdSize * (int)idCount);
            IntPtr _curIDObj = _IDObj;
            for (int idx = 0; idx < idCount; ++idx)
            {
                transID.Translate(ref Ids[idx], ref Id);
                Marshal.StructureToPtr(Id, _curIDObj, true);
                _curIDObj += IdSize;
            }

            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _IDObj, idCount);
            Marshal.FreeHGlobal(_IDObj);
            return result;
        }


        public delegate int FUNC_BS2_SetItems(IntPtr context, UInt32 deviceId, IntPtr ItemsObj, UInt32 ItemCount);
        public static BS2ErrorCode CSP_BS2_SetItems<CSP_T, CXX_T>(IntPtr context, UInt32 deviceId, CSP_T[] ItemsObj, UInt32 ItemCount, FUNC_BS2_SetItems func)
        {
            Translator<CSP_T, CXX_T> transItem = new Translator<CSP_T, CXX_T>();
            CXX_T Item = SupremaV2Utility.AllocateStructure<CXX_T>();

            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr _ItemsObj = Marshal.AllocHGlobal(ItemSize * (int)ItemCount);
            IntPtr _curItemObj = _ItemsObj;
            for (int idx = 0; idx < ItemCount; ++idx)
            {
                transItem.Translate(ref ItemsObj[idx], ref Item);
                Marshal.StructureToPtr(Item, _curItemObj, false);
                _curItemObj += ItemSize;
            }

            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _ItemsObj, ItemCount);
            Marshal.FreeHGlobal(_ItemsObj);
            return result;
        }


        public static T AllocateStructure<T>()
        {
            int structSize = Marshal.SizeOf(typeof(T));
            byte[] empty = new byte[structSize];
            Array.Clear(empty, 0, empty.Length);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(empty, 0, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return instance;

        }

        public static T[] AllocateStructureArray<T>(int count)
        {
            T[] result = new T[count];
            int structSize = Marshal.SizeOf(typeof(T));
            byte[] empty = new byte[structSize * count];
            Array.Clear(empty, 0, empty.Length);
            IntPtr buffer = Marshal.AllocHGlobal(structSize * count);
            IntPtr curBuffer = buffer;
            Marshal.Copy(empty, 0, buffer, structSize * count);
            for (int idx = 0; idx < count; idx++)
            {
                result[idx] = (T)Marshal.PtrToStructure(curBuffer, typeof(T));
                curBuffer = (IntPtr)((long)curBuffer + structSize);
            }

            Marshal.FreeHGlobal(buffer);
            return result;

        }

        public static byte[] StringToByte(int allocSize, string source)
        {
            byte[] result = new byte[allocSize];
            Array.Clear(result, 0, result.Length);
            byte[] sourceByte = Encoding.UTF8.GetBytes(source);
            int copySize = Math.Min(allocSize, sourceByte.Length);
            Buffer.BlockCopy(sourceByte, 0, result, 0, copySize);
            return result;
        }

        public static T ConvertTo<T>(byte[] src)
        {
            if (src.Length < Marshal.SizeOf(typeof(T)))
            {
                throw new ArgumentException("array size is less than object size", "src");
            }

            IntPtr buffer = Marshal.AllocHGlobal(src.Length);
            Marshal.Copy(src, 0, buffer, src.Length);
            T item = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return item;
        }

        public static byte[] ConvertTo<T>(T instance)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(instance, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        //public static double ConvertToUnixTimestamp(DateTime date)
        //{
        // DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        // TimeSpan diff = date.ToUniversalTime() + (TimeZoneInfo.Local.IsDaylightSavingTime(date) ? TimeZoneInfo.Local.BaseUtcOffset : TimeSpan.Zero) - origin.ToUniversalTime();
        // return Math.Floor(diff.TotalSeconds);
        //}


        public static bool LoadBinary(string filePath, out IntPtr binaryData, out UInt32 binaryDataLen)
        {
            bool handled = false;
            FileStream fs = null;

            binaryData = IntPtr.Zero;
            binaryDataLen = 0;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                int fileSize = (int)fs.Length;
                int totalReadCount = 0;
                byte[] readBuffer = new byte[fileSize];

                while (totalReadCount < fileSize)
                {
                    int readCount = fs.Read(readBuffer, totalReadCount, (fileSize - totalReadCount));
                    if (readCount > 0)
                    {
                        totalReadCount += readCount;
                    }
                    else
                    {
                        Console.WriteLine("I/O error occurred while reading firmware file.");
                        break;
                    }
                }

                if (totalReadCount == fileSize)
                {
                    binaryData = Marshal.AllocHGlobal(fileSize);
                    Marshal.Copy(readBuffer, 0, binaryData, fileSize);
                    binaryDataLen = (UInt32)fileSize;
                    handled = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", filePath, e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return handled;
        }


        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);



        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);

    }
}

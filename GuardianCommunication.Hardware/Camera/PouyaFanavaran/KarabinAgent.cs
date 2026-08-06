using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;
using MD.PersianDateTime;
using RestSharp;

namespace GuardianCommunication.Hardware.Camera.PouyaFanavaran
{
    public class KarabinAgent : IDisposable
    {
        private readonly string _accessFilePath;
        private readonly string _tempFilePath;
        private readonly string _cameraBaseAddress;
        private readonly KarabinAgentConfig _config;
        public DtoCamera Camera { get; }

        private static readonly Dictionary<int, string> NationalNumberMapFromNumberToString = new Dictionary<int, string>
        {
            { 0, "الف".ToPersianCharacter() },
            { 1 , "ب"},
            { 2 , "ج"},
            { 3 , "د"},
            { 4 , "س"},
            { 5 , "ص"},
            { 6 , "ط"},
            { 7 , "ق"},
            { 8 , "ل"},
            { 9 , "م"},
            { 10, "ن" },
            { 11, "و" },
            { 12, "ه" },
            { 13, "ی".ToPersianCharacter() },
            { 14, "ع" },
            { 15, "ت" },
            { 16, "جانباز".ToPersianCharacter() },
            { 17, "پ"},
            { 18, "D"},
            { 19, "S"},
            { 20, "گ"},
            { 21, "ز"},
            { 22, "ش"},
            { 23, "ف"},
            { 24, "ک".ToPersianCharacter() },
            { 25, "ث" },
        };

        private static readonly Dictionary<string, int> NationalNumberMapFromStringToNumber = new Dictionary<string, int>
        {
            { "الف".ToPersianCharacter(), 0 },
            { "ب", 1 },
            { "ج", 2 },
            { "د", 3 },
            { "س", 4 },
            { "ص", 5 },
            { "ط", 6 },
            { "ق", 7 },
            { "ل", 8 },
            { "م", 9 },
            { "ن", 10 },
            { "و", 11 },
            { "ه", 12 },
            { "ی".ToPersianCharacter(), 13 },
            { "ع", 14 },
            { "ت", 15 },
            { "جانباز".ToPersianCharacter(), 16 },
            { "پ", 17 },
            { "D", 18 },
            { "S", 19 },
            { "گ", 20 },
            { "ز", 21 },
            { "ش", 22 },
            { "ف", 23 },
            { "ک".ToPersianCharacter(), 24 },
            { "ث", 25 },
        };



        public KarabinAgent(KarabinAgentConfig config, DtoCamera camera)
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgent))
            {
                LoggingSystem.LogInfo("New Karabin with controller agent created",
                    new { Config = config, Camera = camera });
            }


            Camera = camera;
            _accessFilePath = Path.Combine(config.AccessFileBasePath, camera.Id.ToString(), "access.txt");
            _tempFilePath = Path.Combine(config.AccessFileBasePath, camera.Id.ToString(), "temp.txt");
            _cameraBaseAddress = $"http://{Camera.IPAddress}";
            _config = config;
        }


        #region Permissions

        private readonly object _sendAccessDataLock = new object();
        public void SendAccessFiles(List<DtoCarAccessData> carAccessData)
        {
            if (!Camera.CameraSettings.HasFlag(CameraSettingEnumeration.SendValidList))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            if (Monitor.TryEnter(_sendAccessDataLock))
            {
                try
                {
                    if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                    {
                        LoggingSystem.LogInfo("SendAccessFiles send access log started", new
                        {
                            CarAccessData = carAccessData,
                            Camera,
                        });
                    }
                    if (CreatePermissionFileIfRequired(carAccessData))
                    {
                        SendPermissionFile();
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on CreateFileTimerElapsed ", Camera);
                }
                finally
                {
                    Monitor.Exit(_sendAccessDataLock);
                }
            }
            else
            {
                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                {
                    LoggingSystem.LogInfo("SendAccessFiles skipped because of lock is taken", Camera);
                }
            }
        }


        private static bool TryConvertPlateToKarabinNumericFormat(string plateString, CarPlateTypeEnumeration plateType, out int plateNumber)
        {
            switch (plateType)
            {
                case CarPlateTypeEnumeration.National:
                    if (PlateHelper.TryParsNationalPlate(plateString, out var plateObject))
                    {
                        if (NationalNumberMapFromStringToNumber.TryGetValue(plateObject.Letter.Trim(), out var numericValueOfLetter))
                        {
                            plateNumber = $"{plateObject.FirstNumber}{numericValueOfLetter:D2}{plateObject.SecondNumber}{plateObject.IranSerial}"
                                .ToInt32();
                            return true;
                        }
                    }
                    break;
            }

            plateNumber = 0;
            return false;
        }

        private static long ConvertDateTimeToEpochTime(
            DateTime time,
            TimeResolution resolution = TimeResolution.MilliSecond)
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var dateTime1 = dateTimeOffset.DateTime;
            dateTimeOffset = DateTimeOffset.UtcNow;
            var dateTime2 = dateTimeOffset.DateTime;
            var timeSpan = dateTime1 - dateTime2;
            time = time.Subtract(timeSpan);
            var dateTime3 = new DateTime(1970, 1, 1);
            switch (resolution)
            {
                case TimeResolution.Second:
                    return (long)time.Subtract(dateTime3).TotalSeconds;
                case TimeResolution.MilliSecond:
                    return (long)time.Subtract(dateTime3).TotalMilliseconds;
                case TimeResolution.MicroSecond:
                    return (long)(time.Subtract(dateTime3).TotalMilliseconds * 1000.0);
                default:
                    return (long)time.Subtract(dateTime3).TotalMilliseconds;
            }
        }

        private bool CreatePermissionFileIfRequired(List<DtoCarAccessData> accessData)
        {
            var result = false;
            try
            {
                var tempResult = new List<Permission>();
                foreach (var item in accessData)
                {
                    if (!TryConvertPlateToKarabinNumericFormat(item.PlateString, item.PlateType, out var plateNumber))
                    {
                        if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                        {
                            LoggingSystem.LogWarning(
                                ObjectHelper.SerializeAsJsonFormatted(new { Camera, item.PlateString, item.PlateType }),
                                "Invalid plate detected in CreatePermissionFileIfRequired");
                        }

                        continue;
                    }
                    var startDate = new DateTime(2024, 01, 01);
                    var endDate = startDate.AddYears(30);
                    var permissionString = string.Empty.PadLeft(168, '2');
                    tempResult.Add(new Permission(plateNumber, startDate, endDate, permissionString));
                }
                var sortedArray = tempResult.ToArray();
                Array.Sort(sortedArray);

                var version = 1;
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);

                Directory.CreateDirectory(Path.GetDirectoryName(_tempFilePath).ToNotNullString());
                using (var fs = new FileStream(_tempFilePath, FileMode.Create))
                {
                    fs.Write(BitConverter.GetBytes(version), 0, 4);
                    foreach (var perm in sortedArray)
                    {
                        fs.Write(perm.ToBytes(), 0, 62);
                    }
                    fs.Close();
                }

                if (!FileHelper.CompareFiles(_tempFilePath, _accessFilePath, false))
                {
                    if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                    {
                        LoggingSystem.LogInfo("Access file is change so it must be uploaded", Camera);
                    }
                    if (File.Exists(_accessFilePath))
                    {
                        File.Delete(_accessFilePath);
                    }
                    File.Copy(_tempFilePath, _accessFilePath);
                    result = true;
                }
                else
                {
                    if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                    {
                        LoggingSystem.LogInfo("Access file is same as previous", Camera);
                    }
                }
            }
            finally
            {
                if (File.Exists(_tempFilePath))
                {
                    File.Delete(_tempFilePath);
                }
            }

            return result;
        }

        private void SendPermissionFile()
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
            {
                LoggingSystem.LogInfo("SendPermissionFile is calling", Camera);
            }
            if (!File.Exists(_accessFilePath))
            {
                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                {
                    LoggingSystem.LogInfo("No file exists for send", Camera);
                }
            }

            var permissionUploadAddress =
                ApiCallHelpers.CombineUri(_cameraBaseAddress, "Karabin/Permission?Action=UploadPlateList");
            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            var request = (HttpWebRequest)WebRequest.Create(permissionUploadAddress);
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;

            using (Stream memStream = new MemoryStream())
            {
                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                var endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");
                var headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                    "Content-Type: application/octet-stream\r\n\r\n";
                //var fs = new FileInfo(_accessFilePath);

                memStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                var header = string.Format(headerTemplate, "uplTheFile", Path.GetFileName(_accessFilePath));
                var headerBytes = Encoding.UTF8.GetBytes(header);

                memStream.Write(headerBytes, 0, headerBytes.Length);

                using (var fileStream = new FileStream(_accessFilePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    _ = memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }
            }
        }

        #endregion


        #region Read Attendance

        private static bool TryConvertApiReceivedPlateToPlateString(string plateString, out KarabinPlate plateResult)
        {
            if (plateString.IsNotNullOrEmpty())
            {
                var plateStringProcessed = plateString.Replace(",", "");
                switch (plateStringProcessed.Length)
                {
                    case 8:

                        var chunk1 = plateStringProcessed.Substring(0, 2);
                        var chunk2 = plateStringProcessed.Substring(2, 3);
                        var chunk3 = plateStringProcessed.Substring(5, 2);
                        var chunk4 = plateStringProcessed.Substring(7, 1);
                        if (int.TryParse(chunk1, out var chunk1Number)
                            && int.TryParse(chunk2, out var chunk2Number)
                            && int.TryParse(chunk3, out var chunk3Number)
                            && NationalNumberMapFromNumberToString.TryGetValue(chunk4.ToInt32(), out var plateLetter)
                            && chunk1Number > 0
                            && chunk2Number > 0
                            && chunk3Number > 0
                           )
                        {
                            plateResult = new KarabinPlate
                            {
                                PlateString = PlateHelper.ConvertChunksToKarnamaPlate(CarPlateTypeEnumeration.National,
                                    chunk1, plateLetter, chunk2, chunk3),
                                PlateType = CarPlateTypeEnumeration.National
                            };
                            return true;
                        }

                        break;
                }
            }

            plateResult = null;
            return false;
        }

        public List<DtoPlateDetectionCameraCarAttendance> ReadCameraAttendance(DateTime startDateTime, DateTime endDateTime)
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendance))
            {
                LoggingSystem.LogInfo("ReadCameraAttendance for date interval is calling", new
                {
                    Camera,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                });
            }

            var allAttendances = new List<DtoPlateDetectionCameraCarAttendance>();
            if (startDateTime.Date == endDateTime.Date)
            {
                allAttendances.AddRange(
                    ReadCameraAttendance(startDateTime.Date
                        , startDateTime.ToString("HH:mm")
                        , endDateTime.ToString("HH:mm")));
            }
            else
            {

                allAttendances.AddRange(
                    ReadCameraAttendance(startDateTime.Date
                        , startDateTime.ToString("HH:mm")
                        , "23:59"));
                for (var currentDateForReadData = startDateTime.Date.AddDays(1); currentDateForReadData < endDateTime.Date; currentDateForReadData = currentDateForReadData.AddDays(1))
                {
                    allAttendances.AddRange(
                        ReadCameraAttendance(currentDateForReadData.Date
                            , "00:00"
                            , "23:59"));
                }
                allAttendances.AddRange(
                    ReadCameraAttendance(endDateTime.Date
                        , "00:00"
                        , endDateTime.ToString("HH:mm")));
            }
            return allAttendances.OrderBy(a => a.AttendanceDateTime).ToList();
        }

        public List<DtoPlateDetectionCameraCarAttendance> ReadCameraAttendance(DateTime date, string startTime, string endTime)
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendance))
            {
                LoggingSystem.LogInfo("ReadCameraAttendance for date is calling", new
                {
                    Camera,
                    StartTime = startTime,
                    EndTime = endTime
                });
            }
            var result = new List<DtoPlateDetectionCameraCarAttendance>();


            ReadAttendanceResponseModel resultOfGetLastTransmissionsApi = RestSharpClient.GetInstance().Execute<ReadAttendanceResponseModel>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(_cameraBaseAddress, "Karabin/rest/api/getLastTransmissions"),
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Camera.ApiBasicAuthPassword,
                    Username = Camera.ApiBasicAuthUsername,
                },
                Method = Method.Get,
                QueryStringParameters = new Dictionary<string, string>
                {
                    {"plate", ""},
                    {"date", date.ToPersianDateTime().ToString("yyyy/MM/dd")},
                    {"minTime", startTime},
                    {"maxTime", endTime},
                    {"minSpeed", "0"},
                    {"maxSpeed", "300"},
                }
            });

            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendance))
            {
                LoggingSystem.LogInfo("Call getLastTransmissions result", new
                {
                    Camera,
                    Transmissions = resultOfGetLastTransmissionsApi
                });
            }

            if (resultOfGetLastTransmissionsApi?.MainResult == "OK")
            {
                if (resultOfGetLastTransmissionsApi.Data.IsCollectionNotNullOrEmpty())
                {
                    foreach (var listInItem in resultOfGetLastTransmissionsApi.Data)
                    {
                        foreach (var item in listInItem)
                        {
                            var isValidRecord = true;
                            var attendanceDateTime = PersianDateTime.Now;
                            KarabinPlate plate = null;
                            if (item.Info == null)
                            {
                                isValidRecord = false;
                            }
                            else
                            {

                                if (!TryConvertApiReceivedPlateToPlateString(item.Info.Plate, out plate))
                                {
                                    isValidRecord = false;
                                }

                                if (!PersianDateTime.TryParse(item.Info.Datetime, out attendanceDateTime))
                                {
                                    isValidRecord = false;
                                }
                            }

                            if (!isValidRecord)
                            {
                                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendance))
                                {
                                    LoggingSystem.LogInfo("Invalid Attendance detected on karabin camera api call", new
                                    {
                                        Camera,
                                        Attendance = item
                                    });
                                }

                                continue;

                            }

                            PlateRecognitionAcceptType acceptType;
                            switch (item.Info.Authority)
                            {
                                case "ACCEPTED":
                                    acceptType = PlateRecognitionAcceptType.Accepted;
                                    break;
                                case "ACCEPTED_VIP":
                                    acceptType = PlateRecognitionAcceptType.AcceptedVip;
                                    break;
                                case "BLACKLIST":
                                    acceptType = PlateRecognitionAcceptType.Blacklist;
                                    break;
                                case "REJECTED":
                                    acceptType = PlateRecognitionAcceptType.Rejected;
                                    break;
                                default:
                                    acceptType = PlateRecognitionAcceptType.None;
                                    break;
                            }
                            result.Add(new DtoPlateDetectionCameraCarAttendance
                            {
                                AttendanceDateTime = attendanceDateTime,
                                PlateType = plate.PlateType,
                                PlateString = plate.PlateString,
                                CameraId = Camera.Id,
                                IdOnCamera = item.Id,
                                AcceptType = acceptType
                            });
                        }
                    }
                }
                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendance))
                {
                    LoggingSystem.LogInfo("Collected attendances ", new
                    {
                        Camera,
                        Attendances = result
                    });
                }

                if (Camera.CameraSettings.HasFlag(CameraSettingEnumeration.GetImageAtAutoAttendanceCollect))
                {
                    if (result.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var item in result)
                        {

                            try
                            {
                                var resultOfGetImage = GetAttendanceImage(item.IdOnCamera);
                                if (resultOfGetImage.IsCollectionNotNullOrEmpty())
                                {
                                    item.CarImage = resultOfGetImage;
                                }
                            }
                            catch (Exception exp)
                            {
                                LoggingSystem.LogError(exp, "Error on call get Image from camera",
                                    new { Attendance = item, Camera });
                            }
                        }
                    }
                }
            }
            else
            {
                LoggingSystem.LogError("Error on call getLastTransmissions", ObjectHelper.SerializeAsJsonFormatted(new
                {
                    Camera,
                    Result = resultOfGetLastTransmissionsApi
                }));
            }

            return result;
        }

        public byte[] GetAttendanceImage(string attendanceId)
        {
            //localhost:4200/assets/build-icons/top-right-logo.png
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendanceImage))
            {
                LoggingSystem.LogInfo("Start reading attendance image ", new
                {
                    CameraId = Camera.Id,
                    Id = attendanceId
                });
            }

            var imageType = "C";
            if (Camera.CameraSettings.HasFlag(CameraSettingEnumeration.IsBlackAndWhite))
            {
                imageType = "G";
            }
            var result = RestSharpClient.GetInstance().DownloadFile(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(_cameraBaseAddress, "Karabin/rest/api/get"),
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                AuthorizationData = new RestApiBasicAuthorizationData()
                {
                    Password = Camera.ApiBasicAuthPassword,
                    Username = Camera.ApiBasicAuthUsername,
                },
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "*/*" }
                },
                Method = Method.Get,
                QueryStringParameters = new Dictionary<string, string>
                {
                    {"id", attendanceId},
                    {"type", imageType},
                },
                RequestTimeout = TimeSpan.FromSeconds(30),
            });
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAgentReadCameraAttendanceImage))
            {
                LoggingSystem.LogInfo("end reading attendance image ", new
                {
                    CameraId = Camera.Id,
                    Id = attendanceId,
                    ImageSize = result?.Length ?? 0
                });
            }

            return result;
        }

        #endregion


        #region Private Types

        private class KarabinPlate
        {
            public CarPlateTypeEnumeration PlateType { get; set; }

            public string PlateString { get; set; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        private class Permission : IComparable<Permission>
        {
            public int CompareTo(Permission other)
            {
                return plate.CompareTo(other.plate);
            }

            public int plate;
            public string perm;
            public DateTime startDate;
            public DateTime endDate;

            public Permission(int in_plate, DateTime start, DateTime end, string in_perm)
            {
                plate = in_plate;
                perm = in_perm;
                startDate = start;
                endDate = end;
            }
            public byte[] ToBytes()
            {
                //4 bytes plate
                //8 start
                //8 finish
                //42 bytes perm
                var result = new byte[62];
                var plateArr = BitConverter.GetBytes(plate);
                //Array.Reverse(plateArr);

                var startArr = BitConverter.GetBytes(ConvertDateTimeToEpochTime(startDate, TimeResolution.MilliSecond));
                Array.Reverse(startArr, 0, 8);

                var finishArr = BitConverter.GetBytes(ConvertDateTimeToEpochTime(endDate, TimeResolution.MilliSecond));
                Array.Reverse(finishArr, 0, 8);


                Array.Copy(plateArr, 0, result, 0, 4);
                Array.Copy(startArr, 0, result, 4, 8);
                Array.Copy(finishArr, 0, result, 12, 8);
                Array.Copy(PermToByte(), 0, result, 20, 42);
                return result;
            }

            byte[] PermToByte()
            {
                var result = new byte[42];

                for (var i = 0; i < (168 / 4); i++)
                {
                    byte b = 0;
                    var y = 0;
                    for (var j = i * 4; j < (i * 4 + 4); j++)
                    {
                        var x = perm[j];

                        if (x == '0')
                            b |= 0;
                        else if (x == '1')
                            b |= 2;
                        else if (x == '2')
                            b |= 1;
                        else if (x == '3')
                            b |= 3;
                        if (y < 3)
                            b = (byte)(b * 4);//<< 4 shift
                        y++;
                    }

                    result[i] = b;
                }
                return result;
            }
        }


        private enum TimeResolution
        {
            Second,
            MilliSecond,
            MicroSecond,
        }

        #endregion


        #region Implementation of IDisposable

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
            }
            _disposed = true;
        }

        ~KarabinAgent()
        {
            Dispose(false);
        }

        #endregion


    }
}

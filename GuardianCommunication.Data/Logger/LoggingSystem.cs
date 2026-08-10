using System;
using System.Text;
using System.Text.RegularExpressions;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using Newtonsoft.Json;

namespace GuardianCommunication.Data.Logger
{
    public static class LoggingSystem
    {
        private static ILogRepository _loggerRepository;

        public static void Initialize(RepositoryFactory repositoryFactory)
        {
            _loggerRepository = repositoryFactory.GetLogRepository();
        }


        #region Error

        public static void LogError<T>(Exception exp, string title, T obj)
        {
            if (exp == null) return;
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Error,
                    Message = ObjectHelper.SerializeAsJsonFormatted(new { Exception = exp, ExceptionData = obj }),
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = title,
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void LogError<T>(string title, T obj)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Error,
                    Message = ObjectHelper.SerializeAsJsonFormatted(new { ErrorData = obj }),
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = title,
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void LogError(Exception exp, string title, string description)
        {
            if (exp == null) return;
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Error,
                    Message = ObjectHelper.SerializeAsJsonFormatted(new { Exception = exp, Description = description }),
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = title,
                    UserId = null,
                });

            }
            catch
            {
                // ignored
            }
        }

        public static void LogError(Exception exp, string title)
        {
            LogError(exp, title, string.Empty);
        }

        public static void LogError(Exception exp)
        {
            LogError(exp, string.Empty, string.Empty);
        }

        public static void LogError(string title, string errorMessage)
        {
            try
            {
                errorMessage = Regex.Replace(errorMessage, @"<br[^\/]*/?>", Environment.NewLine, RegexOptions.IgnoreCase);
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Error,
                    Message = errorMessage,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }

        #endregion


        public static void LogInfo(string message, string title)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Information,
                    Message = message,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Information,
                    Message = message,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = null,
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }

        public static void LogInfo<T>(string title, T obj)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Information,
                    Message = obj != null
                        ? JsonConvert.SerializeObject(obj, Formatting.Indented)
                        : "NULL",
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });


            }
            catch
            {
                // ignored
            }
        }

        public static void LogInfo<T>(string title, string message, T obj)
        {
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine("MESSAGE: " + (string.IsNullOrEmpty(message) ? string.Empty : message));
                builder.AppendLine(obj != null
                    ? JsonConvert.SerializeObject(obj, Formatting.Indented)
                    : "NULL");
                builder.AppendLine();


                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Information,
                    Message = builder.ToString(),
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });


            }
            catch
            {
                // ignored
            }
        }


        public static void LogWarning(string message, string title)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Warning,
                    Message = message,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });


            }
            catch
            {
                // ignored
            }
        }

        public static void LogWarning<T>(string title, T obj)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Warning,
                    Message = obj != null
                        ? JsonConvert.SerializeObject(obj, Formatting.Indented)
                        : "NULL",
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }


        public static void Debug(string message, string title)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Debug,
                    Message = message,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });

            }
            catch
            {
                // ignored
            }
        }


        public static void LogIncomingRequest(string message, string title)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.ApiCall,
                    Message = message,
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = (string.IsNullOrEmpty(title) ? string.Empty : title),
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }


        public static void LogHookError(Exception exp, object hookDetail)
        {
            try
            {
                _loggerRepository.Log(new DtoLoggerData
                {
                    EventId = null,
                    LevelCode = LogLevelCodeEnum.Error,
                    Message = ObjectHelper.SerializeAsJsonFormatted(new { Exception = exp, HookDetail = hookDetail }),
                    Source = ServiceConstants.LogSource,
                    RegisterDateTime = DateTime.Now,
                    Name = ServiceConstants.LogNameHookError,
                    UserId = null,
                });
            }
            catch
            {
                // ignored
            }
        }


    }

}

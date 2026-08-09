using System.Threading.Tasks;
using Dapper;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{

    public interface ILogRepository
    {
        Task LogAsync(DtoLoggerData loggerData);
        void Log(DtoLoggerData loggerData);
    }



    public class LogRepository : BaseRepository, ILogRepository
    {
        public LogRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }


        #region Command Text

        private const string DataLoggerInsert =
            @"
            INSERT INTO LoggerData
            (
                [LevelCode], [Name], [UserId], [Source], [EventId], [Message], [RegisterDateTime], [Checksum]
            )
            VALUES
            (
                @LevelCode, @Name, @UserId, @Source, @EventId, @Message, @RegisterDateTime, @Checksum
            )
        ";

        #endregion


        public async Task LogAsync(DtoLoggerData loggerData)
        {
            using (var dbConnection = GetLogConnection())
            {
                await dbConnection.ExecuteAsync(DataLoggerInsert, new
                {
                    loggerData.LevelCode,
                    loggerData.Name,
                    loggerData.UserId,
                    loggerData.Source,
                    loggerData.EventId,
                    loggerData.Message,
                    loggerData.RegisterDateTime,
                    Checksum = loggerData.GetChecksum(),
                });
            }

        }

        public void Log(DtoLoggerData loggerData)
        {
            using (var dbConnection = GetLogConnection())
            {
                dbConnection.Execute(DataLoggerInsert, new
                {
                    loggerData.LevelCode,
                    loggerData.Name,
                    loggerData.UserId,
                    loggerData.Source,
                    loggerData.EventId,
                    loggerData.Message,
                    loggerData.RegisterDateTime,
                    Checksum = loggerData.GetChecksum(),
                });
            }

        }

    }






}

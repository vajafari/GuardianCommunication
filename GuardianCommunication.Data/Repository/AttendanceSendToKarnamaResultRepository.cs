using System.Data;
using Dapper;

namespace GuardianCommunication.Data.Repository
{
    public interface IAttendanceSendToKarnamaResultRepository
    {
        void Insert(DtoAttendanceSendToKarnamaResult entity);
    }


    public class AttendanceSendToKarnamaResultRepository : BaseRepository, IAttendanceSendToKarnamaResultRepository
    {

        public AttendanceSendToKarnamaResultRepository(ConnectionConfiguration connectionConfig) : base(connectionConfig)
        { }

        #region Command Strings

        private const string InsertCommand =
            @"	INSERT INTO         [AttendanceSendToKarnamaResult]
				(
					  [AttendanceId]
					, [SendTime]
					, [IsSuccessful]
					, [StatusCode]
					, [ExceptionMessage]
				)
				VALUES
				(
					  @AttendanceId
					, @SendTime
					, @IsSuccessful
					, @StatusCode
					, @ExceptionMessage
				) ;
			";

        #endregion

        public void Insert(DtoAttendanceSendToKarnamaResult entity)
        {
            using (var connection = GetConnection())
            {
                connection.ExecuteScalar<int>(InsertCommand, entity
                    , commandType: CommandType.Text, commandTimeout: connectionConfig.Timeout);

            }
        }

    }

}

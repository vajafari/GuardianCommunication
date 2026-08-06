namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerServerConfig
    {
        public int GetCommandTimerIntervalInMillisecond { get; set; }

        #region Push

        public string PushServerPushAddress { get; set; }
        public int PushServerMaxCommandLength { get; set; }
        public int PushServerMaxCommandCount { get; set; }
        public int PushServerIntervalForConsiderDeviceOnlineInSecond { get; set; }

        #endregion

        #region Grpc

        public int GrpcServerPort { get; set; }
        public int GrpcServerCommandCount { get; set; }
        public int GrpcCommandTimerIntervalInMillisecond { get; set; }
        public int GrpcServerTimeoutShortInMillisecond { get; set; }
        public int GrpcServerTimeoutLongInMillisecond { get; set; }
        public int GrpcServerTimeoutVeryLongInMillisecond { get; set; }

        #endregion
        

    }
}

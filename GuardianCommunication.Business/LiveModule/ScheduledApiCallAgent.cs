using System;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;

namespace GuardianCommunication.Business.LiveModule
{
    public class ScheduledApiCallAgent
    {
        private readonly DtoScheduledApiCallTask _scheduledApiCall;

        public ScheduledApiCallAgent(DtoScheduledApiCallTask scheduledApiCall)
        {
            _scheduledApiCall = scheduledApiCall;
        }

        private readonly object _lock = new object();
        public void CallApi()
        {

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.ScheduledApiCallAgent))
            {
                LoggingSystem.LogInfo("ScheduledApiCallAgent is calling", string.Empty, _scheduledApiCall);
            }
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    RestSharpClient.GetInstance().CallDynamicApiAsVoid(_scheduledApiCall);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on ScheduledApiCallAgent", _scheduledApiCall);
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            else
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.ScheduledApiCallAgent))
                {
                    LoggingSystem.LogInfo("_lock skipped because of lock is taken");
                }
            }
        }

    }
}

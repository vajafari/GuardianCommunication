using System.Net.NetworkInformation;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class NetworkHelpers
    {
        public static bool PingHost(string ipAddress, int timeout)
        {
            try
            {
                using (var pingSender = new Ping())
                {
                    var options = new PingOptions
                    {
                        Ttl = 64,
                        DontFragment = false // اجازه عبور از مسیرهای مختلف شبکه
                    };

                    var buffer = new byte[32];
                    var reply = pingSender.Send(ipAddress, timeout, buffer, options);

                    if (reply != null && reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                    return false;

                }
            }
            catch
            {
                return false;
            }
        }
    }
}

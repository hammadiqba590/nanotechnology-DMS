using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NanoDMSAuthService.Common
{
    public static class ClientInfoHelper
    {
        // Get the IP address of the client's machine
        public static string GetIpAddress()
        {
            // Use Dns.GetHostEntry for the local machine
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return localIp?.ToString() ?? "127.0.0.1"; // Default to localhost if not found
        }

        // Get MAC address with hyphens
        public static string GetMacAddress()
        {
            var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => string.Join("-", nic.GetPhysicalAddress()
                    .GetAddressBytes()
                    .Select(b => b.ToString("X2"))))
                .FirstOrDefault();

            return macAddress ?? string.Empty;
        }

        // Get PC name
        public static string GetPcName()
        {
            return Environment.MachineName;
        }
    }

}

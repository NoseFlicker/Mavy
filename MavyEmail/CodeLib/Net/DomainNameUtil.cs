using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using QiHe.CodeLib.Net.Dns;

namespace QiHe.CodeLib.Net
{
    public class DomainNameUtil
    {
        private static string[] FindDnsServers()
        {
            var start = Registry.LocalMachine;
            const string DNSServers = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

            var DNSServerKey = start.OpenSubKey(DNSServers);
            if (DNSServerKey == null) return null;
            var serverlist = (string)DNSServerKey.GetValue("NameServer");
            DNSServerKey.Close();
            start.Close();
            return string.IsNullOrEmpty(serverlist) ? null : serverlist.Split(' ');
        }

        //some root DNS servers
        //http://en.wikipedia.org/wiki/Root_nameserver
        private static List<IPAddress> GetRootDnsServers()
        {
            var rootServers = new List<IPAddress>
            {
                IPAddress.Parse("128.8.10.90"),
                IPAddress.Parse("192.203.230.10"),
                IPAddress.Parse("192.36.148.17"),
                IPAddress.Parse("192.58.128.30"),
                IPAddress.Parse("193.0.14.129"),
                IPAddress.Parse("202.12.27.33")
            };
            return rootServers;
        }

        private static readonly List<IPAddress> RootDnsServers = GetRootDnsServers();

        public static IPHostEntry GetIPHostEntry(string domainName) => GetIPHostEntry(domainName, QueryType.Address, FindDnsServers());

        public static IPHostEntry GetIPHostEntryForMailExchange(string domainName) => GetIPHostEntry(domainName, QueryType.MailExchange, FindDnsServers());

        private static readonly List<IPAddress> DnsServers = new List<IPAddress>();
        
        /// <summary>
        /// Get IPHostEntry for given domainName.
        /// </summary>
        /// <param name="domainName">domainName</param>
        /// <param name="queryType">QueryType.Address or QueryType.MailExchange</param>
        /// <param name="dnsServers">dnsServers</param>
        /// <returns></returns>
        private static IPHostEntry GetIPHostEntry(string domainName, QueryType queryType, string[] dnsServers)
        {
            if (string.IsNullOrEmpty(domainName)) throw new ArgumentException("Domain name is empty.", "domainName");
            DnsServers.Clear();
            if (dnsServers != null) foreach (var dnsServer in dnsServers) DnsServers.Add(IPAddress.Parse(dnsServer));
            DnsServers.AddRange(RootDnsServers);
            var retry = 0;
            while (retry < 10)
            {
                foreach (var ip in DnsServers.Select(dnsServer => GetIPHostEntry(domainName, queryType, dnsServer)).Where(ip => ip != null)) return ip;
                retry++;
            }
            return null;
        }

        /// <summary>
        /// Get IPHostEntry for given domainName.
        /// </summary>
        /// <param name="domainName">domainName</param>
        /// <param name="queryType">QueryType.Address or QueryType.MailExchange</param>
        /// <param name="dnsServer">dnsServer</param>
        /// <returns></returns>
        public static IPHostEntry GetIPHostEntry(string domainName, QueryType queryType, IPAddress dnsServer)
        {
            var message = DnsMessage.StandardQuery();
            var query = new DnsQuery(domainName, queryType);
            message.Querys.Add(query);
            var msgData = DnsMessageCoder.EncodeDnsMessage(message);
            try
            {
                var reply = QueryServer(msgData, dnsServer);
                if (reply != null)
                {
                    var answer = DnsMessageCoder.DecodeDnsMessage(reply);
                    if (answer.ID == message.ID)
                    {
                        if (answer.Answers.Count > 0)
                        {
                            var host = new IPHostEntry();
                            host.HostName = domainName;
                            switch (queryType)
                            {
                                case QueryType.Address:
                                    host.AddressList = GetAddressList(domainName, answer);
                                    break;
                                case QueryType.MailExchange:
                                    host.Aliases = GetMailExchangeAliases(domainName, answer);
                                    host.AddressList = GetAddressList(answer, new List<string>(host.Aliases));
                                    break;
                                case QueryType.NameServer: break;
                                case QueryType.CanonicalName: break;
                                case QueryType.StartOfAuthorityZone: break;
                                case QueryType.WellKnownService: break;
                                case QueryType.Pointer: break;
                                case QueryType.HostInfo: break;
                                case QueryType.MailInfo: break;
                                case QueryType.Text: break;
                                case QueryType.UnKnown: break;
                                default: throw new ArgumentOutOfRangeException(nameof(queryType), queryType, null);
                            }
                            return host;
                        }
                        if (answer.AuthorityRecords.Count > 0)
                        {
                            var serverAddresses = GetAuthorityServers(answer);
                            // depth first search
                            foreach (var serverIP in serverAddresses)
                            {
                                var host = GetIPHostEntry(domainName, queryType, serverIP);
                                if (host != null) return host;
                            }
                        }
                    }
                }
            }
            catch {}
            return null;
        }

        private static IPAddress[] GetAddressList(string domainName, DnsMessage answer) => (from resource in answer.Answers where resource.QueryType == QueryType.Address && resource.Name == domainName select new IPAddress((byte[]) resource.Content)).ToArray();

        private static string[] GetMailExchangeAliases(string domainName, DnsMessage answer) => (from resource in answer.Answers where resource.QueryType == QueryType.MailExchange && resource.Name == domainName select (MailExchange) resource.Content into mailExchange select mailExchange.HostName).ToArray();

        private static IEnumerable<IPAddress> GetAuthorityServers(DnsMessage answer)
        {
            var authorities = (from resource in answer.AuthorityRecords where resource.QueryType == QueryType.NameServer select (string) resource.Content).ToList();
            if (answer.AdditionalRecords.Count > 0) return GetAddressList(answer, authorities);
            var serverAddresses = new List<IPAddress>();
            foreach (var authority in authorities) serverAddresses.AddRange(System.Net.Dns.GetHostAddresses(authority));
            return serverAddresses.ToArray();
        }

        private static IPAddress[] GetAddressList(DnsMessage answer, ICollection<string> authorities) => (from resource in answer.AdditionalRecords where resource.QueryType == QueryType.Address where authorities.Contains(resource.Name) select new IPAddress((byte[]) resource.Content)).ToArray();

        private static byte[] QueryServer(byte[] query, IPAddress serverIP)
        {
            byte[] retVal = null;
            try
            {
                var ipRemoteEndPoint = new IPEndPoint(serverIP, 53);
                var udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var ipLocalEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var localEndPoint = (EndPoint)ipLocalEndPoint;
                udpClient.Bind(localEndPoint);
                udpClient.Connect(ipRemoteEndPoint);
                //send query
                udpClient.Send(query);
                // Wait until we have a reply
                if (udpClient.Poll(5 * 1000000, SelectMode.SelectRead))
                {
                    retVal = new byte[512];
                    udpClient.Receive(retVal);
                }
                udpClient.Close();
            }
            catch {}
            return retVal;
        }
    }
}
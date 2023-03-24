using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace QiHe.CodeLib.Net
{
    public class EmailSender
    {
        
        #region Variables
        /// <summary>
        /// Default SMTP Port.
        /// </summary>
        private const int SmtpPort = 25;
        #endregion

        public static bool Send(string from, string to, string subject, string body)
        {
            var domainName = GetDomainName(to);
            var servers = GetMailExchangeServer(domainName);
            foreach (var server in servers)
                try
                {
                    var client = new SmtpClient(server.ToString(), SmtpPort);
                    client.Send(from, to, subject, body);
                    return true;
                }
                catch {}
            return false;
        }

        public static bool Send(MailMessage mailMessage)
        {
            var domainName = GetDomainName(mailMessage.To[0].Address);
            var servers = GetMailExchangeServer(domainName);
            foreach (var server in servers)
                try
                {
                    var client = new SmtpClient(server.ToString(), SmtpPort);
                    client.Send(mailMessage);
                    return true;
                }
                catch {}
            return false;
        }

        private static string GetDomainName(string emailAddress)
        {
            var atIndex = emailAddress.IndexOf('@');
            if (atIndex == -1) throw new ArgumentException("Not a valid email address", "emailAddress");
            if (emailAddress.IndexOf('<') > -1 && emailAddress.IndexOf('>') > -1) return emailAddress.Substring(atIndex + 1, emailAddress.IndexOf('>') - atIndex);
            return emailAddress.Substring(atIndex + 1);
        }

        private static IEnumerable<IPAddress> GetMailExchangeServer(string domainName)
        {
            var hostEntry = DomainNameUtil.GetIPHostEntryForMailExchange(domainName);
            if (hostEntry.AddressList.Length > 0) return hostEntry.AddressList;
            return hostEntry.Aliases.Length > 0 ? System.Net.Dns.GetHostAddresses(hostEntry.Aliases[0]) : null;
        }
    }
}
using System.Collections.Generic;

namespace QiHe.CodeLib.Net.Dns
{
    public enum MessageType { Query = 0, Response = 1 }

    /// <summary>
    /// kind of query in DnsMessage
    /// </summary>
    public enum QueryKind
    {
        /// <summary>
        ///  a standard query.
        /// </summary>
        StandardQuery = 0,

        /// <summary>
        /// an inverse query.
        /// </summary>
        InverseQuery = 1,

        /// <summary>
        /// a server status request.
        /// </summary>
        ServerStatus = 2,

        //3-15          reserved for future use
    }

    /// <summary>
    /// Dns server reply codes.
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// No error condition.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Format error - The name server was unable to interpret the query.
        /// </summary>
        FormatError = 1,

        /// <summary>
        /// Server failure - The name server was unable to process this query due to a problem with the name server.
        /// </summary>
        ServerFailure = 2,

        /// <summary>
        /// Name Error - Meaningful only for responses from an authoritative name server, this code signifies that the
        /// domain name referenced in the query does not exist.
        /// </summary>
        NameError = 3,

        /// <summary>
        /// Not Implemented - The name server does not support the requested kind of query.
        /// </summary>
        NotImplemented = 4,

        /// <summary>
        /// Refused - The name server refuses to perform the specified operation for policy reasons.
        /// </summary>
        Refused = 5,

        //6-15            Reserved for future use.
    }

    public class DnsMessage
    {
        /// <summary>
        /// This identifier is copied the corresponding reply and can be used by the requester
        /// to match up replies to outstanding queries.
        /// </summary>
        public ushort ID;

        /// <summary>
        /// Message Type, Query or Response.
        /// </summary>
        public MessageType Type;

        /// <summary>
        /// Kind of query in this message
        /// </summary>
        public QueryKind QueryKind;

        /// <summary>
        ///  valid in responses, and specifies that the responding name server is an
        ///  authority for the domain name in question section.
        /// </summary>
        public bool IsAuthoritativeAnswer;

        /// <summary>
        /// Specifies that this message was truncated due to length greater than 
        /// that permitted on the transmission channel.
        /// </summary>
        public bool IsTruncated;

        /// <summary>
        /// If set, it directs the name server to pursue the query recursively.
        /// Recursive query support is optional.
        /// </summary>
        public bool IsRecursionDesired;
        public bool IsRecursionAvailable;
        
        /// <summary>
        /// set as part of responses.
        /// </summary>
        public ResponseCode ResponseCode;

        public readonly List<DnsQuery> Querys = new List<DnsQuery>();
        public readonly List<DnsResource> Answers = new List<DnsResource>();

        public readonly List<DnsResource> AuthorityRecords = new List<DnsResource>();
        public readonly List<DnsResource> AdditionalRecords = new List<DnsResource>();

        static ushort m_ID = 100;

        private static ushort GenerateID()
        {
            if (m_ID >= 65535) m_ID = 100;
            return m_ID++;
        }

        public static DnsMessage StandardQuery() => new DnsMessage {ID = GenerateID(), Type = MessageType.Query, QueryKind = QueryKind.StandardQuery};

        public static DnsMessage InverseQuery() => new DnsMessage {ID = GenerateID(), Type = MessageType.Query, QueryKind = QueryKind.InverseQuery};
    }
}
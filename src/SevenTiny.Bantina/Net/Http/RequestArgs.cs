using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Bantina.Net.Http
{
    public class GetRequestArgs : CommonRequestArgs { public GetRequestArgs() : base(RequestType.GET) { } }
    public class PostRequestArgs : CommonRequestArgs
    {
        public PostRequestArgs() : base(RequestType.POST) { }
        internal PostRequestArgs(RequestType requestType) : base(requestType) { }

        public string Data { get; set; }
        public string ContentType { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
    public class PutRequestArgs : PostRequestArgs { public PutRequestArgs() : base(RequestType.PUT) { } }
    public class DeleteRequestArgs : PostRequestArgs { public DeleteRequestArgs() : base(RequestType.DELETE) { } }

    /// <summary>
    /// Common Request Arguments
    /// </summary>
    public abstract class CommonRequestArgs
    {
        internal CommonRequestArgs(RequestType requestType)
        {
            RequestType = requestType;
        }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int Timeout { get; set; } = 0;
        /// <summary>
        /// internal mark request type
        /// </summary>
        internal RequestType RequestType { get; set; }
    }
}

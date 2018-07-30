using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Http
{
    public abstract class HttpHelper
    {
        public static string Get(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                Byte[] resultBytes = client.GetByteArrayAsync(url).Result;
                return Encoding.UTF8.GetString(resultBytes);
            }
        }
        public static async Task<string> GetAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                Byte[] resultBytes = await client.GetByteArrayAsync(url);
                return Encoding.Default.GetString(resultBytes);
            }
        }

        public static string Post(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                using (HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8))
                {
                    if (contentType != null)
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }
                    using (HttpResponseMessage responseMessage = client.PostAsync(url, content).Result)
                    {
                        Byte[] resultBytes = responseMessage.Content.ReadAsByteArrayAsync().Result;
                        return Encoding.UTF8.GetString(resultBytes);
                    }
                }
            }
        }
        public static async Task<string> PostAsync(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                using (HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8))
                {
                    if (contentType != null)
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }
                    using (HttpResponseMessage responseMessage = await client.PostAsync(url, content))
                    {
                        Byte[] resultBytes = await responseMessage.Content.ReadAsByteArrayAsync();
                        return Encoding.UTF8.GetString(resultBytes);
                    }
                }
            }
        }
    }
}
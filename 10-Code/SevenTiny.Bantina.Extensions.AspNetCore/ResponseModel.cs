using System;
using Newtonsoft.Json;

namespace SevenTiny.Bantina.Extensions.AspNetCore
{
    /// <summary>
    /// 通用的前端交互实体
    /// </summary>
    [Serializable]
    public class ResponseModel
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; } = 200;
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("more_msg")]
        public string MoreMessage { get; set; }
        [JsonProperty("tip_type")]
        public TipType TipType { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        public static ResponseModel Success(object data, string message = null)
            => new ResponseModel
            {
                IsSuccess = true,
                Message = message,
                TipType = TipType.Success,
                Data = data
            };

        public static ResponseModel Error(string message, object data = null)
            => new ResponseModel
            {
                IsSuccess = false,
                Message = message,
                TipType = TipType.Error,
                Data = data
            };
    }
}

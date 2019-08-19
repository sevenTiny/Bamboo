using System;
using Newtonsoft.Json;

namespace SevenTiny.Bantina.Extensions.AspNetCore
{
    [Serializable]
    public class ResponseModel
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("more_message")]
        public string MoreMessage { get; set; }
        [JsonProperty("tip_type")]
        public TipType TipType { get; }
        [JsonProperty("")]
        public object Data { get; set; }

        public static ResponseModel Success(object data, string message = null)
            => new ResponseModel
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };

        public static ResponseModel Error(string message, object data = null)
            => new ResponseModel
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
    }

    public static class ResponseModelExtension
    {
        public static ResponseModel ToResponseModel(this Result result)
            =>
            new ResponseModel
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
            };
        public static ResponseModel ToResponseModel<T>(this Result<T> result)
            =>
            new ResponseModel
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            };
    }
}

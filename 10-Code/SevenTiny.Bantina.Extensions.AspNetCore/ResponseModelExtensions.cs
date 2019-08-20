using Microsoft.AspNetCore.Mvc;

namespace SevenTiny.Bantina.Extensions.AspNetCore
{
    public static class ResponseModelExtensions
    {
        public static ResponseModel ToResponseModel(this Result result)
            =>
            new ResponseModel
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                MoreMessage = result.MoreMessage,
                TipType = result.TipType
            };

        public static ResponseModel ToResponseModel<T>(this Result<T> result)
            =>
            new ResponseModel
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                MoreMessage = result.MoreMessage,
                TipType = result.TipType,
                Data = result.Data
            };

        public static JsonResult ToJsonResult(this ResponseModel response)
           =>
           new JsonResult(response);

        public static JsonResult ToJsonResult(this Result result)
           =>
           new JsonResult(result.ToResponseModel());

        public static JsonResult ToJsonResult<T>(this Result<T> result)
           =>
           new JsonResult(result.ToResponseModel());
    }
}

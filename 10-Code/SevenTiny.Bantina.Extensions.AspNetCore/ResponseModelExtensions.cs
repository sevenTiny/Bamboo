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
    }
}

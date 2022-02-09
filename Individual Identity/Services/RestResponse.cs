namespace Individual_Identity.Services
{
    public class RestResponse
    {
        public const string Success = "success";
        public const string Fail = "fail";
        public const string Error = "error";

        public static RestResponse SuccessResponse { get { return new RestResponse() { Status = Success, Message = "Done successfully" }; } }
        public static RestResponse FailResponse { get { return new RestResponse() { Status = Fail, Message = "Fail in data" }; } }
        public static RestResponse ErrorResponse { get { return new RestResponse() { Status = Error, Message = "Error occurred" }; } }

        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}

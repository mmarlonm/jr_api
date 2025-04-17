namespace jr_api.DTOs
{
    public class ResponseDTO
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic data { get; set; }

    }
}

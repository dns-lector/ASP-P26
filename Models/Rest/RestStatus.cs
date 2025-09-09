namespace ASP_P26.Models.Rest
{
    public class RestStatus
    {
        public bool   IsOk   { get; set; } = true;
        public int    Code   { get; set; } = 200;
        public String Phrase { get; set; } = "Ok";


        public static readonly RestStatus RestStatus400 = new()
        {
            IsOk = false,
            Code = 400,
            Phrase = "Bad Request"
        };

        public static readonly RestStatus RestStatus401 = new()
        {
            IsOk = false,
            Code = 401,
            Phrase = "UnAuthorized"
        };

        public static readonly RestStatus RestStatus403 = new()
        {
            IsOk = false,
            Code = 403,
            Phrase = "Forbidden"
        };

        public static readonly RestStatus RestStatus409 = new()
        {
            IsOk = false,
            Code = 409,
            Phrase = "Conflict"
        };

        public static readonly RestStatus RestStatus500 = new()
        {
            IsOk = false,
            Code = 500,
            Phrase = "Internal Error. Details in Server logs"
        };
    }
}

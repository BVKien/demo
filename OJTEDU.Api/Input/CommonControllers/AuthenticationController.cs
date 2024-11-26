namespace OJTEDU.Api.Input.CommonControllers
{
    public class AuthenticationController
    {
        public class LoginRequest
        {
            public string? AuthorizeCode { get; set; }
        }
    }
}

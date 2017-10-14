namespace Xamarians.FacebookLogin.Platforms
{
    public class FbLoginResult
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ApplicationId { get; set; }
        public string JsonData { get; set; }
        public string AccessToken { get; set; }
        public FBStatus Status { get; set; }
        public string Message { get; set; }
    }

    public enum FBStatus
    {
        Error = 0, Success = 1, Cancelled = 2
    }

    public class FBShareResult
    {
        public ShareStatus Status { get; set; }
        public string Message { get; set; }
    }


    public enum ShareStatus
    {
        Error, Cancelled, Success
    }

}

namespace TeamSpecs.RideAlong.SecurityLibrary.Model
{
    public class AuthNRequest
    {
        public string username { get; }
        public string otp { get; }
        public AuthNRequest(string pUsername, string pOtp)
        {
            username = pUsername;
            otp = pOtp;
        }
    }
}

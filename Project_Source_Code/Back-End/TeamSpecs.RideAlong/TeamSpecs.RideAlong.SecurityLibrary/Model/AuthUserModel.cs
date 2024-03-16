using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary.Model
{
    public class AuthUserModel : IAuthUserModel
    {
        public long UID { get; set; }
        public string? userName { get; set; }
        public byte[] salt { get; set; }
        public string? userHash { get; set; }
        public AuthUserModel()
        {
            UID = 0;
            userName = null;
            salt = new byte[32];
            userHash = null;
        }
    }
}

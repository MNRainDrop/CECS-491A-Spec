namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class UserAccount
    {
        public ulong? UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public uint salt { get; set; } = 0;
        public string userHash { get; set; } = string.Empty;
    }
}

namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAuthUserModel
    {
        public long UID { get; set; }
        public string? userName { get; set; }
        public byte[] salt { get; set; }
        public string? userHash { get; set; }
    }
}

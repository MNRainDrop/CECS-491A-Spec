namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAppPrincipal
    {
        public IAuthUserModel userIdentity { get; set; }
        public ICollection<KeyValuePair<string, string>> claims { get; set; }
    }
}

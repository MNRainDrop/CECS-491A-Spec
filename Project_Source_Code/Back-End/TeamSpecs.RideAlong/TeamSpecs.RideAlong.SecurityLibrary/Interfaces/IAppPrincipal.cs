namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAppPrincipal
    {
        public IAuthUserModel userIdentity { get; set; }
        public IDictionary<string, string> claims { get; set; }
    }
}

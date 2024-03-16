namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IRideAlongPrincipal
    {
        IAuthUserModel userModel { get; set; }
        Dictionary<string, string> claims { get; set; }
    }
}

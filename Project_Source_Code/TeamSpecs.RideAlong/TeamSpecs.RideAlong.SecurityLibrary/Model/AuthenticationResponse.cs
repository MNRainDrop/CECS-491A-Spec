namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class AuthenticationResponse
    {
        // Member bodied expression
        public bool HasError => Principal is null ? true : false;
        public bool SafeToRetry { get; set; }
        public AppPrincipal? Principal { get; set; }
    }
}

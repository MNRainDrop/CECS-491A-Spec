namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest(UserAccount userIdentity, string proof)
        {
            UserIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
            Proof = proof ?? throw new ArgumentNullException();
        }
        public UserAccount UserIdentity { get; set; }
        // use Proof because it is a generic term
        public string Proof { get; set; } = string.Empty;
    }
}

namespace TeamSpecs.RideAlong.Services;

public interface IRandomService
{
    static abstract uint GenerateUnsignedInt();
    static abstract int GenerateSignedInt();
    string GenerateRandomString(int size); 
}

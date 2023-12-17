namespace TeamSpecs.RideAlong.Services;

public interface IRandomService
{
    byte[] GenerateUnsignedInt(int size);
    byte[] GenerateSignedInt(int size);
    string GenerateRandomString(int size, bool lowercase); 
}

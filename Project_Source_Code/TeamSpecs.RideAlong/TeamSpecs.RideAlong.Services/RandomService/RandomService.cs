
using System.Security.Cryptography;

namespace TeamSpecs.RideAlong.Services;

public class RandomService : IRandomService
{
    public byte[] GenerateUnsignedInt(int size)
    {
        byte[] randUint = RandomNumberGenerator.GetBytes(size);

        return randUint;
    }

    public byte[] GenerateSignedInt(int size)
    {
        //Yes it is obsolete but for some reason if I do it like Vong did it doesn't fuking work 
        byte[] result = new byte[size];
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetNonZeroBytes(result);
        return result;
    }

    public string GenerateRandomString(int size, bool lowercase)
    {
        //This method does not work since it only support .NET 8
        // var resutlString = RandomNumberGenerator.GetString(size, lowercase);
        // return resutlString;
        return "";
    }
}

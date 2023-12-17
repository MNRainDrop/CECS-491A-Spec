using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Services
{
    public class RandomGenerationService
    {

        public static byte[] GenerateUnsignedInt(int size) 
        {
            byte[] randUint = RandomNumberGenerator.GetBytes(size);

            return randUint; 
        }

        public static byte[] GenerateSignedInt(int size)
        {
            //Yes it is obsolete but for some reason if I do it like Vong did it doesn't fuking work 
            byte[] result = new byte[size];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(result);
            return result;
        }

        public static string GenerateRandomString(int size, bool lowercase)
        {
            //This method does not work since it only support .NET 8
            // var resutlString = RandomNumberGenerator.GetString(size, lowercase);
            // return resutlString;
            return "";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{
    public interface IAuthTarget
    {
        IResponse storeHashedPass(long UID, string passHash);//Returns a Response object with info as to if it passed or failed
        IResponse fetchClaims(long UID);//Returns a Dict<string,string> of claims
        IResponse fetchPass(long UID);//returns their password
        IResponse fetchAuthAccountModel(string username);//Returns the user's Auth Account Model based on their username

    }
}

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
        IResponse saveHashedPass(long UID, string passHash);//Returns a Response object with info as to if it passed or failed
        IResponse getClaims(long UID);//Returns a Dict<string,string> of claims
        IResponse fetchPass(long UID);//returns their password

    }
}

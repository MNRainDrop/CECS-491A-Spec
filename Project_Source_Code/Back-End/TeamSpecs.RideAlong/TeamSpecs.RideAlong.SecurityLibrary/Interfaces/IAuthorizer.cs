using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface IAuthorizer
    {
        AppPrincipal? Authenticate(AuthenticationRequest authRequest);
        bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> claims);
    }
}

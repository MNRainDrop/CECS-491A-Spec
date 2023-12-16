using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class AuthenticationRequest
    {
        public string UserIdentity { get; set; } = string.Empty;
        // use Proof because it is a generic term
        public string Proof { get; set; } = string.Empty;
    }
}

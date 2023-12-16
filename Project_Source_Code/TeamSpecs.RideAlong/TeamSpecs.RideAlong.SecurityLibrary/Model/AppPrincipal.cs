using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.SecurityLibrary 
{
    public class AppPrincipal
    {

        public AppPrincipal()
        {
            //expirationDate = getdatetimenow + expiration time
        }

        public AppPrincipal(string UserI, IDictionary<string, string> claims)
        {
            UserIdentity = UserI;
            expirationdate = DateTimeOffset.Now.AddMinutes(10);
            Claims = claims; 
        }


        public string UserIdentity { get; set; }
        // claims based add claims to the collection
        // roles based add the one role to the collection

        //public ICollection<(string ClaimName, string ClaimValue)> Claims { get; set; }
        // becomes a dictionary
        DateTimeOffset? expirationdate { get; }
        public IDictionary<string, string> Claims { get; set; }

    }

    
}

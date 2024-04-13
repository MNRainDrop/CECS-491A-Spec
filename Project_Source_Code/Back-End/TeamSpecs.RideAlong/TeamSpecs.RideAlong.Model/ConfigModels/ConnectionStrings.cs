using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.ConfigModels
{
    public class ConnectionStrings
    {
        public string readOnly { get; set; }
        public string writeOnly { get; set; }
        public string admin { get; set; }
        public ConnectionStrings(string readOnly, string writeOnly, string admin)
        {
            this.admin = admin;
            this.readOnly = readOnly;
            this.writeOnly = writeOnly;
        }
    }
}

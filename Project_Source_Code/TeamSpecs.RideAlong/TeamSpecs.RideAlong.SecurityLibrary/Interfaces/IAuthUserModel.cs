using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAuthUserModel
    {
        //UID
        public long UID { get; set; }
        //Username
        public string? userName { get; set; }
        //Salt
        public byte[] salt { get; set; }
        //UserHash
        public string? userHash { get; set; }
    }
}

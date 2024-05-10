using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.DataAccess
{
    public interface IJsonFileDAO : IReadOnlyFromFile, IWriteOnlyFromFile
    {
    }
}

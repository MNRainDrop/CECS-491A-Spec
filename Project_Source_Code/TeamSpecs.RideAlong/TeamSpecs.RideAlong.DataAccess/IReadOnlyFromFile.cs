using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.DataAccess;

using TeamSpecs.RideAlong.Model;

public interface IReadOnlyFromFile
{
    public IResponse ExecuteReadOnly();
}

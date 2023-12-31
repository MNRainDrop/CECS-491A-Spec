﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services
{
    public interface IPepperTarget
    {
        IResponse WriteToFile(KeyValuePair<string, uint> PepperObject);

        IResponse RetrieveFromFile(string key);

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserCreationTarget
    {
        public IResponse CheckDbForEmail(string email);
    }
}

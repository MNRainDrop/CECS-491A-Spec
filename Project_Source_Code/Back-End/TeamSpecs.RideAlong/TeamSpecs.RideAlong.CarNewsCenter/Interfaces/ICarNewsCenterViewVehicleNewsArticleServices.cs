using System;
using System.Security.Cryptography;
using TeamSpecs.RideAlong.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public interface IViewVehicleNewsArticleServices
    {
        IResponse GetNewsForAllVehicles(IAccountUserModel userAccount);
    }
}

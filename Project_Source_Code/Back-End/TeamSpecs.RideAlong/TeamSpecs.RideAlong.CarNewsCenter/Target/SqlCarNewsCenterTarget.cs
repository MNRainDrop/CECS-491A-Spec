using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public class SqlCarNewsCenterTarget : ICarNewsCenterTarget
    {
        private readonly IGenericDAO _dao;
        
        public SqlCarNewsCenterTarget(IGenericDAO dao)
        {
            _dao = dao; 
        }
        public IResponse GetsAllVehicles(BigInt UID)
        {
            throw new NotImplementedException();
        }
    }
}

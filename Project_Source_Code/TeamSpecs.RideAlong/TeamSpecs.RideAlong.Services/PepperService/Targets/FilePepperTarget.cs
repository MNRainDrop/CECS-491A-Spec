using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using System.Text.Json.Serialization;


namespace TeamSpecs.RideAlong.Services
{
    public class FilePepperTarget : IPepperTarget
    {
        private readonly IGenericDAO _fileDao;

        public FilePepperTarget(IGenericDAO Target)
        {
            _fileDao = Target;
        }
        public IResponse WriteToFile(KeyValuePair<string, uint> PepperObject)
        {
            //put this block to try-catch
            IResponse response = new Response();
            string jsonString = JsonSerializer.Serialize(PepperObject);
            _fileDao.ExecuteWriteOnly(jsonString);
            response.HasError = false;

            return response;
        }

        public IResponse RetrieveFromFile(string key)
        {
            var response = _fileDao.ExecuteReadOnly();
            List<KeyValuePair<string, uint >>? result = new List<KeyValuePair<string, uint>>();
            if (response.ReturnValue is not null){
                 foreach (string i in response.ReturnValue)
                 {
                    result = JsonSerializer.Deserialize<List<KeyValuePair<string, uint>>>(i);
                }
            }
   
            var res = result;
            var Response1 = new Response();
            Response1.ReturnValue = new List<object>();
            Response1.ReturnValue.Add(res);



            return Response1;
        }


    }
}

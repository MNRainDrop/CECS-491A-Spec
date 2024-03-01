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
            //Setting up variables 
            var response1 = _fileDao.ExecuteReadOnly();
            IResponse response = new Response();
            List<KeyValuePair<string, uint>>? temp = new List<KeyValuePair<string, uint>>();

            
            //Loading the Json file
            if (response1.HasError==false && response1.ReturnValue is not null)
            {
                foreach (string i in response1.ReturnValue)
                {
                    temp = JsonSerializer.Deserialize<List<KeyValuePair<string, uint>>>(i);
                }
            }
            //Checking for repetated key before write
            foreach (var x in temp)
            {
                var temp2 = x as KeyValuePair<string, uint>?;
                if (temp2.Value.Key == PepperObject.Key)
                {
                    throw new Exception("Key already exist in Pepper !!");
                }
            }

            //Writing part: adding pepper to the list and write the whole list to file 
            temp.Add(PepperObject);
            string jsonString = JsonSerializer.Serialize(temp);
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

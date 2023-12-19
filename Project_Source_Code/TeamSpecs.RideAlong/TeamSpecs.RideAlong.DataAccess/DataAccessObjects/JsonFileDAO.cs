using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DataAccess
{
    public class JsonFileDAO : IGenericDAO
    {
        private readonly string _currentFile;
        private string _relativePath;
        private readonly string _newfile;
        public JsonFileDAO()
        {
            //Default get relative path to PepperService folder
            _currentFile = @"\DataAccessObjects";
            _newfile = @"\PepperOutput.txt";
            _relativePath = "";
            //_relativePath = Path.GetFullPath(_currentFile)+_newfile;


        }
        public IResponse ExecuteWriteOnly(string Writevalue)
        {
            var response = new Response();
            //string fullPath = "C:\\Users\\huynj\\OneDrive\\Documents\\GitHub\\323--Project1\\CECS-491A-Spec\\Project_Source_Code\\TeamSpecs.RideAlong\\TeamSpecs.RideAlong.DataAccess\\DataAccessObjects\\PepperOutput.txt";
            //File.WriteAllText(fullPath, Writevalue);
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "PepperOutput.txt"),true))
            {
                outputFile.WriteLine(Writevalue);
            }
            //System.Diagnostics.Debug.WriteLine(Path.Combine(docPath, "PPOut.txt"));
            //System.Diagnostics.Debug.WriteLine("Hello");

            //try-catch block and see which to return 
            response.HasError = false;
            return response;
        }


        public IResponse ExecuteReadOnly()
        {
            var response = new Response();
         
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            response.ReturnValue = new List<object>();
            using (var result = new StreamReader(Path.Combine(docPath, "PepperOutput.txt")))
            {
                response.ReturnValue.Add(result.ReadToEnd());
            }
            response.HasError = false;


            return response; 
        }

        public int ExecuteWriteOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands)
        {
            throw new NotImplementedException();

        }

        public IResponse ExecuteReadOnly(SqlCommand sql)
        {
            throw new NotImplementedException();
        }



    }
}

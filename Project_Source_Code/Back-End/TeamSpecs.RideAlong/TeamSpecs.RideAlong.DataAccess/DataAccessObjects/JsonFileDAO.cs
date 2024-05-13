using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DataAccess
{
    public class JsonFileDAO : IJsonFileDAO
    {
       
        public IResponse ExecuteWriteOnly(string Writevalue)
        {
            var response = new Response();
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = Path.Combine(currentDir, @"../../../../PepperOutput.json");
            using (StreamWriter outputFile = new StreamWriter(filePath, false))
            {
                outputFile.WriteLine(Writevalue);
            }

            /*
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "PepperOutput.json"), false))
            {
                outputFile.WriteLine(Writevalue);
            }*/

            //try-catch block and see which to return 
            response.HasError = false;
            return response;
        }


        public IResponse ExecuteReadOnly()
        {
            var response = new Response();
            string text;

            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDir, @"../../../../PepperOutput.json");

            //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            response.ReturnValue = new List<object>();
            /*using (var result = new StreamReader(Path.Combine(docPath, "PepperOutput.json")))
            {
                response.ReturnValue.Add(result.ReadToEnd());
            }*/
            //File exist then read 
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
                response.ReturnValue.Add(text);
                response.HasError = false;

            }
            //If file doesn't exist then create a blanc file and return error for response object 
            else
            {
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.WriteLine("");
                }
                response.HasError = true;
            }


            return response;
        }

    }
}

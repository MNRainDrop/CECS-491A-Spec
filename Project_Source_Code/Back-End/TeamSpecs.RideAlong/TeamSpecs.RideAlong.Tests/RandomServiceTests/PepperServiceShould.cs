using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class PepperServiceShould
    {
        [Fact]
        public void PepperService_GeneratePepper_KeyPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var flag = true;
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            uint result;
            var _pepperTarget = new FilePepperTarget(dao);
            IRandomService randomService = new RandomService();
            IPepperService PepperObject = new PepperService(_pepperTarget, randomService);
            string key = "Test Key2";
            //aray of 10 keys to be passed in for generate
            string[] test_key = {"RideAlongPepper", "Test Key2" , "Test Key3" , "Test Key4" , 
            "Test Key5" , "Test Key6" , "Test Key7" , "Test Key8" , "Test Key9" , "Test Key10" };
            List<uint> test_result = new List<uint>();


            //Act 
            timer.Start();
            File.Delete(Path.Combine(docPath, "PepperOutput.json"));
            foreach (var i in test_key)
            {
                result = PepperObject.GeneratePepper(i); 
                test_result.Add(result);
            }
            foreach (var i in test_result)
            {
                if (i.GetType() != typeof(uint))
                {
                    flag = false; break;
                }
            }
            //File.Delete(Path.Combine(docPath, "PepperOutput.json"));
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(flag==true);
        }

        [Fact]
        public void PepperService_PopulateKeyValue_KeyAndValuePassedIn_ReturnKeyValuePair_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            IRandomService randomService = new RandomService();
            IPepperService PepperObject = new PepperService(_pepperTarget, randomService);
            string key = "AccountCreation";
            uint value = 0102030405;

            //Act 
            timer.Start();
            var result = PepperObject.PopulateKeyValue(key, value);
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.True(result.GetType() == typeof(KeyValuePair<string,uint>) && (result.Key == key && result.Value == value));
        }

       
        [Fact]
        public void PepperService_RetrievePepper_KeyPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var flag = true;
            uint result;
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            IRandomService randomService = new RandomService();
            IPepperService PepperObject = new PepperService(_pepperTarget, randomService);
            string key = "Test Key5";
            uint value = 1161839200;
            //aray of 10 keys to be passed in for retrieving 
            string[] test_key = {"Test Key1", "Test Key2" , "Test Key3" , "Test Key4" ,
            "Test Key5" , "Test Key6" , "Test Key7" , "Test Key8" , "Test Key9" , "Test Key10" };
            List<uint> test_result = new List<uint>();

            //Act 
            timer.Start();
            File.Delete(Path.Combine(docPath, "PepperOutput.json"));
            foreach (var i in test_key)
            {
                result = PepperObject.GeneratePepper(i);
                test_result.Add(result);
            }
            foreach (var i in test_key)
            {
                result = PepperObject.RetrievePepper(i);
                test_result.Add(result);
            }
            foreach (var i in test_result)
            {
                if (i.GetType() != typeof(uint))
                {
                    flag = false; break;
                }
            }
            File.Delete(Path.Combine(docPath, "PepperOutput.json"));
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.True(flag == true);
            

        }







        





    }
}

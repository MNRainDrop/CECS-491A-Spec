using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
#pragma warning disable
namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class PepperServiceShould
    {
        [Fact]
        public void PepperService_GeneratePepper_KeyPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            uint result;
            var _pepperTarget = new FilePepperTarget(dao);
            PepperService PepperObject = new PepperService(_pepperTarget);
            string key = "Test Key2";
            //aray of 10 keys to be passed in for generate
            string[] test_key = {"Test Key1", "Test Key2" , "Test Key3" , "Test Key4" , 
            "Test Key5" , "Test Key6" , "Test Key7" , "Test Key8" , "Test Key9" , "Test Key10" };
            List<uint> test_result = new List<uint>();


            //Act 
            timer.Start();
            foreach (var i in test_key)
            {
                result = PepperObject.GeneratePepper(i); 
                test_result.Add(result);
            }
            //result = PepperObject.GeneratePepper(key);
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            //Assert.True(result.GetType() == typeof(uint));
            foreach (var i in test_result)
            {
                Assert.True(i.GetType() == typeof(uint));
            }
        }

        [Fact]
        public void PepperService_PopulateKeyValue_KeyAndValuePassedIn_ReturnKeyValuePair_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            PepperService PepperObject = new PepperService(_pepperTarget);
            string key = "Test Key";
            uint value = 0000000000;

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
            uint result;
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            PepperService PepperObject = new PepperService(_pepperTarget);
            string key = "Test Key5";
            uint value = 1161839200;
            //aray of 10 keys to be passed in for retrieving 
            string[] test_key = {"Test Key1", "Test Key2" , "Test Key3" , "Test Key4" ,
            "Test Key5" , "Test Key6" , "Test Key7" , "Test Key8" , "Test Key9" , "Test Key10" };
            List<uint> test_result = new List<uint>();

            //Act 
            timer.Start();
            //var result = PepperObject.RetrievePepper(key);
            foreach (var i in test_key)
            {
                result = PepperObject.RetrievePepper(i);
                test_result.Add(result);
            }
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            foreach (var i in test_result)
            {
                Assert.True(i.GetType() == typeof(uint));
            }

        }







        





    }
}
#pragma warning restore
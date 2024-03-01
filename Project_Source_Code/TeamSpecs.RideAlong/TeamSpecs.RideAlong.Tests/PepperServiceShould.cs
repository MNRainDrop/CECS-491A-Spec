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
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            PepperService PepperObject = new PepperService(_pepperTarget);
            string key = "Test Key2";

            //Act 
            timer.Start();
            var result = PepperObject.GeneratePepper(key);
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(result.GetType() == typeof(uint));
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
            var timer = new Stopwatch();
            IResponse response;
            var dao = new JsonFileDAO();
            var _pepperTarget = new FilePepperTarget(dao);
            PepperService PepperObject = new PepperService(_pepperTarget);
            string key = "Test Key2";
            uint value = 3490748009;

            //Act 
            timer.Start();
            var result = PepperObject.RetrievePepper(key);
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.True(result==value);


        }





        





    }
}

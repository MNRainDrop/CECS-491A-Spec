﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary;
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
            string key = "Test Key";

            //Act 
            timer.Start();
            var result = PepperObject.GeneratePepper(key);
            timer.Stop();

            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
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


    }
}

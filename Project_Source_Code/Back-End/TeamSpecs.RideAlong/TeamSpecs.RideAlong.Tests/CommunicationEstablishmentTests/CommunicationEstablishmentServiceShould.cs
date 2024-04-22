using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.CommunicationEstablishmentTests
{
    public class CommunicationEstablishmentServiceShould
    {
        [Fact]
        public void CommuncationEstablishment_Retrieve_Seller()
        {
            //Arange
            var timer = new Stopwatch();
            var _dao = new SqlServerDAO();
            var _target = new SqlCommunicationEstablishmentTarget(_dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(_dao);
            var logService = new LogService(logTarget, hashService);
            var giveSellerUsername = new GetSellerInfoService(_target, logService);

            var user = new AccountUserModel("testCommunicationEstablishmentUser1")
            {
                Salt = 0,
                UserHash = "testCommunicationEstablishmentUser1",
            };


            //Act


            //Assert
        }
    }
}

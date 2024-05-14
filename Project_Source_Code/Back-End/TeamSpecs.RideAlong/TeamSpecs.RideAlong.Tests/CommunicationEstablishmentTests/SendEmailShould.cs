using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.CELibrary.Target;
using TeamSpecs.RideAlong.CoEsLibrary.Interfaces;
using TeamSpecs.RideAlong.CoEsLibrary.Services;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.CommunicationEstablishmentTests
{
    public class SendEmailShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly ISqlServerDAO dao = new SqlServerDAO(configService);
        private static readonly ISqlDbCETarget userTarget = new SqlDbCETarget(dao);

        private static readonly IHashService hashService = new HashService();
        private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
        private static readonly ILogService logService = new LogService(logTarget, hashService);
        private static readonly IMailKitService mailKitService = new MailKitService(configService);

        private static readonly ISendEmailService sendEmailService = new SendEmailService(logService, userTarget, mailKitService);

        [Fact]
        public void CommunicationEstablishmentSendEmailServiceShould()
        {
            //Arrange
            var timer = new Stopwatch();


            IResponse response;

            //Parameters 
            var user = new AccountUserModel("huh12345@aol.com")
            {
                UserId = 10041,
                Salt = 123456,                         // Sample salt value
                UserHash = "IMY"           // Sample user hash (hashed password)
            };

            string vin = "987654";
            string status = "Selling";
            DateTime ps = DateTime.Now;
            decimal price = 1.00m;
            DateTime priceDate = DateTime.Now;
            int inquiries = 0;


            var vehiclepro = "INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year,DateCreated ) VALUES (@VIN, @Owner_UID, @LicensePlate, @Make, @Model, @Year,@DateCreated)";
            var vehicleproParameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", vin),
                new SqlParameter("@Owner_UID", 0),
                new SqlParameter("@LicensePlate", "ABC123"),
                new SqlParameter("@Make", "HOnda"),
                new SqlParameter("@Model", "CRV"),
                new SqlParameter("@Year", 2000),
                new SqlParameter("@DateCreated", DateTime.Now)
            };
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehiclepro, vehicleproParameters)
            });

            var vendingStatusSql = "INSERT INTO VendingStatus (VIN, Status, PostingDate, Price, PriceDate, Inquiries) VALUES (@VIN, @Status, @PostingDate, @Price, @PriceDate, @Inquiries)";
            var vendingStatusParameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", vin),
                new SqlParameter("@Status", status),
                new SqlParameter("@PostingDate", ps),
                new SqlParameter("@Price", price),
                new SqlParameter("@PriceDate", priceDate),
                new SqlParameter("@Inquiries", inquiries)
            };
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vendingStatusSql, vendingStatusParameters)
            });

            //Act
            //Service
            try
            {
                timer.Start();
                response = sendEmailService.SendEmail(user, vin);
                timer.Stop();
            }
            finally
            {
                var undoInsert = $"DELETE FROM VehicleProfile WHERE Year = {2000}; DELETE FROM VendingStatus WHERE VIN = {vin};";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
            }
            //Assert
            Assert.True(response is not null);
            Assert.True(response.HasError == false);
            Assert.True(response.ReturnValue is not null);
            Assert.True(timer.ElapsedMilliseconds < 3000);
            //Assert.True(response.ReturnValue.First().ToString() == "gioman155@yahoo.com");
        }
    }

}

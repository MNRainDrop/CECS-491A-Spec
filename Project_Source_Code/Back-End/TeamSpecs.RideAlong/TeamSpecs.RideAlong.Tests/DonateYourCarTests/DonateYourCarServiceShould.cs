using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.TestingLibrary.DonateYourCarTests
{
    public class DonateYourCarServiceShould
    {
        [Fact]

        public void DonateYourCar_ReturnCharities()
        {
            // Arrange
            var timer = new Stopwatch();
            var dao = new SqlServerDAO();

            var sql = new SqlCommand("SELECT Charity, Description, Link FROM Charity;");

            // Expected 
            var expectedHasError = false;
            string? expectedErrorMessage = null;
            var expectedRowCount = 4;

            // Define the expected rows with their values
            var expectedRows = new List<object[]>
            {
                new object[] { "Catholic Charities of Orange County", "The mission of Catholic Charities of Orange County is to serve people in need, strengthen family and community, support parish ministries and extend Gods love to all.", "https://ccoc.careasy.org/home#form-iframe" },
                new object[] { "Kars4Kids", "Your donation supports the youth and educational programs of national nonprofit Kars4Kids", "https://www.kars4kids.org/" },
                new object[] { "Vehicles For Veterans", "Your car donation helps benefit disabled veterans throughout the United States. The proceeds from your donation help to fund programs that provide services offered to veterans in need of assistance and support.", "https://www.vehiclesforveterans.org/" },
                new object[] { "Wheels For Wishes & Wellness", "Wheels For Wishes & Wellness believes that every child, no matter his or her health, deserves a chance to have a happy and fulfilling childhood. By donating your vehicle, we are able to raise funds to support local childrens charities.", "https://kids.wheelsforwishes.org/car-donation-form/" }
            };

            // Act
            timer.Start();
            var response = dao.ExecuteReadOnly(sql);
            timer.Stop();

            // Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.True(response.HasError == expectedHasError);
            Assert.True(response.ErrorMessage == expectedErrorMessage);

            // Assert that the number of rows returned matches the expected row count
            Assert.Equal(expectedRowCount, response.ReturnValue.Count);

            // Assert that each row in the result matches the expected rows
            foreach (object[] actualRow in response.ReturnValue)
            {
                bool rowFound = false;
                foreach (object[] expectedRow in expectedRows)
                {
                    if (actualRow.SequenceEqual(expectedRow))
                    {
                        rowFound = true;
                        break;
                    }
                }
                Assert.True(rowFound, "Row not found in expected rows");
            }
        }
    }
}

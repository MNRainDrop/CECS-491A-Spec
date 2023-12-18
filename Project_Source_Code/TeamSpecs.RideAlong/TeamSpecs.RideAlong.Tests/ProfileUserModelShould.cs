using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class ProfileUserModelShould
    {
        [Fact]
        public void ProfileUserModel_GetDateOfBirth_NoParametersPassedIn_OneValidDateTimeReturned_Pass()
        {
            // Arrange
            DateTime expectedDateofBirth = new DateTime(1990, 1, 1);
            string testEmail = "testemail@gmail.com";
            var user = new ProfileUserModel(expectedDateofBirth, testEmail);

            // Act
            DateTime actualDateOfBirth = user.DateOfBirth;

            // Assert
            Assert.Equal(expectedDateofBirth, actualDateOfBirth);
        }

        [Fact]
        public void ProfileUserModel_SetDateofBirth_ValidParameterPassedIn_NewValueExpected_Pass()
        {
            // Arrange
            DateTime expectedDateOfBirth = new DateTime(1995, 5, 5);
            string testEmail = "testemail@gmail.com";
            var user = new ProfileUserModel(DateTime.MinValue, testEmail);

            // Assert
            user.DateOfBirth = expectedDateOfBirth;

            // Arrange
            Assert.Equal(expectedDateOfBirth, user.DateOfBirth);
        }
    }
}

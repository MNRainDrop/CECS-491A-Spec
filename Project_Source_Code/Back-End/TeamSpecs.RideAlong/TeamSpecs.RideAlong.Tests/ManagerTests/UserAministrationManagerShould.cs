using Moq;
using TeamSpecs.RideAlong.Managers;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.Services.HashService;



namespace TeamSpecs.RideAlong.TestingLibrary.ManagerTests
{
    public class UserAministrationManagerShould
    {
        [Fact]
        public void RegisterUser_WithValidData_ReturnsSuccessResponse()
        {
            // Arrange
            var timer = new Stopwatch();
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO), new PepperService(new FilePepperTarget(new JsonFileDAO())), new HashService());
            var userAdministrationManager = new UserAdministrationManager(accountCreationService);

            // Act
            response = userAdministrationManager.RegisterUser("beebop@yahoo.com", DateTime.Now.AddYears(-20), "Vendor");

            // Assert
            Assert.False(response.HasError);
        }

        [Fact]
        public void RegisterUser_WithInvalidUsername_ReturnsErrorResponse()
        {
            // Arrange
            var manager = new UserAdministrationManager(Mock.Of<IAccountCreationService>());

            // Act
            var response = manager.RegisterUser("Hello", DateTime.Now.AddYears(-20), "Vendor");

            // Assert
            Assert.True(response.HasError);
        }

        [Fact]
        public void RegisterUser_WithInvalidDateOfBirth_ReturnsErrorResponse()
        {
            // Arrange
            var manager = new UserAdministrationManager(Mock.Of<IAccountCreationService>());
            var invalidDateOfBirth = DateTime.Now.AddYears(-16); // Assuming the user must be at least 18 years old

            // Act
            var response = manager.RegisterUser("validusername@aol.com", invalidDateOfBirth, "Vendor");

            // Assert
            Assert.True(response.HasError);
        }

        [Fact]
        public void RegisterUser_WithInvalidAccountType_ReturnsErrorResponse()
        {
            // Arrange
            var manager = new UserAdministrationManager(Mock.Of<IAccountCreationService>());

            // Act
            var response = manager.RegisterUser("validusername@aol.com", DateTime.Now.AddYears(-20), "Not an accountType");

            // Assert
            Assert.True(response.HasError);
        }

        [Fact]
        public void IsNotTakenUserNameInDatabase_WithExistingUsername_ReturnsFalse()
        {
            // Arrange
            var accountCreationServiceMock = new Mock<IAccountCreationService>();
            accountCreationServiceMock.Setup(x => x.IsUserRegistered("existingusername")).Returns(new Response { HasError = true });

            var manager = new UserAdministrationManager(accountCreationServiceMock.Object);

            // Act
            var result = manager.IsNotTakenUserNameInDatabase("existingusername");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsNotTakenUserNameInDatabase_WithNonExistingUsername_ReturnsTrue()
        {
            // Arrange
            var accountCreationServiceMock = new Mock<IAccountCreationService>();
            accountCreationServiceMock.Setup(x => x.IsUserRegistered("nonexistingusername")).Returns(new Response { HasError = false });

            var manager = new UserAdministrationManager(accountCreationServiceMock.Object);

            // Act
            var result = manager.IsNotTakenUserNameInDatabase("nonexistingusername");

            // Assert
            Assert.True(result);
        }

        
    }

}


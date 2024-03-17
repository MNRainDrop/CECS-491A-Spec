namespace TeamSpecs.RideAlong.TestingLibrary.ModelTests; 
using TeamSpecs.RideAlong.Model;

public class VehicleProfileModelShould
{
    #region Get Functions
    [Fact]
    public void VehicleProfileModel_GetVIN_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var expectedvin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(expectedvin, ownerID, licensePlate);

        // Act
        var actualvin = vehicle.VIN;

        // Assert
        Assert.Equal(expectedvin, actualvin);
    }

    [Fact]
    public void VehicleProfileModel_GetOwnerID_NoParametersPassedIn_OneLongReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long expectedOwnerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, expectedOwnerID, licensePlate);

        // Act
        var actualOwnerID = vehicle.Owner_UID;

        // Assert
        Assert.Equal(actualOwnerID, expectedOwnerID);
    }

    [Fact]
    public void VehicleProfileModel_GetLicensePlate_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long ownerID = 1;
        var expectedLicensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, expectedLicensePlate);

        // Act
        var actualLicensePlate = vehicle.LicensePlate;

        // Assert
        Assert.Equal(actualLicensePlate, expectedLicensePlate);
    }

    [Fact]
    public void VehicleProfileModel_GetMake_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long ownerID = 1;
        var licensePlate = "licenseplate";
        var make = "testMake";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate, make);

        // Act
        var actual = vehicle.Make;

        // Assert
        Assert.Equal(actual, make);
    }

    [Fact]
    public void VehicleProfileModel_GetModel_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long ownerID = 1;
        var licensePlate = "licenseplate";
        var model = "testModel";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate, model: model);

        // Act
        var actual = vehicle.Model;

        // Assert
        Assert.Equal(actual, model);
    }

    [Fact]
    public void VehicleProfileModel_GetYear_NoParametersPassedIn_OneIntReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long ownerID = 1;
        var licensePlate = "licenseplate";
        var year = 1999;
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate, year: year);

        // Act
        var actual = vehicle.Year;

        // Assert
        Assert.Equal(actual, year);
    }

    [Fact]
    public void VehicleProfileModel_GetName_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        long ownerID = 1;
        var licensePlate = "licenseplate";
        var name = "testName";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate, name: name);

        // Act
        var actual = vehicle.Name;

        // Assert
        Assert.Equal(actual, name);
    }
    #endregion

    #region Set Functions
    [Fact]
    public void VehicleProfileModel_SetVIN_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = "11111111111111111";

        // Act
        vehicle.VIN = newValue;

        // Assert
        Assert.Equal(vehicle.VIN, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetOwnerID_OneLongPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        long newValue = 1;

        // Act
        vehicle.Owner_UID = newValue;

        // Assert
        Assert.Equal(vehicle.Owner_UID, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetLicensePlate_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = "11111111111111111";

        // Act
        vehicle.LicensePlate = newValue;

        // Assert
        Assert.Equal(vehicle.LicensePlate, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetMake_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = "11111111111111111";

        // Act
        vehicle.Make = newValue;

        // Assert
        Assert.Equal(vehicle.Make, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetModel_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = "11111111111111111";

        // Act
        vehicle.Model = newValue;

        // Assert
        Assert.Equal(vehicle.Model, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetYear_OneIntPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = 1001;

        // Act
        vehicle.Year = newValue;

        // Assert
        Assert.Equal(vehicle.Year, newValue);
    }

    [Fact]
    public void VehicleProfileModel_SetName_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var ownerID = 1;
        var licensePlate = "licenseplate";
        var vehicle = new VehicleProfileModel(vin, ownerID, licensePlate);
        var newValue = "11111111111111111";

        // Act
        vehicle.Name = newValue;

        // Assert
        Assert.Equal(vehicle.Name, newValue);
    }
    #endregion
}

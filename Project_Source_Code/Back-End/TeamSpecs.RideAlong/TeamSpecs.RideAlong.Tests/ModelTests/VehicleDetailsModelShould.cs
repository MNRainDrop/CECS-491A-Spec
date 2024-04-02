namespace TeamSpecs.RideAlong.TestingLibrary.ModelTests;
using TeamSpecs.RideAlong.Model;

public class VehicleDetailsModelShould
{
    [Fact]
    public void VehicleDetailsModel_GetVIN_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var expectedValue = vin;
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        var actualValue = vehicle.VIN;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void VehicleDetailsModel_GetColor_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var expectedValue = color;
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        var actualValue = vehicle.Color;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void VehicleDetailsModel_GetDescription_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var expectedValue = description;
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        var actualValue = vehicle.Description;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void VehicleDetailsModel_SetVIN_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var updatedValue = "TESTING VALUES";
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        vehicle.VIN = updatedValue;

        // Assert
        Assert.Equal(vehicle.VIN, updatedValue);
    }

    [Fact]
    public void VehicleDetailsModel_SetColor_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var updatedValue = "TESTING VALUES";
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        vehicle.Color = updatedValue;

        // Assert
        Assert.Equal(vehicle.Color, updatedValue);
    }

    [Fact]
    public void VehicleDetailsModel_SetDescription_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var color = "testColor";
        var description = "testDescription";

        var updatedValue = "TESTING VALUES";
        var vehicle = new VehicleDetailsModel(vin, color, description);

        // Act
        vehicle.Description = updatedValue;

        // Assert
        Assert.Equal(vehicle.Description, updatedValue);
    }
}

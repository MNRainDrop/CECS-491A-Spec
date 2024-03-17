namespace TeamSpecs.RideAlong.TestingLibrary.ModelTests;
using TeamSpecs.RideAlong.Model;


public class ViewableDetailsModelShould
{
    #region Gets
    [Fact]
    public void ViewableDetailsModel_GetVIN_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var expectedValue = vin;

        // Act
        var actualvin = vehicle.VIN;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetMakeIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, makeIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Make_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetModelIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, modelIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Model_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetYearIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, yearIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Year_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetColorIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, colorIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Color_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetNameIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, nameIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Name_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetVINIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, vinIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.VIN_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }

    [Fact]
    public void ViewableDetailsModel_GetPhotoIsViewable_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var expectedValue = 1;
        var vehicle = new ViewableDetailsModel(vin, photoIsViewable: expectedValue);

        // Act
        var actualvin = vehicle.Photo_isViewable;

        // Assert
        Assert.Equal(expectedValue, actualvin);
    }
    #endregion

    #region Sets
    [Fact]
    public void ViewableDetailsModel_SetVIN_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = "000000000000";

        // Act
        vehicle.VIN = newValue;

        // Assert
        Assert.Equal(vehicle.VIN, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetMakeIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Make_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Make_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetModelIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Model_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Model_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetYearIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Year_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Year_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetColorlIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Color_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Color_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetNameIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Name_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Name_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetVinIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.VIN_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.VIN_isViewable, newValue);
    }

    [Fact]
    public void ViewableDetailsModel_SetPhotoIsVisible_OneStringPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var vin = "12345678901234567";
        var vehicle = new ViewableDetailsModel(vin);

        var newValue = 1;

        // Act
        vehicle.Photo_isViewable = newValue;

        // Assert
        Assert.Equal(vehicle.Photo_isViewable, newValue);
    }
    #endregion
}

namespace TeamSpecs.RideAlong.TestingLibrary.ModelTests;
using TeamSpecs.RideAlong.Model;

public class SearchModelShould
{
    #region Gets
    [Fact]
    public void SearchModelShould_GetCollection_NoParametersPassedIn_OneCollectionReturned_Pass()
    {
        // Arrange
        var list = new List<object>();
        var search = new SearchModel(list);

        var expectedValue = list.GetType();

        // Act
        var actualValue = search.SearchParameters.GetType();

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SearchModelShould_GetFirstElementInCollection_NoParametersPassedIn_OneStringReturned_Pass()
    {
        // Arrange
        var list = new List<object>();
        var testObject = "test Object";
        list.Add(testObject);
        var search = new SearchModel(list);

        var expectedValue = testObject;

        // Act
        var actualValue = search.SearchParameters.First<object>();
        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SearchModelShould_GetAllElementInCollection_NoParametersPassedIn_MultipleObjectsReturned_Pass()
    {
        // Arrange
        var list = new List<object>();
        var testObject = "test Object";
        list.Add(testObject);
        list.Add(testObject);
        list.Add(testObject);
        list.Add(testObject);
        list.Add(testObject);
        var search = new SearchModel(list);

        var expectedValue = testObject;

        foreach (var item in search.SearchParameters)
        {
            //Act
            var actualValue = item;

            //Assert
            Assert.Equal(actualValue, expectedValue);
        }
    }
    #endregion

    #region Sets
    [Fact]
    public void SearchModelShould_SetCollection_OneObjectPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var list = new List<object>();
        var search = new SearchModel(list);

        var newValue = new HashSet<object>();

        // Act
        search.SearchParameters = newValue;

        // Assert
        Assert.Equal(search.SearchParameters.GetType(), newValue.GetType());
    }

    [Fact]
    public void SearchModelShould_SetFirstElementInCollection_OneObjectPassedIn_NoValuesReturned_Pass()
    {
        // Arrange
        var list = new List<object>();
        var testObject = "test Object";
        list.Add(testObject);
        var search = new SearchModel(list);

        var expectedValue = "New test Object";

        // Act
        var temp = search.SearchParameters.First<object>();
        search.SearchParameters.Remove(temp);
        temp = expectedValue;
        search.SearchParameters.Add(temp);
        
        // Assert
        Assert.Equal(expectedValue, search.SearchParameters.First<object>());
    }
    #endregion
}

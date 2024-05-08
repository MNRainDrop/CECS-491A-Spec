using Microsoft.Data.SqlClient;
using ScrapYourCarLibrary;
using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.DataAccess.DataAccessObjects;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ScrapYourCarLibrary;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary;
public class ScrapYourCarShould
{
    #region Local Variables
    IHashService _hasher;
    IGenericDAO _dao;
    IGenericDAO _failDao;
    ILogTarget _logTarget;
    ILogService _logger;
    IBuyTarget _bTarget;
    IBuyRequestService _bService;
    IPartsTarget _pTarget;
    IPartsService _pService;
    IListingSearchTarget _sTarget;
    IListingSearchService _sService;
    IScrapYourCarManager _scrapYourCarManager;
    string testUser;
    string testUserHash;
    long testUserUID;
    #endregion
    public ScrapYourCarShould()
    {
        Dispose();
        _hasher = new HashService();
        _dao = new SqlServerDAO(new ConfigServiceJson());
        _failDao = new FailDao();
        _logTarget = new SqlDbLogTarget(_dao);
        _logger = new LogService(_logTarget, _hasher);
        _bTarget = new SqlBuyTarget(_dao, _logger);
        _bService = new BuyRequestService(_bTarget);
        _pTarget = new SqlPartsTarget(_dao, _logger);
        _pService = new PartsService(_pTarget);
        _sTarget = new SqlListingSearchTarget(_dao, _logger);
        _sService = new ListingSearchService(_sTarget);
        _scrapYourCarManager = new ScrapYourCarManager(_pService, _bService, _sService);

        testUser = "SYCTestUser";
        testUserHash = "SYCTestUserHash";

        // This is a set of sql designed to pre prep the database for accurate testing 
        #region Sql Statement
        var testSql =
            $"DECLARE @UserName VARCHAR(50);" +
$"SET @UserName = 'SYCTestUser';" +
$"DECLARE @UserHash VARCHAR(64);" +
$"SET @UserHash = 'SYCTestUserHash';" +
$"DECLARE @UserID BIGINT;" +

$"INSERT INTO UserAccount (UserName, Salt, UserHash)" +
$"VALUES (@UserName, 123456, @UserHash);" +

$"SELECT @UserID = UID FROM UserAccount WHERE UserName = @UserName;" +

$"DECLARE @partUID1 BIGINT;" +
$"DECLARE @partUID2 BIGINT;" +
$"DECLARE @partUID3 BIGINT;" +
$"DECLARE @partUID4 BIGINT;" +
$"DECLARE @partUID5 BIGINT;" +
$"DECLARE @partUID6 BIGINT;" +
$"DECLARE @partUID7 BIGINT;" +
$"DECLARE @partUID8 BIGINT;" +
$"DECLARE @partUID9 BIGINT;" +
$"DECLARE @partUID10 BIGINT;" +
$"DECLARE @partUID11 BIGINT;" +
$"DECLARE @partUID12 BIGINT;" +
$"DECLARE @partUID13 BIGINT;" +
$"DECLARE @partUID14 BIGINT;" +
$"DECLARE @partUID15 BIGINT;" +
$"DECLARE @partUID16 BIGINT;" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName1', 'abcdefg', 'Honda', 'Civic', 2020, 'associatedVin');" +
$"SELECT @partUID1 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName1';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID1, 20.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName2', 'abcdefg', 'Honda', 'Civic', 2018, 'associatedVin');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName3', 'abcdefg', 'Honda', 'Civic', 2020, 'associatedVin');" +
$"SELECT @partUID3 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName3';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID3, 40.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName4', 'abcdefg', 'Honda', 'Civic', 2018, 'associatedVin');" +
$"SELECT @partUID4 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName4';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID4, 40.5, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName5', '123456', 'Honda', 'Civic', 2020, 'associatedVin');" +
$"SELECT @partUID5 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName5';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID5, 60.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName6', '123456', 'Honda', 'Civic', 2018, 'associatedVin');" +
$"SELECT @partUID6 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName6';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID6, 60.5, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName7', '123456', 'Honda', 'Civic', 2020, 'associatedVin');" +
$"SELECT @partUID7 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName7';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID7, 80.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName8', '123456', 'Honda', 'Civic', 2018, 'associatedVin');" +
$"SELECT @partUID8 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName8';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID8, 80.5, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName9', 'abcdefg', 'Toyota', 'Corolla', 2020, 'associatedVin');" +
$"SELECT @partUID9 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName9';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID9, 100.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName10', 'abcdefg', 'Toyota', 'Corolla', 2018, 'associatedVin');" +
$"SELECT @partUID10 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName10';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID10, 100.5, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName11', 'abcdefg', 'Toyota', 'Corolla', 2020, 'associatedVin');" +
$"SELECT @partUID11 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName11';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID11, 120.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName12', 'abcdefg', 'Toyota', 'Corolla', 2018, 'associatedVin');" +
$"SELECT @partUID12 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName12';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID12, 120.5, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName13', '123456', 'Toyota', 'Corolla', 2020, 'associatedVin');" +
$"SELECT @partUID13 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName13';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID13, 20.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName14', '123456', 'Toyota', 'Corolla', 2018, 'associatedVin');" +
$"SELECT @partUID14 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName14';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID14, 20.0, 'Land Of Setting Moon');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName15', '123456', 'Toyota', 'Corolla', 2020, 'associatedVin');" +
$"SELECT @partUID15 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName15';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID15, 20.0, 'Land Of Rising Sun');" +

$"INSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName16', '123456', 'Honda', 'Civic', 2018, 'associatedVin');" +
$"SELECT @partUID16 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName16';" +
$"INSERT INTO Listings (partUID, price, description) VALUES (@partUID16, 20.0, 'Land Of Setting Moon');";
        #endregion
        _dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(testSql, null) });

        var getUIDSql = $"SELECT UID FROM UserAccount WHERE UserName = '{testUser}';";
        testUserUID = (long)(_dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUIDSql, null) })).First()[0];
        // The previous executes, gets back a list of rows, checks the first row, gets the first value, then typecasts to long
    }
    private void Dispose()
    {
        var undoTestSql = $"DELETE FROM UserAccount WHERE UserName = '{testUser}';";
        try
        {
            _dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoTestSql, null) });
        } catch (Exception ex)
        {

        }
        
    }
    // Parts
    [Fact]
    public void PartsServiceShould_CreatePart_Pass()
    {
        #region Arrange        
        ICarPart part = new CarPart();
        List<ICarPart> partList = new List<ICarPart> { part };
        part.ownerUID = testUserUID;
        part.partName = "InsertTestPart1";

        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        #endregion


        #region Act
        timer.Start();
        response = _pService.CreateParts(partList);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(response.HasError, expectedHasError);
        Assert.Equal(response.ErrorMessage, expectedErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion

    }
    [Fact]
    public void PartsServiceShould_RetrieveUsersParts_Pass()
    {
        #region Arrange        
        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int expectedReturnValueCount = 16;
        #endregion


        #region Act
        timer.Start();
        response = _pService.GetUserParts(testUserUID);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(response.HasError, expectedHasError);
        Assert.Equal(response.ErrorMessage, expectedErrorMessage);
        Assert.Equal(response.ReturnValue!.Count, expectedReturnValueCount);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_RetrieveMatching_Pass()
    {
        #region Arrange
        ICarPart part1 = new CarPart();
        part1.ownerUID = testUserUID;
        part1.partName = "PartName1";
        ICarPart part2 = new CarPart();
        part2.ownerUID = testUserUID;
        part2.partName = "PartName2";


        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int expectedReturnValueCount = 2;
        #endregion


        #region Act
        timer.Start();
        response = _pService.GetMatchingParts(new List<ICarPart> { part1, part1 });
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(response.HasError, expectedHasError);
        Assert.Equal(response.ErrorMessage, expectedErrorMessage);
        Assert.Equal(response.ReturnValue!.Count, expectedReturnValueCount);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_DeletePart_Pass()
    {
        #region Arrange        
        ICarPart part = new CarPart();
        part.ownerUID = testUserUID;
        part.partName = "PartName2";
        List<ICarPart> partList = new List<ICarPart> { part };

        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        #endregion


        #region Act
        timer.Start();
        response = _pService.RemoveParts(partList);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(response.HasError, expectedHasError);
        Assert.Equal(response.ErrorMessage, expectedErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_ReturnErrorWhenNoDatabase_Fail()
    {
        #region Arrange
        IPartsTarget badTarget = new SqlPartsTarget(new FailDao(), _logger);
        IPartsService badService = new PartsService(badTarget);
        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = true;
        string? expectedErrorMessage = "Format of the initialization string does not conform to specification starting at index 0.";
        #endregion

        #region Act
        timer.Start();
        response = badService.GetUserParts(testUserUID);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    // Listings
    [Fact]
    public void PartsServiceShould_CreateListing_Pass()
    {
        #region Arrange
        ICarPart part = new CarPart();
        part.ownerUID = testUserUID;
        part.partName = "PartName2";

        // Getting test object
        IResponse response;

        // Create a test Listing
        IListing listing = new Listing(part, (float)200.0, "This is the most solid of test objects");

        var timer = new Stopwatch();

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        #endregion


        #region Act
        timer.Start();
        response = _pService.AddListingToPart(listing);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_RetrieveUsersListings_Pass()
    {
        #region Arrange
        // Getting test object
        IResponse response;

        var timer = new Stopwatch();

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int expectedReturValue = 15;
        #endregion


        #region Act
        timer.Start();
        response = _pService.RetrieveUserListings(testUserUID);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.Equal(expectedReturValue, response.ReturnValue!.Count);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_RetrievePartListings_Pass()
    {
        #region Arrange
        // Setting up two test car parts
        ICarPart part1 = new CarPart();
        ICarPart part2 = new CarPart();
        part1.ownerUID = testUserUID;
        part2.ownerUID = testUserUID;
        part1.partName = "PartName1";
        part2.partName = "PartName3";
        List<object> partsList = (List<object>)_pService.GetMatchingParts(new List<ICarPart>() { part1, part2 }).ReturnValue!;
        part1 = (ICarPart)partsList.ElementAt(0);
        part2 = (ICarPart)partsList.ElementAt(1);
        List<ICarPart> validParts = new List<ICarPart> { part1, part2 };
        // Getting test object
        IResponse response;

        var timer = new Stopwatch();

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int expectedReturValue = 2;
        #endregion

        #region Act
        timer.Start();
        response = _pService.RetrievePartListings(validParts);
        timer.Stop();
        Dispose();
        #endregion


        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.Equal(expectedReturValue, response.ReturnValue!.Count);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_UpdateListing_Pass()
    {
        #region Arrange
        ICarPart part = new CarPart();
        part.ownerUID = testUserUID;
        part.partName = "PartName15";
        part = (ICarPart)_pService.GetMatchingParts(new List<ICarPart>() { part }).ReturnValue!.First();

        // GetListing
        IListing listing = (IListing)_pService.RetrievePartListings(new List<ICarPart> { part }).ReturnValue!.First();

        // Getting test object
        IResponse response;

        var timer = new Stopwatch();

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        string expectedDescription = "Land of the falling moon";
        listing.description = expectedDescription;
        #endregion


        #region Act
        timer.Start();
        response = _pService.UpdateListing(listing);
        timer.Stop();
        IListing updatedListing = (IListing)_pService.RetrievePartListings(new List<ICarPart> { part }).ReturnValue!.First();
        Dispose();
        #endregion



        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.Equal(expectedDescription, updatedListing.description);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void PartsServiceShould_RemoveListing_Pass()
    {
        #region Arrange
        ICarPart part = new CarPart();
        part.ownerUID = testUserUID;
        part.partName = "PartName16";
        var validatedPart = (ICarPart)_pService.GetMatchingParts(new List<ICarPart>() { part }).ReturnValue!.First();

        // GetListing
        IListing listing = (IListing)_pService.RetrievePartListings(new List<ICarPart> { validatedPart }).ReturnValue!.First();
        listing.part = part;

        // Getting test object
        IResponse response;

        var timer = new Stopwatch();

        // Expected values
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int badListingAttemptCount = 0;
        #endregion


        #region Act
        timer.Start();
        response = _pService.DeleteListings(new List<IListing> { listing });
        timer.Stop();
        var badListingAttempt = _pService.RetrievePartListings(new List<ICarPart> { validatedPart });
        Dispose();
        #endregion

        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.Equal(badListingAttemptCount, badListingAttempt.ReturnValue!.Count);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    // Search
    [Fact]
    public void SearchServiceShould_GetCorrectValuesAndOrder_Pass()
    {
        #region Arrange
        ISearchParameters searchBy = new SearchParameters(0, 15, make: "honda");
        IResponse response;
        var timer = new Stopwatch();
        bool expectedHasError = false;
        string? expectedErrorMessage = null;
        int expectedResultCount = 7;
        #endregion

        #region Act
        timer.Start();
        response = _sService.RetrieveListingsBySearch(searchBy);
        timer.Start();
        Dispose();
        #endregion

        #region Assert
        Assert.Equal(expectedHasError, response.HasError);
        Assert.Equal(expectedErrorMessage, response.ErrorMessage);
        Assert.Equal(expectedResultCount, response.ReturnValue!.Count());
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        #endregion
    }
    [Fact]
    public void SearchServiceShould_PaginateProperly_Pass()
    {

    }
    [Fact]
    public void SearchServiceShould_ReturnNoneIfNoneMatch_Pass()
    {

    }
    [Fact]
    public void SearchServiceShould_ReturnNoneIfSearchHasNone_Pass()
    {

    }
    // Buy Request
    [Fact]
    public void BuyRequestServiceShould_CreateBuyRequest_Pass()
    {

    }
    [Fact]
    public void BuyRequestServiceShould_RetrieveUsersBuyRequests_Pass()
    {

    }
    [Fact]
    public void BuyRequestServiceShould_RetrieveIncomingBuyRequests_Pass()
    {

    }
    [Fact]
    public void BuyRequestServiceShould_RetrieveMatchingBuyRequests_Pass()
    {

    }
    [Fact]
    public void BuyRequestServiceShould_UpdateBuyRequests_Pass()
    {

    }
    [Fact]
    public void BuyRequestServiceShould_DeleteBuyRequests_Pass()
    {

    }

}
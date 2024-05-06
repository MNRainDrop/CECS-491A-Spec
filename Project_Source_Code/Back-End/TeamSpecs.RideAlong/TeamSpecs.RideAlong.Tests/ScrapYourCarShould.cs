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
        var testSql = $"DECLARE @UserName VARCHAR(50);\r\nSET @UserName = '{testUser}';\r\nDECLARE @UserHash VARCHAR(64);\r\nSET @UserHash = '{testUserHash}';\r\nDECLARE @UserID BIGINT;\r\n\r\nINSERT INTO UserAccount (UserName, Salt, UserHash)\r\nVALUES (@UserName, 123456, @UserHash);\r\n\r\nSELECT @UserID = UID FROM UserAccount WHERE UserName = @UserName;\r\n\r\nDECLARE @partUID1 BIGINT;\r\nDECLARE @partUID2 BIGINT;\r\nDECLARE @partUID3 BIGINT;\r\nDECLARE @partUID4 BIGINT;\r\nDECLARE @partUID5 BIGINT;\r\nDECLARE @partUID6 BIGINT;\r\nDECLARE @partUID7 BIGINT;\r\nDECLARE @partUID8 BIGINT;\r\nDECLARE @partUID9 BIGINT;\r\nDECLARE @partUID10 BIGINT;\r\nDECLARE @partUID11 BIGINT;\r\nDECLARE @partUID12 BIGINT;\r\nDECLARE @partUID13 BIGINT;\r\nDECLARE @partUID14 BIGINT;\r\nDECLARE @partUID15 BIGINT;\r\nDECLARE @partUID16 BIGINT;\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName1', 'abcdefg', 'Honda', 'Civic', 2020, 'associatedVin');\r\nSELECT @partUID1 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName1';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID1, 20.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName2', 'abcdefg', 'Honda', 'Civic', 2018, 'associatedVin');\r\nSELECT @partUID2 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName2';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID2, 20.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName3', 'abcdefg', 'Honda', 'Civic', 2020, 'associatedVin');\r\nSELECT @partUID3 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName3';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID3, 40.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName4', 'abcdefg', 'Honda', 'Civic', 2018, 'associatedVin');\r\nSELECT @partUID4 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName4';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID4, 40.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName5', '123456', 'Honda', 'Civic', 2020, 'associatedVin');\r\nSELECT @partUID5 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName5';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID5, 60.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName6', '123456', 'Honda', 'Civic', 2018, 'associatedVin');\r\nSELECT @partUID6 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName6';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID6, 60.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName7', '123456', 'Honda', 'Civic', 2020, 'associatedVin');\r\nSELECT @partUID7 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName7';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID7, 80.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName8', '123456', 'Honda', 'Civic', 2018, 'associatedVin');\r\nSELECT @partUID8 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName8';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID8, 80.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName9', 'abcdefg', 'Toyota', 'Corolla', 2020, 'associatedVin');\r\nSELECT @partUID9 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName9';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID9, 100.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName10', 'abcdefg', 'Toyota', 'Corolla', 2018, 'associatedVin');\r\nSELECT @partUID10 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName10';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID10, 100.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName11', 'abcdefg', 'Toyota', 'Corolla', 2020, 'associatedVin');\r\nSELECT @partUID11 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName11';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID11, 120.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName12', 'abcdefg', 'Toyota', 'Corolla', 2018, 'associatedVin');\r\nSELECT @partUID12 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName12';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID12, 120.5, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName13', '123456', 'Toyota', 'Corolla', 2020, 'associatedVin');\r\nSELECT @partUID13 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName13';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID13, 20.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName14', '123456', 'Toyota', 'Corolla', 2018, 'associatedVin');\r\nSELECT @partUID14 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName14';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID14, 20.0, 'Land Of Setting Moon');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName15', '123456', 'Toyota', 'Corolla', 2020, 'associatedVin');\r\nSELECT @partUID15 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName15';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID15, 20.0, 'Land Of Rising Sun');\r\n\r\nINSERT INTO Parts(ownerUID, partName, partNumber, make, model, year, associatedVin) VALUES (@UserID, 'PartName16', '123456', 'Honda', 'Civic', 2018, 'associatedVin');\r\nSELECT @partUID16 = partUID FROM Parts WHERE ownerUID = @UserID AND partName = 'PartName16';\r\nINSERT INTO Listings (partUID, price, description) VALUES (@partUID16, 20.0, 'Land Of Setting Moon');";
        _dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(testSql, null) });

        var getUIDSql = $"SELECT UID FROM UserAccount WHERE UserName = '{testUser}';";
        testUserUID = (long)(_dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUIDSql, null) })).First()[0];
        // The previous executes, gets back a list of rows, checks the first row, gets the first value, then typecasts to long
    }
    private void Dispose()
    {
        var undoTestSql = $"DELETE FROM UserAccount WHERE UserName = '{testUser}';";
        _dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoTestSql, null) });
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
        int expectedReturnValueCount = 5;
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
        part2.partName = "DeleteMe1";


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
        part.partName = "DeleteMe2";
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
        IPartsTarget badTarget = new SqlPartsTarget(new FailDao(), _logger);
        IPartsService badService = new PartsService(badTarget);
        var timer = new Stopwatch();
        IResponse response;

        // Expected values
        bool expectedHasError = true;
        string? expectedErrorMessage = "Format of the initialization string does not conform to specification starting at index 0.";

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
        int expectedReturValue = 3;
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
        part2.partName = "WrongName1";
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
        part.partName = "WrongName1";
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
        part.partName = "WrongName1";
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
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.TestingLibrary;
public class ScrapYourCarShould
{
    public ScrapYourCarShould()
    {
        IGenericDAO _dao = new SqlServerDAO();
    }
    ~ScrapYourCarShould()
    {

    }
    // Parts
    [Fact]
    public void PartsServiceShould_CreatePart_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_RetrieveUsersParts_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_RetrieveMatching_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_DeletePart_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_ReturnErrorWhenNoDatabase_Fail()
    {

    }
    // Listings
    [Fact]
    public void PartsServiceShould_CreateListing_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_RetrieveUsersListings_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_RetrievePartListings_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_UpdateListing_Pass()
    {

    }
    [Fact]
    public void PartsServiceShould_RemoveListing_Pass()
    {

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
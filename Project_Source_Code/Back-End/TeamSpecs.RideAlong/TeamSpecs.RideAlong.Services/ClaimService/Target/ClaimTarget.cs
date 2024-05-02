using Azure;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public class ClaimTarget : IClaimTarget
{
    private readonly IGenericDAO _dao;
    public ClaimTarget(IGenericDAO dao)
    {
        _dao = dao;
    }
    public IResponse CreateClaimSQL(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
    {
        /* INSERT INTO UserClaim (UID, ClaimID, ClaimScope)
         * VALUES (user.uid, (SELECT CaimID WHERE Claim = claim), scope)
         */
        #region Default sql setup
        var defaultCommandSql = "INSERT ";
        var defaultIntoSql = "INTO UserClaim (";
        var defaultValuesSql = "VALUES (";
        #endregion



        foreach(var claim in claims)
        {
            var commandSql = defaultCommandSql;
            var intoSql = defaultIntoSql;
            var valuesSql = defaultValuesSql;

            intoSql +=
        }

        
    }

    public IResponse DeleteAllUserClaimsSQL(IAccountUserModel user)
    {
        throw new NotImplementedException();
    }

    public IResponse DeleteUserClaimSQL(IAccountUserModel user, string claim)
    {
        throw new NotImplementedException();
    }

    public IResponse ModifyUserClaimSql(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
    {
        throw new NotImplementedException();
    }
}

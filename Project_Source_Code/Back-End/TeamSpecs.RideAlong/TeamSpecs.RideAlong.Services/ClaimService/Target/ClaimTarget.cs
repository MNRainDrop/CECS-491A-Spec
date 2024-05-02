﻿using Microsoft.Data.SqlClient;
using System.Security.Claims;
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
        var defaultSql = "INSERT INTO UserClaim (UID, ClaimID, ClaimScope) VALUES (@UID, (SELECT ClaimID FROM Claim WHERE Claim = @CLAIM), @SCOPE)";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        #endregion

        try
        {
            if (claims is not null)
            {
                foreach (var claim in claims)
                {
                    // create new hash set of SqlParameters
                    var parameters = new HashSet<SqlParameter>()
                    {
                        new SqlParameter("@UID", user.UserId),
                        new SqlParameter("@CLAIM", claim.Key),
                        new SqlParameter("@SCOPE", claim.Value)
                    };

                    sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(defaultSql, parameters));
                }
            }
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate SQL statement to create user claim. ";
            return response;
        }

        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not write user claim to database. ";
            return response;
        }

        response.HasError = false;
        return response;

    }

    public IResponse DeleteAllUserClaimsSQL(IAccountUserModel user)
    {
        /* DELETE FROM UserClaim WHERE UID = user.id
         */
        #region Default sql setup
        var defaultSql = "DELETE FROM UserClaim WHERE UID = @UID";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        #endregion

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UID", user.UserId),
            };
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(defaultSql, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate SQL statement to create user claim. ";
            return response;
        }

        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not write user claim to database. ";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse DeleteUserClaimSQL(IAccountUserModel user, string claim, string? scope)
    {
        /* DELETE FROM UserClaim WHERE UID = user.id AND Claim = claim AND Scope = scope
         */
        #region Default sql setup
        var defaultSql = "DELETE FROM UserClaim WHERE UID = @UID AND ClaimID = (SELECT ClaimID FROM CLAIM WHERE Claim = @CLAIM) ";
        var scopeSql = "AND ClaimScope = @SCOPE";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        #endregion

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UID", user.UserId),
                new SqlParameter("@CLAIM", claim)
            };
            if (!string.IsNullOrEmpty(scope))
            {
                defaultSql += scopeSql;
                parameters.Add(new SqlParameter("@SCOPE", scope));
            }
            
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(defaultSql, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate SQL statement to create user claim. ";
            return response;
        }

        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not write user claim to database. ";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse ModifyUserClaimSql(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
    {
        throw new NotImplementedException();
    }
}

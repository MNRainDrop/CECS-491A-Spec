using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserCreationTarget
{
    private readonly IGenericDAO _dao;


    public SqlDbUserCreationTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

}
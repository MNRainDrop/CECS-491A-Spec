using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;
namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserTarget : IUserTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbUserTarget(IGenericDAO dao)
    {
        _dao = dao;
    }
    public IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<string, string> userClaims)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {

            // convert user model into sql

            // convrt user claims into sql
            var returnValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>
            {
                returnValue
            };
        }
        catch
        {
            response.HasError = true;
        }
        return response;
    }

    public IResponse DeleteUserAccountSql(string userName)
    {
        throw new NotImplementedException();
    }

    public IResponse ModifyUserProfileSql(string userName, IDictionary<string, string> something)
    {
        throw new NotImplementedException();
    }

    public IResponse RecoverUserAccountSql(string userName)
    {
        throw new NotImplementedException();
    }
}
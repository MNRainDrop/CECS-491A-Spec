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

    public IResponse ModifyUserProfileSql(string userName, ProfileUserModel profile)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // Get properties of ProfileUserModel object with reflection
            var properties = profile.GetType().GetProperties();

            // Iterates thru. each property and generates a SQL command
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(profile, null);

                // Create SQL command for each property
                var sqlCommand = $"UPDATE UserProfile SET {propertyName} = @{propertyName} WHERE UserName = @UserName";
                var sqlParameters = new HashSet<SqlParameter>
                {
                    new SqlParameter($"@{propertyName}", propertyValue),
                    new SqlParameter("@UserName", propertyValue)
                };

                sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(sqlCommand, sqlParameters));
            }

            // Call DAO
            // response.ReturnValue = new obj?
            // 

        }
        catch
        {

        }
        return response;
    }

    public IResponse RecoverUserAccountSql(string userName)
    {
        throw new NotImplementedException();
    }
}
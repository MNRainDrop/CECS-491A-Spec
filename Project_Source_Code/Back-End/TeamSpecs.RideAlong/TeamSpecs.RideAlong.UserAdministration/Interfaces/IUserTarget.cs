﻿using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public interface IUserTarget
{
    IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<int, string> userClaims);
    IResponse DeleteUserAccountSql(string userName);
    IResponse ModifyUserProfileSql(string userName, IProfileUserModel profile);
    IResponse EnableUserAccountSql(string userName);
    IResponse DisableUserAccountSql(string userName);
    IResponse RecoverUserAccountSql(string userName);
    IResponse GetUpdatedUserSql(IAccountUserModel userModel);
    IResponse PostUpdatedUserSql(IAccountUserModel userAccount, string address, string name, string phone, string accountType);
}
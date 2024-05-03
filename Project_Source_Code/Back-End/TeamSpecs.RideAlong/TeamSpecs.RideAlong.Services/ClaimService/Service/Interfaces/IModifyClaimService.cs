﻿
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IModifyClaimService
{
    IResponse ModifyUserClaim(IAccountUserModel user, KeyValuePair<string, string> currClaim, KeyValuePair<string, string> newClaim);
}
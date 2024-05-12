using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountRetrievalManager : IAccountRetrievalManager
    {
        private readonly IAccountRetrievalService _accountRetrievalService;

        public AccountRetrievalManager(IAccountRetrievalService accountRetrievalService) { 
            _accountRetrievalService = accountRetrievalService;
        
        }

        public IResponse RetrieveAccount(long uid) {
            IResponse response; 
            response = _accountRetrievalService.RetrieveAccount(uid);
            return response;
        
        }
    }
}

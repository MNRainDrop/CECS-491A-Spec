using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.PaginationModel;
using TeamSpecs.RideAlong.Model.ServiceLogModel;


namespace TeamSpecs.RideAlong.ServiceLog.Interfaces
{
    public interface IServiceLogService
    {
        public IResponse CreateServiceLog(IServiceLogModel serviceLogModel, IAccountUserModel userAccount);

        public IResponse ModifyServiceLog(IServiceLogModel serviceLogModel, IAccountUserModel userAccount);

        public IResponse DeleteServiceLog(IAccountUserModel userAccount);

        public IResponse RetrieveServiceLogs(IAccountUserModel userAccount, IPaginationModel page, string vin);

        public IResponse CreateMantainenceReminder(IAccountUserModel userAccount);
    }
}

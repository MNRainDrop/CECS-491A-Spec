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
    public interface ISqlDbServiceLogTarget
    {
        public IResponse GenerateCreateServiceLogSql(IServiceLogModel serviceLog);

        public IResponse GenerateModifyServiceLogSql(IServiceLogModel serviceLog);

        public IResponse GenerateDeleteServiceLogSql();

        public IResponse GenerateRetrieveServiceLogSql(IPaginationModel page, string vin);

        public IResponse GenerateCreateMantainenceReminderSql();
        
    }
}

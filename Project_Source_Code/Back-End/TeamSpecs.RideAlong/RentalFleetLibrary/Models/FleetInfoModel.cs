using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.RentalFleetLibrary.Models
{
    public class FleetInfoModel
    {
        public FleetInfoModel(string pVin, int pStatus, string? pStatusInfo, DateTime? pexpectedReturnDate)
        {
            vin = pVin;
            status = pStatus;
            statusInfo = pStatusInfo;
            expectedReturnDate = pexpectedReturnDate;
        }
        public string vin { get; set; }
        public int status { get; set; }
        public string? statusInfo { get; set; }
        public DateTime? expectedReturnDate { get; set; }
    }
}

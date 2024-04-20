using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.ServiceLogModel
{
    public  interface IServiceLogModel
    {
        public int? ServiceLogID { get; set; }
        public string Part { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int? Mileage { get; set; }
        public string VIN { get; set; }
    }
}

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.RentalFleetLibrary.Models
{
    public class FleetFullModel
    {
        public FleetFullModel(
            string vin, string? make, string? model, int? year, DateTime dateCreated, string color, int? status, string statusInfo, DateTime expectedReturnDate)
        {
            this.vin = vin;
            this.make = make;
            this.model = model;
            this.year = year;
            this.dateCreated = dateCreated;
            this.color = color;
            this.status = status;
            this.statusInfo = statusInfo;
            this.expectedReturnDate = expectedReturnDate;
        }

        public string vin {  get; set; }
        public string? make { get; set; }
        public string? model { get; set; }
        public int? year { get; set; }
        public DateTime dateCreated { get; set; }
        public string color { get; set; }
        public int? status { get; set; }
        public string? statusInfo { get; set; }
        public DateTime? expectedReturnDate { get; set; }
        
    }
}

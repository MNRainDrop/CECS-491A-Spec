using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.PaginationModel
{
    public class PaginationModel: IPaginationModel
    {
        PaginationModel(int pageSize, int pageNumber, string[] filter)
        {
            this.pageSize = pageSize;
            this.pageNumber = pageNumber;
            this.filter = filter;
        }

        public int pageSize {  get; set; }

        public int pageNumber { get; set; }

        public required string[] filter { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.PaginationModel
{
    public interface IPaginationModel
    {
        public int pageSize { get; set; }

        public int pageNumber { get; set; }

        public string[] filter { get; set; }
    }
}

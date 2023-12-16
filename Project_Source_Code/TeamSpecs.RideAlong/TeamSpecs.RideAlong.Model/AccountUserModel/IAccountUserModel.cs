using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.AccountUserModel
{
    public interface IAccountUserModel
    {
        public T GetProperty<T>(string propertyName);

        public void SetProperty(string propertyName, object value);
    }

} 

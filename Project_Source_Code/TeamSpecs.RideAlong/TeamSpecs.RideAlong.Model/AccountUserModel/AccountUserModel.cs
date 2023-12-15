using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.AccountUserModel
{
    public class AccountUserModel : IAccountUserModel
    {
       
        public Dictionary<string,object> userProperties = new Dictionary<string, object>();
        public T GetProperty<T>(string propertyName)
        {
            return userProperties.ContainsKey(propertyName) ? (T)userProperties[propertyName] : default;
        }

        public void SetProperty(string propertyName, object value)
        {
            if (userProperties.ContainsKey(propertyName))
            {
                userProperties[propertyName] = value;
            }
            else
            {
                userProperties.Add(propertyName, value);
            }
        }
    }
}

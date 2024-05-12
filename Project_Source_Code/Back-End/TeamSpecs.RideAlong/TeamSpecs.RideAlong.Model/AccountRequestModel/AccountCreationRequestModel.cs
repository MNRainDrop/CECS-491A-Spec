using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.AccountRequestModel
{
    public class AccountCreationRequestModel : IAccountCreationRequestModel
    {
        public DateTime DateOfBirth { get; set; }
        public string AltEmail { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public string AccountType { get; set; }
    }
}

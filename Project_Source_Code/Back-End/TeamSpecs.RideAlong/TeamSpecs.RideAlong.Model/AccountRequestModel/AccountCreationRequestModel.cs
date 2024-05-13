using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.Model.AccountRequestModel
{
    public class AccountCreationRequestModel : IAccountCreationRequestModel
    {
        public AccountCreationRequestModel(string dateOfbirth, string altEmail, string email, string otp, string accountType)
        {
            DateOfBirthString = dateOfbirth;
            DateOfBirth = DateTime.Parse(DateOfBirthString);
            this.AltEmail = altEmail;
            this.Email = email;
            this.Otp = otp;
            this.AccountType = accountType;
        }

        public AccountCreationRequestModel() 
        {
            DateOfBirth = DateTime.MinValue;
            DateOfBirthString = string.Empty;
            AltEmail = string.Empty;
            Email = string.Empty;
            Otp = string.Empty;
            AccountType = string.Empty;
        }

        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthString { get; set; }
        public string AltEmail { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public string AccountType { get; set; }
    }
}

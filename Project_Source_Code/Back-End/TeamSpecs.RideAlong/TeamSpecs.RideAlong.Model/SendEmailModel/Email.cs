
using TeamSpecs.RideAlong.Model.SendEmailModel;
public class EmailModel : IEmail
{
    public string useremail { get; set; }



    public EmailModel(string mail)
    {
        useremail = mail;
    }
}


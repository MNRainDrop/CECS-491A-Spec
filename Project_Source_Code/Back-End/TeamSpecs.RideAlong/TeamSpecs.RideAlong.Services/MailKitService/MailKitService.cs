using MimeKit;
using MailKit.Net.Smtp;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.ConfigService.ConfigModels;

namespace TeamSpecs.RideAlong.Services
{
    public class MailKitService : IMailKitService
    {
        private readonly string _username;
        private readonly string _password;

        private readonly IConfigServiceJson _config;
        public MailKitService(IConfigServiceJson config)
        {
            _config = config;

            _username = _config.GetConfig().EMAIL_SERVICE_LOGIN.EMAIL;
            _password = _config.GetConfig().EMAIL_SERVICE_LOGIN.PASSWORD;
        }
        public IResponse SendEmail(string emailaddrs, string title, string body)
        {

            //Reponse object
            IResponse response = new Response();
            try
            {
                //From Address    
                string FromAddress = "huynjoon2002@gmail.com";
                string FromAdressTitle = "My Name";
                //To Address    
                //string ToAddress = emailaddrs;
                string ToAddress = emailaddrs;
                string ToAdressTitle = "Microsoft ASP.NET Core";
                string Subject = title;
                string BodyContent = body;

                //Smtp Server    
                string SmtpServer = "smtp-mail.outlook.com";
                //Smtp Port Number    
                int SmtpPortNumber = 587;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress
                                        (FromAdressTitle,
                                         FromAddress
                                         ));
                mimeMessage.To.Add(new MailboxAddress
                                         (ToAdressTitle,
                                         ToAddress
                                         ));
                mimeMessage.Subject = Subject; //Subject  
                mimeMessage.Body = new TextPart("plain")
                {
                    Text = BodyContent
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate(
                        "huynjoon2002@gmail.com",
                        "longvi230502"
                        );
                    client.Send(mimeMessage);
                    //Console.WriteLine("The mail has been sent successfully !!");
                    //Console.ReadLine();
                    client.Disconnect(true);
                }
                response.HasError = false;
                return response;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                return response;
            }
        }


    }
}

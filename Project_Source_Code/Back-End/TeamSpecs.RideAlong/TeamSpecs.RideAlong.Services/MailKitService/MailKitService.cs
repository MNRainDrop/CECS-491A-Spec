using MimeKit;
using MailKit.Net.Smtp;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.ConfigService.ConfigModels;
using MimeKit;

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
                string FromAddress = _username;
                string FromAdressTitle = "RideAlong Notification";
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
                        _username,
                        _password
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

        public IResponse SendEmailwithAttachments(string emailaddrs, string title, string body, string path)
        {
            //Reponse object
            IResponse response = new Response();
            try
            {
                //From Address    
                string FromAddress = _username;
                string FromAdressTitle = "RideAlong Notification";
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

                // Create a MimePart for the attachment
                MimePart attachment = new MimePart("text", "plain")
                {
                    Content = new MimeContent(File.OpenRead(path)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(path)
                };
                // Create a multipart/mixed container to hold the message and attachment
                Multipart multipart = new Multipart("mixed");
                multipart.Add(mimeMessage.Body);
                multipart.Add(attachment);

                // Set the message's body to the multipart container
                mimeMessage.Body = multipart;





                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate(
                        _username,
                        _password
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

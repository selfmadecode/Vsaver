using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Services.Repository
{
    public class Notification : INotification
    {
        public Task SendSmsNotification(string body, string destination)
        {
            
            string accountSID = WebConfigurationManager.AppSettings["TwilloAccountSID"];
            string authToken = WebConfigurationManager.AppSettings["TwilloAccountAuthToken"];

            TwilioClient.Init(accountSID, authToken);

            var sms = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber("+18653209925"),
                to: new Twilio.Types.PhoneNumber(destination));

            
            return Task.FromResult(0);
            

            // Uncomment to use
        }

        public async Task SendEmailNotification(string body, string destination, string subject)
        {
            var apiKey = WebConfigurationManager.AppSettings["KeyMain"];
            var user = WebConfigurationManager.AppSettings["user"];

            var mailClient = new SendGridClient(apiKey);

            var email = new SendGridMessage()
            {
                From = new EmailAddress("kentekz61@gmail.com", user),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body
            };

            email.AddTo(new EmailAddress(destination));
            await mailClient.SendEmailAsync(email);

            //return Task.FromResult(0);
        }

    }
}
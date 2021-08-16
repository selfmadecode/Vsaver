using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;

namespace VSaver.Web.Services.Repository
{
    public class Jobs :IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private const decimal interest = 0.0001369863M;


        public Jobs()
        {
            _dbContext = new ApplicationDbContext();
        }
        public void AddInterest()
        {
            var accounts = _dbContext.Accounts.ToList();

            if (accounts == null)
                return;


            foreach (var account in accounts)
            {
                account.Interest += Convert.ToDecimal(interest, CultureInfo.InvariantCulture);
                _dbContext.Entry(account).State = EntityState.Modified;
                //_dbContext.Set<Account>().AddOrUpdate(account);
                //_dbContext.Accounts.AddOrUpdate(account);
                _dbContext.SaveChanges();
            }

            // accounts.ForEach(a => a.Balance += a.Balance.AddInterest());
            //accounts.ForEach(a => a.Interest += a.Interest.AddInterest());

        }

        private List<Customer> AllCustomers()
            => _dbContext.Customers.Include(c => c.Account).ToList();
        public void GenerateStatementOfAccount(string filePath)
        {
            var customers = AllCustomers();

            if (customers == null)
                return;
                //return Task.FromResult(0);

            foreach (var customer in customers)
            {
                string subject = $"Account statement for the month of: {DateTime.Now.AddMonths(-1):MMMM}";
                string body = $"Your total balance is: {customer.Account.Balance}, \n" +
                    $"interest is: {customer.Account.Interest} ";
                GenerateStatementOfAccountAsPdfReport(subject, body, filePath);
                SendAccountStatementViaEmail(subject, body, customer.Email, filePath);
            }
            //return Task.FromResult(0);
        }
        private void SendAccountStatementViaEmail(string subject, string body, string destination, string filePath)
        {

            var apiKey = WebConfigurationManager.AppSettings["apiKeyMain"];
            var user = WebConfigurationManager.AppSettings["user"];

            var mailClient = new SendGridClient(apiKey);

            var email = new SendGridMessage()
            {
                From = new EmailAddress("anyanwuraphaelc@gmail.com", user),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body
            };

            //var bytes = File.ReadAllBytes("C:\\Users\\SelfMade\\source\\repos\\VSaver.Web\\Content\\statement.PDF");
            var bytes = File.ReadAllBytes(Path.Combine(filePath, "statement.PDF"));
            var file = Convert.ToBase64String(bytes);
            email.AddAttachment("statement.PDF", file);

            email.AddTo(new EmailAddress(destination));
            mailClient.SendEmailAsync(email);

            //return Task.FromResult(0);
        }
        private void GenerateStatementOfAccountAsPdfReport(string PdfTitle, string body, string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = PdfTitle;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            // Draw the text
            gfx.DrawString(body, font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height),
            XStringFormats.Center);

            // HttpContext.Current.Server.Mappath returns null
            // HostingEnvirement.MapPath
            // should work just fine, as long as the code is running InProc with the website application            

            string filename = Path.Combine(filePath, "statement.PDF");
            Debug.WriteLine(filename);
            //string filename = "C:\\Users\\SelfMade\\source\\repos\\VSaver.Web\\Content\\statement.PDF";

            try
            {
                if (!File.Exists(filename))
                    File.Create(filename);
                    //Directory.CreateDirectory(filename);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }

            document.Save(filename);
            Process.Start(filename);
        }

        public void SendMontlyStatementOfAccountViaSms()
        {
            var customers = AllCustomers();
            foreach (var customer in customers)
            {
                SendMontlyAccountBalanceViaSMS($"Your account balance for the " +
                    $"month of {DateTime.Now.AddMonths(-1)} is {customer.Account.Balance}", customer.PhoneNumber);
            }
        }
        private Task SendMontlyAccountBalanceViaSMS(string body, string destination)
        {
            /*
            string accountSID = WebConfigurationManager.AppSettings["TwilloAccountSID"];
            string authToken = WebConfigurationManager.AppSettings["TwilloAccountAuthToken"];

            TwilioClient.Init(accountSID, authToken);

            var sms = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber("+13477369198"),
                to: new Twilio.Types.PhoneNumber($"+234{destination}"));

            return Task.FromResult(0);
            */
            // Uncomment to use
            return Task.FromResult(0);
        }
        public void Dispose()
        {
            //_dbContext.Dispose();
        }
    }

    public static class JobHelper
    {
        private const decimal interest = 0.0001369863M;
        public static decimal AddInterest(this decimal account)
        {
            return account + interest;
        }
    }
}
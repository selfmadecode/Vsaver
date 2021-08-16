using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSaver.Web.Services.Interfaces
{
    public interface INotification
    {
        Task SendSmsNotification(string body, string destination);
        Task SendEmailNotification(string body, string destination, string subject);

    }
}

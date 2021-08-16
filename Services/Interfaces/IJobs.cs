using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSaver.Web.Models.Entities;

namespace VSaver.Web.Services.Interfaces
{
    public interface IJobs
    {
        Task AddInterest();
        Task SendAccountStatement();
    }
}

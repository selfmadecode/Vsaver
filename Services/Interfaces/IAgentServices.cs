using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;

namespace VSaver.Web.Services.Interfaces
{
    public interface IAgentServices
    {
        Task CreateAgent(AgentRegisterViewModel agent, string userid);

        bool VerifyAgentId(string agentId);

        Task<IEnumerable<Customer>> GetCustomersByAgent(string agentId);

        IEnumerable<string> GetRolesInDb();

        Task<IEnumerable<Transaction>> GetTransactionsByAgent(string agentId);
    }
}

using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Services.Repository
{
    public class AgentServices : IAgentServices
    {
        private readonly ApplicationDbContext _dbContext;
        public AgentServices()
        {
            _dbContext = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
        }
        private static Random random = new Random();        

        public async Task CreateAgent(AgentRegisterViewModel agent, string userId)
        {
            // use auto mapper
            var newagent = new Agent
            {
                UserId = userId,
                FirstName = agent.FirstName,
                LastName  = agent.LastName,
                PhoneNumber = agent.PhoneNumber,
                AccountVerified = true,
            };

            _dbContext.Agents.Add(newagent);
            await _dbContext.SaveChangesAsync();
        }

        public bool VerifyAgentId(string agentId)
        {
            var idExist = _dbContext.Agents.FirstOrDefault(a => a.UserId == agentId);

             if ((idExist != null) && (idExist.AccountVerified == true)) return true;
            
             return false;
        }

        public async Task<IEnumerable<Customer>> GetCustomersByAgent(string agentUserId)
        {
            var agent = _dbContext.Agents.FirstOrDefault(a => a.UserId == agentUserId);

            if (agent == null)
                return null;

            return await _dbContext.Customers
                    .Where(c => c.AgentId == agent.Id)
                    .Include(c => c.Account)
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAgent(string agentUserId)
        {
            var agentPerformTran = _dbContext.Agents.FirstOrDefault(a => a.UserId == agentUserId);
            

            if (agentPerformTran == null)
                return null;

            //var agent = _dbContext.Transactions.FirstOrDefault(a => a.AgentId == agentPerformTran.Id);


            return await _dbContext.Transactions
                    .Where(a => a.AgentId == agentPerformTran.Id)
                    .Include(c => c.Customer)
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();
        }

        public IEnumerable<string> GetRolesInDb()
         => _dbContext.Roles.Select(r => r.Name).ToList();
    }
}
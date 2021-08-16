using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;

namespace VSaver.Web.Services.Interfaces
{
    public interface IAdminServices
    {
        Task<IEnumerable<Customer>> GetUnVerifiedCustomerAccount();
        Task<IEnumerable<Customer>> GetVerifiedCustomerAccounts();
        Task<IEnumerable<Agent>> GetUnVerifiedAgentAccount();

        Task<bool> ProcessAccount(int customerId, bool condition);

        CustomerTransactionHistory GetCustomerAndTransactionInfo(int id);

        Task<IEnumerable<Customer>> AllAccounts();

        Task<bool> VerifyAgent(int agentId, bool condition);
        Task<RevenueAgentCustomerInfoViewModel> RevenueAndCustomersInfo();

        Task<IEnumerable<Transaction>> GetPendingWithdrawal();
        Task<IEnumerable<Transaction>> GetPendingDeposit();

        Task<bool> ApproveWithdrawal(int Id, double accountNumber, decimal amount);
        Task<bool> DisapproveWithdrawal(int Id, double accountNumber, decimal amount);

        /*
        Task<bool> ApproveDeposit(int Id, double accountNumber, decimal amount);
        Task<bool> DisapproveDeposit(int Id, double accountNumber, decimal amount);
        */

    }
}

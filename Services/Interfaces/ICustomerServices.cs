using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Enums;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;

namespace VSaver.Web.Services.Interfaces
{
    public interface ICustomerServices
    {
        Task<bool> CreateCustomer(Customer customer, decimal IntialDeposit, int pin, string customerId, string agentUserId);
        // Customer GetCustomerDetails(string userId);

        TransactionStatus WithdrawFromUserAccount(WithdrawalViewModel transaction, string agentId);
        TransactionStatus DepositIntoUserAccount(DepositViewModel transaction, string agentId);

        Customer GetCustomerAndTransactionDetails(string customerId);

        string GetAccountName(double accountNumber);
    }
}

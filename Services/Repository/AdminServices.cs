using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Enums;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Services.Repository
{
    public class AdminServices : IAdminServices
    {
        private readonly ApplicationDbContext _context;
        private readonly INotification _notification;
        private static Random random = new Random();


        public AdminServices(ApplicationDbContext context, INotification notification)
        {
            _context = context;
            _notification = notification;
        }

        public async Task<IEnumerable<Customer>> AllAccounts()
           => await _context.Customers.Include(c => c.Account).ToListAsync();

        public async Task<IEnumerable<Customer>> GetUnVerifiedCustomerAccount()
            => await GetAccounts(null);
        

        public async Task<IEnumerable<Agent>> GetUnVerifiedAgentAccount()
            => await _context.Agents
                .Where(a => a.AccountVerified == false).ToListAsync() ?? null;

        public async Task<bool> ProcessAccount(int customerId, bool condition)
        {
            var customerAccount = await FindCustomer(customerId);

            if (customerAccount != null)
            {
                if (condition)
                {
                    // if condition is true
                    ProcessCustomerAccount(customerAccount, true);
                    //customerAccount.Account.AccountNumber = GenerateAccountNumber();
                    //customerAccount.AccountStatus = AccountStatus.Open;
                    _context.SaveChanges();
                    // start accumulating interest
                    return true;
                }
                ProcessCustomerAccount(customerAccount, false);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        private async Task<Customer> FindCustomer(int id)
            => await _context.Customers.FindAsync(id);

        private async Task<Agent> FindAgent(int id)
            => await _context.Agents.FindAsync(id);

        private void ProcessCustomerAccount(Customer customer, bool condition)
        {
            var accountStatus = customer.AccountVerified;

            if (accountStatus != condition)
            {
                customer.AccountVerified = condition;
            }
            _context.SaveChanges();
        }

        public CustomerTransactionHistory GetCustomerAndTransactionInfo(int id)
        {
            var customer = _context.Customers.Include(c => c.Agent).FirstOrDefault(c => c.Id == id);

            if (customer == null)
                return null;

           var transaction = _context.Transactions.Include(c => c.Agent).Where(t => t.CustomerId == customer.Id).ToList();

            return new CustomerTransactionHistory {
                Customer = customer,
                Transactions = transaction
            };
        }

        public async Task<IEnumerable<Customer>> GetVerifiedCustomerAccounts()
        {
            var VerifiedAccounts = await GetAccounts(true);

            if (VerifiedAccounts == null) return null;

            return VerifiedAccounts;
        }

        private async Task<IEnumerable<Customer>> GetAccounts(bool? condition)
           => await _context.Customers.Where(a => a.AccountVerified == condition).Include(a => a.Account).ToListAsync();

       
        public async Task<RevenueAgentCustomerInfoViewModel> RevenueAndCustomersInfo()
        {
            var info = new RevenueAgentCustomerInfoViewModel
            {
                TotalAgents = await _context.Agents.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync()
            };

            return info;
        }

        public async Task<bool> VerifyAgent(int agentId, bool condition)
        {
            var agent = await FindAgent(agentId);

            if (agent != null)
            {
                if (condition)
                {
                    // if condition is true
                    agent.AccountVerified = condition;
                    _context.SaveChanges();
                    return true;
                }
                agent.AccountVerified = condition;
                _context.SaveChanges();

                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Transaction>> GetPendingWithdrawal()
            => await _context.Transactions
            .Include(a => a.Agent)
            .Where(t => (t.TransactionStatus == TransactionStatus.Processing)
                && (t.TransactionType == TransactionType.Withdrawal)).ToListAsync();

        public async Task<IEnumerable<Transaction>> GetPendingDeposit()
            => await _context.Transactions
            .Include(a => a.Agent)
            .Where(t => (t.TransactionStatus == TransactionStatus.Processing)
                && (t.TransactionType == TransactionType.Deposit)).ToListAsync();

        public async Task<bool> ApproveWithdrawal(int Id, double accountNumber, decimal Amount)
        {

            var customerAccount = GetCustomerAccountInfo(accountNumber);

            var pendingTransaction = GetCustomerPendingTransaction(Id);

            if (customerAccount == null || pendingTransaction == null)
                return false;
                        
            customerAccount.Account.Balance -= Amount;
            pendingTransaction.TransactionStatus = TransactionStatus.Approved;
            _context.SaveChanges();

            var customer = GetUserDetails(customerAccount.UserId);


            // Send Debite Alert

             await _notification.SendEmailNotification("Debit Alert",
               $"{Amount} has been Debited from your account", customer.Email);

            await _notification.SendSmsNotification(
            body: $"{Amount} has been debitted into your account",
              destination: $"+234{customer.PhoneNumber}");

            return true;
        }

        public async Task<bool> DisapproveWithdrawal(int Id, double accountNumber, decimal Amount)
        {

            var customerAccount = GetCustomerAccountInfo(accountNumber);

            var pendingTransaction = GetCustomerPendingTransaction(Id);

            if (customerAccount == null || pendingTransaction == null)
                return false;

            pendingTransaction.TransactionStatus = TransactionStatus.Declined;
            _context.SaveChanges();

            var customer = GetUserDetails(customerAccount.UserId);


            // Send Decline Approval Alert

             await _notification.SendEmailNotification("Transaction Declined",
               $"{Amount} was declined", customer.Email);

            await _notification.SendSmsNotification(
            body: $"{Amount} was declined",
              destination: $"+234{customer.PhoneNumber}");

            return true;
        }
        /*
        public async Task<bool> ApproveDeposit(int Id, double accountNumber, decimal Amount)
        {

            var customerAccount = GetCustomerAccountInfo(accountNumber);

            var pendingTransaction = GetCustomerPendingTransaction(Id);

            if (customerAccount == null || pendingTransaction == null)
                return false;

            customerAccount.Account.Balance += Amount;
            pendingTransaction.TransactionStatus = TransactionStatus.Approved;
            _context.SaveChanges();

            var customer = GetUserDetails(customerAccount.UserId);


            // Send Deposit Alert

            // await _notification.SendEmailNotification("Debit Alert",
            //   $"{transaction.Amount} has been Debited from your account", customer.Email);

            //await _notification.SendSmsNotification(
            //body: $"{transaction.Amount} has been debitted into your account",
            //  destination: $"+234{customer.PhoneNumber}");
            return true;
        }

        public async Task<bool> DisapproveDeposit(int Id, double accountNumber, decimal Amount)
        {

            var customerAccount = GetCustomerAccountInfo(accountNumber);

            var pendingTransaction = GetCustomerPendingTransaction(Id);

            if (customerAccount == null || pendingTransaction == null)
                return false;

            pendingTransaction.TransactionStatus = TransactionStatus.Declined;
            _context.SaveChanges();

            var customer = GetUserDetails(customerAccount.UserId);


            // Send Decline Deposit Alert

            // await _notification.SendEmailNotification("Transaction Declined",
            //   $"{transaction.Amount} was declined", customer.Email);

            //await _notification.SendSmsNotification(
            //body: $"{transaction.Amount} was declined",
            //  destination: $"+234{customer.PhoneNumber}");
            return true;
        }
        */
        private Customer GetCustomerAccountInfo(double accountNumber)
        {
            return _context.Customers.Include(c => c.Account)
                .FirstOrDefault(a => a.Account.AccountNumber == accountNumber);            
        }

        private Transaction GetCustomerPendingTransaction(int Id)
        {
            return _context.Transactions.FirstOrDefault(t => t.Id == Id);

        }

        private ApplicationUser GetUserDetails(string UserId) =>
            _context.Users.FirstOrDefault(u => u.Id == UserId);

    }
}
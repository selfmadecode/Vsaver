using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
    public class CustomerServices : ICustomerServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityMessageService _messageService;
        private readonly INotification _smsNotification;
        private static Random random = new Random();

        public CustomerServices(IIdentityMessageService messageService, INotification smsNotification )
        {
            _messageService = messageService;
            _smsNotification = smsNotification;
            _context = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
        }
        public async Task<bool> CreateCustomer(Customer customer, decimal initialDeposit, int pin, string customerUserId, string agentId)
        {
            try
            {
                var accountNumber = GenerateCustomerAccountNumber();
                //check if the account number is already in db

                var agentCreatingAccount = GetAgent(agentId);

                customer.Account = new Account
                {
                    AccountNumber = accountNumber,
                    Balance = initialDeposit,
                    PIN = pin
                };
                customer.Agent = agentCreatingAccount;
                //customer.Agent_Id = agentCreatingAccount.Id;
                customer.UserId = customerUserId;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                
                await SendMailNotification(subject: "Vsaver Welcomes you",
                    body: $"We are delighted to serve you, your account number is: {accountNumber}",
                    destination: customer.Email);

               await _smsNotification.SendSmsNotification(
                    body: $"We are delighted to serve you, your account number is: {accountNumber}",
                    destination: $"+234{customer.PhoneNumber}");
                
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

            return true;
        }

        private async Task SendMailNotification(string subject, string body, string destination)
        {
            var message = new IdentityMessage
            {
                Subject = subject,
                Body = body,
                Destination = destination,
            };
            await _messageService.SendAsync(message);
        }
        
        private double GenerateCustomerAccountNumber()
        {
            //Get last account number in DB
            var lastAccount = _context.Accounts
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            if (lastAccount == null)
            {
                const int accountLength = 8;
                const string chars = "0123456789";

                var accountNumber = "56" + new string(Enumerable.Repeat(chars, accountLength)
                  .Select(s => s[random.Next(s.Length)]).ToArray());

                return double.Parse(accountNumber);
            };

            return lastAccount.AccountNumber + 1;
        }
        
        public TransactionStatus WithdrawFromUserAccount(WithdrawalViewModel transaction, string agentId)
        {
            var customerAccount = GetCustomerAccount(transaction.AccountNumber);

            var accountBalance = customerAccount.Account.Balance;

                if (customerAccount != null)
                {
                    var customer = GetUserDetails(customerAccount.UserId);
                    var agentPerformingTransaction = GetAgent(agentId);

                    if(transaction.Amount >= 1)
                    {
                        if (!(transaction.Amount > accountBalance))
                        {
                            // if the PIN is the same
                            if (transaction.PIN == customerAccount.Account.PIN)
                            {
                                // dont debit the customer yet
                                //customerAccount.Account.Balance -= transaction.Amount;
                                customerAccount.Transactions = new List<Transaction>
                                {
                                    new Transaction
                                    {
                                        AccountNumber = customerAccount.Account.AccountNumber,
                                        Amount = transaction.Amount,
                                        DateOfTransaction = DateTime.Now,
                                        TransactionType = TransactionType.Withdrawal,
                                        TransactionStatus = TransactionStatus.Processing,
                                        CustomerId = customerAccount.Id,
                                        Agent = agentPerformingTransaction
                                    }
                                };

                                try
                                {
                                    // Send notification to admin
                                    _smsNotification.SendEmailNotification("Withdrawal Request",
                                      $"{transaction.Amount} has been has been requested for withdrawal" +
                                      $"from {agentPerformingTransaction.FirstName} {agentPerformingTransaction.LastName} for {customerAccount.FirstName} {customerAccount.LastName}", "hello@vsavings.com.ng");

                                    _smsNotification.SendSmsNotification(
                                     body: $"{transaction.Amount} has been has been requested for withdrawal" +
                                     $"from {agentPerformingTransaction.FirstName} {agentPerformingTransaction.LastName} for {customerAccount.FirstName} {customerAccount.LastName}",
                                     destination: $"+234{08130938556}");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                _context.SaveChanges();
                                return TransactionStatus.Successful;
                            }
                            else
                            {
                                return TransactionStatus.IncorrectPin;
                            }
                        }
                        else
                        {
                            ///Insufficient Funds in account
                            return TransactionStatus.InsufficientFunds;
                        }
                    }

                    return TransactionStatus.InvalidAmount;
                }

                return TransactionStatus.AccountNotFound;
        }

        public TransactionStatus DepositIntoUserAccount(DepositViewModel transaction, string agentId)
        {            
                var customerAccount = _context.Customers
                .FirstOrDefault(a => a.Account.AccountNumber == transaction.AccountNumber);

                var accountBalance = customerAccount.Account.Balance;

                if (customerAccount != null)
                {
                    if(transaction.Amount >= 1)
                    {
                        var agentPerformingTransaction = GetAgent(agentId);
                        customerAccount.Account.Balance += transaction.Amount;

                        var newAccountTransaction = new Transaction
                        {
                            AccountNumber = customerAccount.Account.AccountNumber,
                            Amount = transaction.Amount,
                            TransactionType = transaction.TransactionType,
                            TransactionStatus = TransactionStatus.Approved,
                            CustomerId = customerAccount.Id,
                            Agent = agentPerformingTransaction,
                            DateOfTransaction = DateTime.Now
                        };

                        customerAccount.Transactions.Add(newAccountTransaction);
                        // Send Credit Alert
                        try
                        {
                            //await SendMailNotification("Debit Alert",
                            //$"{transaction.Amount} has been Debited from your account", customerAccount.Email);

                            _smsNotification.SendEmailNotification(
                                "Debit Alert", $"{transaction.Amount} has been Credited into your account",
                                customerAccount.Email);

                            _smsNotification.SendSmsNotification(
                              body: $"{transaction.Amount} has been Debited from your account",
                              destination: $"+234{customerAccount.PhoneNumber}");

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        _context.SaveChanges();
                        return TransactionStatus.Successful;
                    }
                    return TransactionStatus.InvalidAmount;
                }
                return TransactionStatus.AccountNotFound;
           // return TransactionStatus.Failed;
        }

        public Customer GetCustomerAndTransactionDetails(string customerId)
        {
            var customer = _context.Customers.Include(c => c.Account)
                .Include(t => t.Transactions)
                .FirstOrDefault(user => user.UserId == customerId);

            if (customer == null)
                return null;

            return customer;
        }

        private ApplicationUser GetUserDetails(string UserId) =>
            _context.Users.FirstOrDefault(u => u.Id == UserId);

        private Agent GetAgent(string agentId)
            => _context.Agents.FirstOrDefault(a => a.UserId == agentId);

        public string GetAccountName(double accountNumber)
        {
            // var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            var customer = _context.Customers
                .FirstOrDefault(a => a.Account.AccountNumber == accountNumber);

            if (customer == null)
                return null;

            return $"{customer.FirstName} {customer.LastName}";
        }

        private Customer GetCustomerAccount(double AccountNumber)
            => _context.Customers
                .FirstOrDefault(a => a.Account.AccountNumber == AccountNumber);
    }
}
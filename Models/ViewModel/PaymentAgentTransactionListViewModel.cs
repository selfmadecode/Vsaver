using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;

namespace VSaver.Web.Models.ViewModel
{
    public class PaymentAgentTransactionListViewModel
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public double AccountNumber { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public string CustomerName { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }
}
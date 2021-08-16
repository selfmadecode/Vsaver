using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;

namespace VSaver.Web.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public double AccountNumber { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public string agentId { get; set; }
        
        public int CustomerId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }


    }
}
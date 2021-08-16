using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Enums;
using VSaver.Web.Models.Models;

namespace VSaver.Web.Models.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public double AccountNumber { get; set; }
        public DateTime DateOfTransaction { get; set; } //= DateTime.Today;
        
        public Customer Customer { get; set; }        
        public int CustomerId { get; set; }
        public int PIN { get; set; }
        public Agent Agent { get; set; }
        public int AgentId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Enums;

namespace VSaver.Web.Models.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddelName { get; set; }
        public decimal NIN { get; set; }
        public decimal BVN { get; set; }
        public string PhoneNumber { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool? AccountVerified { get; set; } = null;

        public AccountStatus AccountStatus { get; set; } = AccountStatus.Open;

        public Account Account { get; set; }
        public int AccountId{ get; set; }

        public Agent Agent { get; set; }
        public int AgentId { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Enums;

namespace VSaver.Web.Models.ViewModel
{
    public class CustomerAndAccountViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }

        public DateTime CreatedAt { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public bool? AccountVerified { get; set; }
    }
}
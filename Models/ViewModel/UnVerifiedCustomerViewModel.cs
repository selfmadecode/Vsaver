using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSaver.Web.Models.ViewModel
{
    public class UnVerifiedCustomerViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Amount { get; set; }
        public bool? AccountVerified { get; set; }
        public string UserId { get; set; }

    }
}
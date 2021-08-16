using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;

namespace VSaver.Web.Models.ViewModel
{
    public class CustomerTransactionHistory
    {

        public Customer Customer { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
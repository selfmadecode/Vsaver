using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Enums;

namespace VSaver.Web.Models.ViewModel
{
    public class CustomerAccountAndTransactionViewModel : CustomerAndAccountViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; } =
            new List<Transaction>();
    }
}
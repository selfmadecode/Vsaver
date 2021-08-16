using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSaver.Web.Models.ViewModel
{
    public class WithdrawalViewModel : TransactionViewModel
    {
        public int PIN { get; set; }
    }
}
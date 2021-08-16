using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSaver.Web.Models.Enums
{
    public enum TransactionStatus
    {
        IncorrectPin,
        InsufficientFunds,
        InvalidAmount,
        AccountNotFound,
        Successful,
        Failed,
        Approved,
        Processing,
        Declined
    }
}
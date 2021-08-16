using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VSaver.Web.Models.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public double AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public int PIN { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Today;

        //public bool AllowedNotification { get; set; }
    }
}
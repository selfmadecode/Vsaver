using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSaver.Web.Models.ViewModel
{
    public class AgentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
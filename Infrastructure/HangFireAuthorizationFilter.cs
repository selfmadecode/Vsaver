using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSaver.Web.Infrastructure
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());
            /*
            bool authorizeCurrentUserToAccessHangFireDashoard = false;

            if (owinContext.Authentication.User.Identity.IsAuthenticated)
            {
                if (owinContext.Authentication.User.IsInRole("Admin"))
                    return authorizeCurrentUserToAccessHangFireDashoard = false;
            }

            return authorizeCurrentUserToAccessHangFireDashoard;
            */

            return owinContext.Authentication.User.IsInRole("Admin");
        }
    }
}
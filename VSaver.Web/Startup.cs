using Hangfire;
using Microsoft.Owin;
using Ninject;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using VSaver.Web.Infrastructure;
using VSaver.Web.Services.Interfaces;
using VSaver.Web.Services.Repository;

[assembly: OwinStartupAttribute(typeof(VSaver.Web.Startup))]
namespace VSaver.Web
{
    public partial class Startup
    {
        //HangeFire setup
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            yield return new BackgroundJobServer();
        }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // for IOC bindings
            // var kernel = new StandardKernel();
            // GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard("/vsaverjob", new DashboardOptions
            {
                Authorization = new [] { new HangFireAuthorizationFilter() },
                DashboardTitle = "Vsaver Cron Jobs"
            });

            app.UseHangfireServer();
            var filePath = HttpContext.Current.Server.MapPath("~/Content/");
            var job = new Jobs();

            try
            {
                RecurringJob.AddOrUpdate("Add Interest", () => job.AddInterest(), Cron.Daily);
               //RecurringJob.AddOrUpdate("Montly account Balance via SMS", () => job.SendMontlyStatementOfAccountViaSms(), Cron.Monthly);
                //Got the file path outside hangefire and sent it where needed
                //Cant get the file path from hangfire, because hang fire doesnt implement httpContext
               // RecurringJob.AddOrUpdate("Montly Account Statement", () => job.GenerateStatementOfAccount(filePath), Cron.Monthly);
               
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

        }
    }
}

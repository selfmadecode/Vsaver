using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VSaver.Web.Services.Interfaces;
using VSaver.Web.Services.Repository;

namespace VSaver.Web.Infrastructure
{
    public class NinjectDependenceResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependenceResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<ICustomerServices>().To<CustomerServices>();
            kernel.Bind<IAdminServices>().To<AdminServices>();
            kernel.Bind<IAgentServices>().To<AgentServices>();
            //using usermanager in another service
            //kernel.Bind(typeof(IUserStore<>)).To(typeof(UserStore<>)).InRequestScope();
            kernel.Bind<IIdentityMessageService>().To<EmailService>();
            kernel.Bind<INotification>().To<Notification>();
            //kernel.Bind<IJobs>().To<Jobs>().InBackgroundJobScope();
        }
    }
}
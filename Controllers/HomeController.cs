using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerServices _customerServices;
        public HomeController(ICustomerServices customerServices)
        {
            _customerServices = customerServices ?? throw new ArgumentNullException(nameof(customerServices));
        }
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            return View();
        }
        [Authorize]
        public ActionResult CustomerDashboard()
        {
            var userId = User.Identity.GetUserId();

            var customer = _customerServices.GetCustomerAndTransactionDetails(userId);

            if (customer == null)
                return HttpNotFound("User Notfound");

            
            return View(Mapper.Map<CustomerAccountAndTransactionViewModel>(customer));
            //return View(Mapper.Map<CustomerAndAccountViewModel>(customer));
            /*

            if (User.IsInRole("AccountAgent"))
                return RedirectToAction("Index", "Agent");

            if (User.IsInRole("PaymentAgent"))
                return RedirectToAction("Index", "Agent");



            return RedirectToAction("CustomerDashboard");
            */
        }
    }
}
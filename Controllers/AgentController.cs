using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VSaver.Web.Models;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Enums;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        private ICustomerServices _customerServices;
        private readonly IAgentServices _agentServices;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public AgentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }



        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public AgentController(ICustomerServices customerServices, IAgentServices agentServices )
        {
            _customerServices = customerServices;
            _agentServices = agentServices;
        }
        // GET: Dashboard

        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            if (User.IsInRole("AccountAgent"))
                return RedirectToAction("GetAccountsCreatedByAgent");

            //BVN and NIN optional

            if (User.IsInRole("PaymentAgent"))
                return RedirectToAction("GetTransactionsByAgent");


            //redirects the customer to customerdashboard in home controller
            return RedirectToAction("CustomerDashboard", "Home");
        }

        [Authorize(Roles = "AccountAgent")]
        public ViewResult CreateCustomer()
        {
            return View("Details");
        }

        [Authorize(Roles = "AccountAgent")]
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CustomerViewModel customerViewModel)
        {
                if (!ModelState.IsValid)
                    return View("Details", customerViewModel);

                string agentUserId = User.Identity.GetUserId();


                var agentId = _agentServices.VerifyAgentId(agentUserId);

                if(agentId == false)
                {
                    ModelState.AddModelError("AgentId", "Review AgentID");
                    return View("Details", customerViewModel);
                }

                // create a login account for the user
                var newUser = new ApplicationUser
                {
                    UserName = customerViewModel.Email,
                    Email = customerViewModel.Email,
                    EmailConfirmed = true
                };

                var identityResult = await UserManager.CreateAsync(newUser, customerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    var customer = Mapper.Map<CustomerViewModel, Customer>(customerViewModel);

                    var result = await _customerServices.CreateCustomer(
                        customer,
                        customerViewModel.Amount,
                        customerViewModel.PIN,
                        newUser.Id, agentUserId);


                    if (result)
                    {
                        TempData["CustomerAccountCreated"] = "success";
                        return RedirectToAction("GetAccountsCreatedByAgent");
                    }
                
                }

                AddErrors(identityResult);            

                return View("Details", customerViewModel);

        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [Authorize(Roles = "AccountAgent")]
        public async Task<ViewResult> GetAccountsCreatedByAgent()
        {
            string loggedInUser = User.Identity.GetUserId();

            var customers = Mapper.Map<IEnumerable<CustomerAndAccountViewModel>>(
                await _agentServices.GetCustomersByAgent(loggedInUser));

            return View("AgentDashboard", customers);
        }

        [Authorize(Roles = "PaymentAgent")]
        public async Task<ViewResult> GetTransactionsByAgent()
        {
            string loggedInUser = User.Identity.GetUserId();

            var transactions = Mapper.Map<IEnumerable<PaymentAgentTransactionListViewModel>>(
                await _agentServices.GetTransactionsByAgent(loggedInUser));

            return View("PaymentAgentIndex", transactions);
        }

        [HttpGet]
        [Authorize(Roles = "PaymentAgent")]
        public ViewResult Deposit()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "PaymentAgent")]
        public ViewResult Withdrawal()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "PaymentAgent")]
        public ViewResult Deposit(DepositViewModel transaction)
        {
            if (!ModelState.IsValid)
                return View(transaction);

            var agentId = User.Identity.GetUserId();

            var result = _customerServices.DepositIntoUserAccount(transaction, agentId);

            switch (result)
            {

                case TransactionStatus.InvalidAmount:
                    ModelState.AddModelError("Amount", "Invalid Amount");
                    return View(transaction);                    

                case TransactionStatus.AccountNotFound:
                    ModelState.AddModelError("AccountNumber", "Account Not found");
                    return View(transaction);

                case TransactionStatus.Successful:
                    TempData["TransactionStatus"] = "success";
                    ViewBag.TransactionStatus = "Transaction Successful";
                    break;
                case TransactionStatus.Failed:
                    ViewBag.TransactionStatus = "Transaction Failed";
                    break;
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "PaymentAgent")]
        public ViewResult Withdraw(WithdrawalViewModel transaction)
        {
            if (!ModelState.IsValid)
                return View(transaction);

            var agentId = User.Identity.GetUserId();

            var result = _customerServices.WithdrawFromUserAccount(transaction, agentId);

            switch (result)
            {
                case TransactionStatus.IncorrectPin:
                    ModelState.AddModelError("PIN", "Incorrect Pin");
                    return View("Withdrawal", transaction);

                case TransactionStatus.InsufficientFunds:
                    ModelState.AddModelError("Amount", "Insufficient Funds");
                    return View("Withdrawal", transaction);

                case TransactionStatus.InvalidAmount:
                    ModelState.AddModelError("Amount", "Invalid Amount");
                    return View("Withdrawal", transaction);

                case TransactionStatus.AccountNotFound:
                    ModelState.AddModelError("AccountNumber", "Account Not found");
                    return View("Withdrawal", transaction);

                case TransactionStatus.Successful:
                    TempData["TransactionStatus"] = "success";
                    ViewBag.TransactionStatus = "Transaction Processing";
                    break;
                case TransactionStatus.Failed:
                    ViewBag.TransactionStatus = "Transaction Failed";
                    break;
            }
            return View("Withdrawal");
        }

        [Authorize(Roles = "PaymentAgent")]
        public ActionResult GetCustomerAccountName(double accountNumber)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CustomerName = "Not foundggg";
            }
            var accountName = _customerServices.GetAccountName(accountNumber);

            if(accountName == null)
                ViewBag.CustomerName = "Not found";
            else
            {
                ViewBag.CustomerName = accountName;
                ViewBag.CustomerAccountNumber = accountNumber;
            }               

            return View("Withdrawal");
        }

    }
}
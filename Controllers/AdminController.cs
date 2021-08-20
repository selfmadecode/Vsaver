using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;
using VSaver.Web.Services.Interfaces;

namespace VSaver.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminServices _adminServices;

        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            return View(await _adminServices.RevenueAndCustomersInfo());
        }

        public async Task<ActionResult> AllAccounts()
        {
            var accounts = await _adminServices.AllAccounts();
            return View(Mapper.Map<IEnumerable<CustomerAndAccountViewModel>>(accounts));
        }

        public async Task<ActionResult> VerifiedAccounts()
        {
            var verifiedCustomersAccounts = await _adminServices.GetVerifiedCustomerAccounts();
            var result = Mapper.Map<IEnumerable<CustomerAndAccountViewModel>>(verifiedCustomersAccounts);

            return View(result);
        }
        public async Task<ActionResult> GetUnVerifiedAccounts()
        {
            var unVerifiedAccounts = await _adminServices.GetUnVerifiedCustomerAccount();

            var result = Mapper.Map<IEnumerable<CustomerAndAccountViewModel>>(unVerifiedAccounts);
            return View(result);
        }

        public ActionResult CustomerAndTransactionDetails(int id)
        {
            var customer = _adminServices.GetCustomerAndTransactionInfo(id);
            
            if (customer == null)
                return HttpNotFound("Customer not found");

            //var map = Mapper.Map<UnVerifiedCustomerViewModel>(customer);
            return View(customer);
        }

        public async Task<ActionResult> ProcessAccount(int id, bool condition)
        {

            var result = await _adminServices.ProcessAccount(id, condition);

            if(result)
                return RedirectToAction("GetUnVerifiedAccount");

            return HttpNotFound("Account Not found");
        }
        
        public async Task<ActionResult> UnVerifiedAgents()
        {
            return View(await _adminServices.GetUnVerifiedAgentAccount());
        }
        public async Task<ActionResult> VerifyAgent(int id, bool condition)
        {

            var result = await _adminServices.VerifyAgent(id, condition);

            if (result)
                return RedirectToAction("UnVerifiedAgents");

            return HttpNotFound("Account Not found");
        }

        public async Task<ActionResult> PendingWithdrawals()
        {
            var result = Mapper.Map<IEnumerable<PendingTransactionsViewModel>>(await _adminServices.GetPendingWithdrawal());

            return View(result);
        }
        public async Task<ActionResult> PendingDeposits()
        {
            var result = Mapper.Map<IEnumerable<PendingTransactionsViewModel>>(await _adminServices.GetPendingDeposit());

            return View(result);
        }
        public async Task<ActionResult> ApproveWithdrawal(int id, double accountNumber, decimal amount)
        {
           var result =  await _adminServices.ApproveWithdrawal(id, accountNumber, amount);

            if (!result)
                return HttpNotFound("Account or Transaction not found");

            return RedirectToAction("PendingWithdrawals");
        }
        public async Task<ActionResult> DisapproveWithdrawal(int id, double accountNumber, decimal amount)
        {
            var result = await _adminServices.DisapproveWithdrawal(id, accountNumber, amount);

            if (!result)
                return HttpNotFound("Account or Transaction not found");

            return RedirectToAction("PendingWithdrawals");
        }
        
        /*
        public async Task<ActionResult> ApproveDeposit(int id, double accountNumber, decimal amount)
        {
            var result = await _adminServices.ApproveDeposit(id, accountNumber, amount);

            if (!result)
                return HttpNotFound("Account or Transaction not found");

            return RedirectToAction("PendingWithdrawals");
        }
        public async Task<ActionResult> DisapproveDeposit(int id, double accountNumber, decimal amount)
        {
            var result = await _adminServices.DisapproveDeposit(id, accountNumber, amount);
            
            if (!result)
                return HttpNotFound("Account or Transaction not found");

            return RedirectToAction("PendingWithdrawals");
        }
        */
    }
}
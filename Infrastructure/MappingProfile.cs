using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;
using VSaver.Web.Models.ViewModel;

namespace VSaver.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<UnVerifiedCustomerViewModel, Customer>()
                .ForMember(c => c.NIN, c => c.Ignore())
                .ForMember(c => c.BVN, c => c.Ignore())
                .ForMember(c => c.AgentId, c => c.Ignore())
                .ForMember(c => c.MiddelName, c => c.Ignore());


            Mapper.CreateMap<Customer, UnVerifiedCustomerViewModel>();

            Mapper.CreateMap<Customer, CustomerViewModel>().ReverseMap();


            // used to get account from db
            Mapper.CreateMap<Customer, CustomerAndAccountViewModel>()
                .ForMember(c => c.AccountNumber, opt => opt.MapFrom(c => c.Account.AccountNumber))
                .ForMember(c => c.Balance, opt => opt.MapFrom(c => c.Account.Balance))
                //.ForMember(c => c.Interest, opt => opt.MapFrom(c => c.Account.Interest))
                .ForMember(c => c.CreatedAt, opt => opt.MapFrom(c => c.Account.CreatedAt));

            Mapper.CreateMap<Transaction, TransactionViewModel>();
            Mapper.CreateMap<Transaction, PendingTransactionsViewModel>()
                .ForMember(a => a.AgentName,
                opt => opt.MapFrom(a => $"{a.Agent.FirstName} {a.Agent.LastName}"));

            Mapper.CreateMap<Transaction, PaymentAgentTransactionListViewModel>()
                .ForMember(c => c.CustomerName,
                opt => opt.MapFrom(a => $"{ a.Customer.FirstName} {a.Customer.LastName}"));

            Mapper.CreateMap<Transaction, PendindWithdrawalViewModel>();


            Mapper.CreateMap<Customer, CustomerAccountAndTransactionViewModel>()
                .ForMember(c => c.AccountNumber, opt => opt.MapFrom(c => c.Account.AccountNumber))
                .ForMember(c => c.Balance, opt => opt.MapFrom(c => c.Account.Balance))
                //.ForMember(c => c.Interest, opt => opt.MapFrom(c => c.Account.Interest))
                //.ForMember(c => c.CreatedAt, opt => opt.MapFrom(c => c.Account.CreatedAt))
                .ForMember(ct => ct.Transactions, opt => opt.MapFrom(ct => ct.Transactions));
          

        }
    }
}
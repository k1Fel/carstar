using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.DTO;
using System.Linq;
using api.Repository;
using api.Mappers.AccountMappers;
namespace api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account?> GetAccountById(int id)
        {
            return await _accountRepository.GetAccountById(id);
        }

        public async Task<bool> isEmailExists(string email)
        {
            return await _accountRepository.isEmailExists(email);
        }

        public async Task<Account?> Login(LoginDto model)
        {
            return await _accountRepository.Login(model);
        }

        public async Task<Account> Register(RegisterDto model)
        {
            var account = model.RegisterToModel();
            return await _accountRepository.Register(account);
        }

    }
}
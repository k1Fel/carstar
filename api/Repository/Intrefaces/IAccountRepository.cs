using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;

namespace api.Repository
{
    public interface IAccountRepository
    {
        Task<Account> Register(Account model);
        Task<Account?> Login(LoginDto model);
        Task<Account?> GetAccountById(int id);
        Task<bool> isEmailExists(string email);
        Task<bool> isUserExists(string userName, string email);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTO;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Account?> GetAccountById(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account;
        }

        public async Task<bool> isEmailExists(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            return account != null;
        }

        public async Task<Account?> Login(LoginDto model)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == model.Email);

            if (account == null)
            {
                throw new Exception("Invalid email or password");
            }

            // Перевіряємо пароль
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            return account;
        }

        public async Task<Account> Register(Account model)
        {
            _context.Accounts.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }
        public async Task<bool> isUserExists(string userName, string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName == userName || a.Email == email);
            return account != null;
        }
    }
}
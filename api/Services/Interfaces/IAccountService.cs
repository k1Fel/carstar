using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.DTO.Account;
using api.Models;

namespace api.Services
{
    
    public interface IAccountService
    {
        Task<Account> Register(RegisterDto model);
        Task<Account?> Login(LoginDto model);
        Task<Account?> GetAccountById(int id);
        Task<bool> isEmailExists(string email);
        Task<bool> isUserExists(string userName, string email);
        Task<string> CreateRefreshTokenAsync(int accountId);
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }   
}
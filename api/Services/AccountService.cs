using System.Security.Cryptography;
using api.Data;
using api.Models;
using api.DTO;
using api.Repository;
using api.Mappers.AccountMappers;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AccountService(
            IAccountRepository accountRepository,
            ApplicationDbContext context,
            IJwtService jwtService)
        {
            _accountRepository = accountRepository;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<Account?> GetAccountById(int id)
            => await _accountRepository.GetAccountById(id);

        public async Task<bool> isEmailExists(string email)
            => await _accountRepository.isEmailExists(email);

        public async Task<Account?> Login(LoginDto model)
            => await _accountRepository.Login(model);

        public async Task<Account> Register(RegisterDto model)
        {
            var account = model.RegisterToModel();
            return await _accountRepository.Register(account);
        }

        public async Task<bool> isUserExists(string userName, string email)
            => await _accountRepository.isUserExists(userName, email);

        // =====================
        // Refresh Token
        // =====================

        public async Task<string> CreateRefreshTokenAsync(int accountId)
        {
            // Відкликати всі старі токени цього акаунта
            var oldTokens = await _context.RefreshTokens
                .Where(r => r.AccountId == accountId && !r.IsRevoked)
                .ToListAsync();

            foreach (var old in oldTokens)
                old.IsRevoked = true;

            var refreshToken = new RefreshToken
            {
                Token = GenerateSecureToken(),
                AccountId = accountId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(r => r.Account)
                .FirstOrDefaultAsync(r =>
                    r.Token == refreshToken &&
                    !r.IsRevoked &&
                    r.ExpiresAt > DateTime.UtcNow);

            if (token == null)
                throw new UnauthorizedAccessException("Невалідний або протухлий refresh token");

            // Відкликати поточний
            token.IsRevoked = true;

            // Створити новий refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateSecureToken(),
                AccountId = token.AccountId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            // Генерувати новий access token
            var accessToken = _jwtService.GenerateToken(token.Account!);

            return (accessToken, newRefreshToken.Token);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        private static string GenerateSecureToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
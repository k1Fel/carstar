using System.Security.Claims;
using api.Data;
using api.DTO;
using api.DTO.Account;
using api.Mappers.AccountMappers;
using api.Services;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;

        public AccountController(IAccountService accountService, IJwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model == null)
                return BadRequest("Дані не надано");

            try
            {
                if (model.Password.Length < 6)
                    return BadRequest(new { message = "Пароль повинен містити не менше 6 символів" });

                if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@"))
                    return BadRequest(new { message = "Невірний формат email" });

                if (string.IsNullOrEmpty(model.UserName))
                    return BadRequest(new { message = "Ім'я користувача не може бути порожнім" });

                if (await _accountService.isUserExists(model.UserName, model.Email))
                    return BadRequest(new { message = "Користувач з таким ім'ям або email вже існує" });

                var account = await _accountService.Register(model);
                var accessToken = _jwtService.GenerateToken(account);
                var refreshToken = await _accountService.CreateRefreshTokenAsync(account.Id);

                return Ok(new
                {
                    message = "Registration successful",
                    token = accessToken,
                    refreshToken = refreshToken,
                    account = account.ToAccountResponseDto()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model == null)
                return BadRequest("Дані не надано");

            try
            {
                var account = await _accountService.Login(model);

                if (account == null)
                    return NotFound(new { message = "Account not found" });

                var accessToken = _jwtService.GenerateToken(account);
                var refreshToken = await _accountService.CreateRefreshTokenAsync(account.Id);

                return Ok(new
                {
                    message = "Login successful",
                    token = accessToken,
                    refreshToken = refreshToken,
                    account = account.ToAccountResponseDto()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            try
            {
                var (accessToken, refreshToken) = await _accountService.RefreshAsync(dto.RefreshToken);

                return Ok(new
                {
                    token = accessToken,
                    refreshToken = refreshToken
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            await _accountService.RevokeRefreshTokenAsync(dto.RefreshToken);
            return Ok(new { message = "Вийшли успішно" });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var account = await _accountService.GetAccountById(int.Parse(userId));

                if (account == null)
                    return NotFound(new { message = "Account not found" });

                return Ok(account.ToAccountResponseDto());
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Accounts
                .Select(a => new AccountResponseDto
                {
                    Id = a.Id,
                    Email = a.Email,
                    UserName = a.UserName,
                    Role = a.Role
                })
                .ToListAsync();
            return Ok(users);
        }

        // Змінити роль — тільки адмін
        [Authorize(Roles = "admin")]
        [HttpPatch("users/{id:int}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUserId == id)
                return BadRequest(new { message = "Не можна змінити власну роль" });

            if (dto.Role != "user" && dto.Role != "admin")
                return BadRequest(new { message = "Невірна роль. Дозволено: user, admin" });

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound(new { message = "Користувач не знайдено" });

            account.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Роль змінено на {dto.Role}",
                account = account.ToAccountResponseDto()
            });
        }

        // Видалити юзера — тільки адмін
        [Authorize(Roles = "admin")]
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUserId == id)
                return BadRequest(new { message = "Не можна видалити власний акаунт" });

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound(new { message = "Користувача не знайдено" });

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача видалено" });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.DTO;
using api.Mappers.AccountMappers;
using api.Services;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
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
            {
                return BadRequest("Дані не надано");
            }
            try
            {
                var account = await _accountService.Register(model);
                
                var token = _jwtService.GenerateToken(account);
                return Ok(new
                {
                    message = "Registration successful",
                    token = token,
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
            {
                return BadRequest("Дані не надано");
            }
            try
            {
                var account = await _accountService.Login(model);
                if(account == null)
                {
                    return NotFound(new { message = "Account not found" });
                }
                var token = _jwtService.GenerateToken(account);
                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    account = account.ToAccountResponseDto()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Отримуємо ID з токену
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var account = await _accountService.GetAccountById(int.Parse(userId));
                
                if (account == null)
                {
                    return NotFound(new { message = "Account not found" });
                }

                return Ok(account.ToAccountResponseDto());
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        
    }
}
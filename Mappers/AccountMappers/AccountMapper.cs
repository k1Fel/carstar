using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Account;
namespace api.Mappers.AccountMappers
{
    public static class AccountMapper
    {
        
        public static Models.Account RegisterToModel(this DTO.RegisterDto dto)
        {
            return new Models.Account
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), 
                UserName = dto.UserName,
            };
        }

        // Цей маппер не потрібен для Login, але залишимо
        public static Models.Account LoginToModel(this DTO.LoginDto dto)
        {
            return new Models.Account
            {
                Email = dto.Email,
                PasswordHash = dto.Password 
            };
        }

        
        public static AccountResponseDto ToAccountResponseDto(this Models.Account model)
        {
            return new AccountResponseDto
            {
                Id = model.Id,
                Email = model.Email,
                UserName = model.UserName
            };
        }
    }
}
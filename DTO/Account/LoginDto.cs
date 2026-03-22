using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірный формат email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Пароль обов'язковий")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен бути від 6 до 100 символів")]
        public string Password { get; set; } = string.Empty;
    }
}
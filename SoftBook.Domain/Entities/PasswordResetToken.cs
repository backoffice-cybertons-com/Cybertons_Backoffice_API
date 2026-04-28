using System.Threading.Tasks;
using System.Data;
using System;
using System.Collections.Generic;

namespace SoftBook.Domain.Entities
{
    public class PasswordResetToken
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
    }
}

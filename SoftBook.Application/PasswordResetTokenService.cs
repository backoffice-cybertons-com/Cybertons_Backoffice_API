using System;
using System.Collections.Generic;
using System.Linq;
using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services
{
    public class PasswordResetTokenService
    {
        private readonly List<PasswordResetToken> _tokens = new List<PasswordResetToken>();
        private readonly object _lock = new object();

        /// <summary>
        /// Generate a new password reset token
        /// </summary>
        public string GenerateToken(int userId, string email)
        {
            lock (_lock)
            {
                var token = Guid.NewGuid().ToString();
                var resetToken = new PasswordResetToken
                {
                    Token = token,
                    UserId = userId,
                    Email = email,
                    ExpirationTime = DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
                    IsUsed = false
                };

                _tokens.Add(resetToken);

                // Clean up expired tokens
                CleanupExpiredTokens();

                return token;
            }
        }

        /// <summary>
        /// Validate a password reset token
        /// </summary>
        public PasswordResetToken ValidateToken(string token)
        {
            lock (_lock)
            {
                var resetToken = _tokens.FirstOrDefault(t => 
                    t.Token == token && 
                    !t.IsUsed && 
                    t.ExpirationTime > DateTime.UtcNow);

                return resetToken;
            }
        }

        /// <summary>
        /// Mark a token as used
        /// </summary>
        public void MarkTokenAsUsed(string token)
        {
            lock (_lock)
            {
                var resetToken = _tokens.FirstOrDefault(t => t.Token == token);
                if (resetToken != null)
                {
                    resetToken.IsUsed = true;
                }
            }
        }

        /// <summary>
        /// Remove expired tokens from memory
        /// </summary>
        private void CleanupExpiredTokens()
        {
            _tokens.RemoveAll(t => t.ExpirationTime < DateTime.UtcNow || t.IsUsed);
        }
    }
}

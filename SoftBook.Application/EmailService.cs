using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SoftBook.Application.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        public async Task<bool> SendPasswordResetEmail(string toEmail, string resetLink)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUsername;
                var fromName = _configuration["EmailSettings:FromName"] ?? "SoftBook";

                // If SMTP is not configured, log the link instead of sending email
                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    Console.WriteLine($"[EMAIL NOT CONFIGURED] Password reset link for {toEmail}: {resetLink}");
                    return true; // Return true for testing purposes
                }

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = "Password Reset Request",
                        Body = GetPasswordResetEmailBody(resetLink),
                        IsBodyHtml = true
                    };

                    message.To.Add(toEmail);

                    await client.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string GetPasswordResetEmailBody(string resetLink)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #7367f0; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f4f4f4; padding: 30px; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #7367f0; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>We received a request to reset your password. Click the button below to reset your password:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Reset Password</a>
                            </p>
                            <p>Or copy and paste this link into your browser:</p>
                            <p style='word-break: break-all;'>{resetLink}</p>
                            <p><strong>This link will expire in 1 hour.</strong></p>
                            <p>If you didn't request a password reset, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; {DateTime.Now.Year} SoftBook. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace SoftBook.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Company ID is required")]
        [Display(Name = "Company ID")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Email or Username is required")]
        [Display(Name = "Email or Username")]
        public string EmailUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        // Optional error message to surface non-field specific errors
        public string? ErrorMessage { get; set; }
    }
}

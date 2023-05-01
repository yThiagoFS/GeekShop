﻿using System.ComponentModel.DataAnnotations;

namespace GeekShopping.IdentityServer.Pages.Account.Register
{
    public class InputRegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }

        public string RoleName { get; set; }

    }
}

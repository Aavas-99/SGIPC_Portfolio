using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGIPC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SignUpViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Confirm { get; set; }
    }

    public class SignInViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
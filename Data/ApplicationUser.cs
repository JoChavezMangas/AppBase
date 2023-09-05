using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LoginDate { get; set; }
        public string NormalizedUserName { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string NormalizedEmail { get; set; }

    }




    public class UserSignUpViewModel
    {
        public UserData User { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
    public class UserData
    {
        [Required(ErrorMessage = "*Campo requerido")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "*Campo requerido")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "*Campo requerido")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "*Campo requerido")]
        public string UserType { get; set; }
    }
    public class ResponseWebApiMulti
    {
        public DateTime Datetime { get; set; }
        public string Message { get; set; }
        public string userId { get; set; }
        public string email { get; set; }
        public string username { get; set; }
    }






}

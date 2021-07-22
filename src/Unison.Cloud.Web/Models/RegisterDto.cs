using System;
using System.ComponentModel.DataAnnotations;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

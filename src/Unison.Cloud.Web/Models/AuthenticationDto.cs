using System;
using System.ComponentModel.DataAnnotations;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class AuthenticationDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

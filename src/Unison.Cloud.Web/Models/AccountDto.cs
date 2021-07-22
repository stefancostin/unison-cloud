﻿using System;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class AccountDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}

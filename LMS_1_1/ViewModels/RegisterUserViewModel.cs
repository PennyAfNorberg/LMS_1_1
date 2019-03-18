﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.ViewModels
{
    public class RegisterUserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Confirmpassword { get; set; }
        public string Role { get; set; }

    }

    public class ManageUserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Confirmpassword { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
        public string Oldpassword { get; set; }
    }
}

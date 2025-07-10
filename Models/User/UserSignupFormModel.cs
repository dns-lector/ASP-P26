﻿using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Models.User
{
    public class UserSignupFormModel
    {
        [FromForm(Name = "user-name")]
        public String UserName { get; set; } = null!;

        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null!;

        [FromForm(Name = "birthdate")]
        public DateTime? Birthdate { get; set; }

        [FromForm(Name = "user-login")]
        public String UserLogin { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; } = null!;

        [FromForm(Name = "user-repeat")]
        public String UserRepeat { get; set; } = null!;

        [FromForm(Name = "agree")]
        public bool Agree { get; set; } = false;

    }
}

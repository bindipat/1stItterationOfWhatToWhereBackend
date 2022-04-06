using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
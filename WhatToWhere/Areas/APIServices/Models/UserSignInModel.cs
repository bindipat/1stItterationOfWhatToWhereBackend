using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class UserSignInModel
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        [Required]
        [MaxLength(10)]
        [LoginType(ErrorMessage = "Login type should be normal, insta, gmail, apple or facebook")]
        public string LoginType { get; set; }
        [Required]
        public string FirebaseToken { get; set; }
        [Required]
        [MaxLength(10)]
        [DeviceType(ErrorMessage = "Device type should be android or iphone")]
        public string DeviceType { get; set; }
    }
    public class LoginTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return ((string)value) == "normal" || ((string)value) == "insta" || ((string)value) == "apple" || ((string)value) == "gmail" || ((string)value) == "facebook";
        }
    }
    public class DeviceTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return ((string)value) == "android" || ((string)value) == "iphone";
        }
    }

    public class UserSignInModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UserSignInModel
            {
                Email = "Test@gmail.com",
                Password = "mypassword",
                LoginType = "gmail",
                FirebaseToken = "ZxxZxdassdgdc", 
                DeviceType = "iphone" 
            };
        }
    }
}
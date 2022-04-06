using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models.DataModel
{
    public class UserModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string ProfileImage { get; set; }
        public string LoginType { get; set; }
        public bool IsEmailVerified { get; set; }
        public string FirebaseToken { get; set; }
        public bool IsActive { get; set; }
        public string DeviceType { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Mobile { get; set; }
        public string Description { get; set; }
        public string Pronouns { get; set; }
    }
}
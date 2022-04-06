using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class EditProfileModel
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [MaxLength(8)]
        [RequiredGenderType(ErrorMessage = "Gender type should be male or female")]
        public string Gender { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(5)]
        [RequiredPronounsType(ErrorMessage = "Pronouns should be he, she or the")]
        public string Pronouns { get; set; }

        [MaxLength(15)]
        public string Mobile { get; set; }

        public DateTime? DOB { get; set; }
    }

    public class RequiredGenderTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return ((string)value) == "male" || ((string)value) == "female";
        }
    }

    public class RequiredPronounsTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return ((string)value) == "he" || ((string)value) == "she" || ((string)value) == "they";
        }
    }

    public class EditProfileModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditProfileModel
            {
                UserId = 1,
                UserName = "Test User",
                Gender = "male",
                Description = "Test Decription",
                Pronouns = "he",
                Mobile = "9876543210",
                DOB = DateTime.Now
            };
        }
    }
}
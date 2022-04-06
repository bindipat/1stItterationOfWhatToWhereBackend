using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;  
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Swashbuckle.Examples;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class NewUserModel : UserSignInModel
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
         
        
        [MaxLength(15)]
        public string Mobile { get; set; }
        
        [MaxLength(8)]
        [GenderType(ErrorMessage = "Gender type should be male or female")]
        public string Gender { get; set; }
         
        [MaxLength(500)]
        public string ProfileImage { get; set; }
       
        [Required]
        public bool IsEmailVerified { get; set; }
         
        public DateTime? DOB { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
     
    public class GenderTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            return value==null||((string)value) == "male" || ((string)value) == "female";
        }
    }

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }

    public class NewUserModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewUserModel
            { 
                UserName = "Test User",
                Gender = "male",
                Description = "Test Decription",                
                Mobile = "9876543210",
                DOB = DateTime.Now,
                ProfileImage="test.jpg",
                IsEmailVerified=false,
                Email = "Test@gmail.com",
                Password = "mypassword",
                LoginType = "gmail",
                FirebaseToken = "ZxxZxdassdgdc",
                DeviceType = "iphone"
            };
        }
    }
}
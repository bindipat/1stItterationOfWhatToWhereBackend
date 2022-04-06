using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatToWhere.Models
{
    public class ResponseModel
    { 
        public string message { get; set; }
        public bool issuccess { get; set; }
        public object data { get; set; }
    }
}
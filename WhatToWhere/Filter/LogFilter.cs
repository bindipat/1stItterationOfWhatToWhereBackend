using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WhatToWhere.Filter
{
    public class LogFilter : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(APILogFilter));

            var form = context.HttpContext.Request.Form;

            // need to convert Form object into dictionary so it can be converted to json properly
            var dictionary = form.AllKeys.ToDictionary(k => k, k => form[k]);

            // You'll need Newtonsoft.Json nuget package here.
            var jsonPostedData = JsonConvert.SerializeObject(dictionary);

            log.Debug(jsonPostedData);
        }
         
    }
}
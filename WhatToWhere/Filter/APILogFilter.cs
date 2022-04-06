using log4net;
using Newtonsoft.Json;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WhatToWhere.Filter
{
    public class APILogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            // I use instance wrapper around NLog logger. Apply your own logic here for creating logger.
            ILog log = log4net.LogManager.GetLogger(typeof(APILogFilter));

            var form = actionContext.ActionArguments;

            // need to convert Form object into dictionary so it can be converted to json properly
            //var dictionary = form.AllKeys.ToDictionary(k => k, k => form[k]);

            // You'll need Newtonsoft.Json nuget package here.
            var jsonPostedData = JsonConvert.SerializeObject(form);
            log.Info(actionContext.Request.RequestUri.OriginalString);
            log.Info(jsonPostedData);

        }
    }
}
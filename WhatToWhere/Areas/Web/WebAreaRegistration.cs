using System.Web.Mvc;

namespace WhatToWhere.Areas.Web
{
    public class WebAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Web";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Web_default",
                "Web/{controller}/{action}/{id}",
                new {area="Web", Controller="User", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
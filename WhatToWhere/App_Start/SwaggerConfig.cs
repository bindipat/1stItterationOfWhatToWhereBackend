using System.Web.Http;
using WebActivatorEx;
using WhatToWhere;
using Swashbuckle.Application;
using Swashbuckle.Examples;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WhatToWhere
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                { 
                    c.SingleApiVersion("v1", "Documentation Using Swagger UI");
                    c.OperationFilter<ExamplesOperationFilter>();

                    // Enable swagger descriptions
                    c.OperationFilter<DescriptionOperationFilter>();

                    // Enable swagger response headers
                    c.OperationFilter<AddResponseHeadersFilter>();

                    // Add (Auth) to action summary
                    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                    c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Employee Swagger UI");
                }); 
        }
    }
}

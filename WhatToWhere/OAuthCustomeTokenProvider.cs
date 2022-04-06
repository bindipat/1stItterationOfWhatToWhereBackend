using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WhatToWhere
{
    public class OAuthCustomeTokenProvider : OAuthAuthorizationServerProvider
    {
        #region[GrantResourceOwnerCredentials]
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var userid = context.UserName;
                var email = context.Password;


                var claims = new List<Claim>()
                    {
                       new Claim(ClaimTypes.Sid, userid),
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Email, email)
                    };

                claims.Add(new Claim(ClaimTypes.Role, "user"));

                var data = new Dictionary<string, string>
                    {
                        { "userName", email },
                        { "roles", "user"}
                    };
                var properties = new AuthenticationProperties(data);

                ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims,
                    Startup.OAuthOptions.AuthenticationType);

                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);

            });
        }
        #endregion

        #region[GrantRefreshToken]
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            // Change authentication ticket for refresh token requests  
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            // newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        } /**/
        #endregion

        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using Newtonsoft.Json.Converters;

namespace BPA.API.Web.Providers
{
    /// <summary>
    /// Custom authorization provider
    /// </summary>
    /// <remarks>
    /// PJ 10.5.2018: The custom properties (userName, roles, etc) logic comes from the 
    /// following two SO posts:
    /// http://stackoverflow.com/questions/24096634/return-user-roles-with-bearer-token-web-api
    /// http://stackoverflow.com/questions/24078905/aspnet-identity-2-customize-oauth-endpoint-response
    /// </remarks>
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        #region Fields

        IAuthRepository _repository;

        #endregion Fields


        #region Constructors

        public SimpleAuthorizationServerProvider()
        {
            _repository = new Istriwala.Services.Repositories.AuthRepository();
        }

        #endregion Constructors


        #region Public Methods

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            User user = await Task.Run<User>(() =>
            {
                return _repository.Login(context.UserName, context.Password);
            });

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("userId", user.Id.ToString()));
            string roles = String.Empty;

            if (user.Roles != null)
            {
                roles = Newtonsoft.Json.JsonConvert.SerializeObject(user.Roles.Select(x => x.ToString()));
            }
            AuthenticationProperties properties = CreateProperties(user.Id, user.UserName, user.EmailId, user.Name, roles);

            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);
        }

        public AuthenticationProperties CreateProperties(int id, string userName, string email, string name, string roles)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", id.ToString() },
                { "userName", userName },
                { "userEmail", email },
                { "name", name },
                { "roles", roles }
            };
            
            return new AuthenticationProperties(data);
        }

        #endregion Public Methods
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TFiN.OAuthLab.Helpers;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class ScopeAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public string Scope { get; set; }

        public ScopeAuthorizeAttribute(string scope)
        {
            Scope = scope;
        }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (!HasExistingBearerToken(authHeader))
                throw new HttpResponseException(buildUnAuthorizedMessage("Geen valide access token gevonden"));

            var tokenString = authHeader.Parameter;
            var accessToken = await DocumentDbRepository<BearerToken>.GetTokenByString(tokenString);

            if (accessToken == null)
                throw new HttpResponseException(buildUnAuthorizedMessage("Niet bestaande access token"));

            if (accessToken.Scopes.Contains(Scope))
            {
                return;
            }

            throw new HttpResponseException(buildUnAuthorizedMessage("Niet geautoriseerd voor deze scope"));
        }

        private static bool HasExistingBearerToken(AuthenticationHeaderValue authHeader)
        {
            return authHeader != null && authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) &&
                   !string.IsNullOrWhiteSpace(authHeader.Parameter);
        }

        private static HttpResponseMessage buildUnAuthorizedMessage(string msg)
        {
            return new HttpResponseMessage
            {
                Content =
                    new StringContent(msg),
                StatusCode = HttpStatusCode.Unauthorized
            };
        }
    }
}
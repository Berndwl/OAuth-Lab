using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TFiN.OAuthLab.Areas.OAuth.Models;
using TFiN.OAuthLab.Helpers;
using TFiN.Redis;
using static System.String;
using Error = TFiN.OAuthLab.Areas.OAuth.Models.Error;

namespace TFiN.OAuthLab.Areas.OAuth.Controllers
{
    public class TokenController : ApiController
    {
        // GET: OAuth/Token
        public async Task<object> Get()
        {
            var queryString = HttpContext.Current.Request.QueryString;

            var grantType = queryString[OAuthElement.GrantType];
            var code = queryString[OAuthElement.Code];
            var refreshTokenString = queryString[OAuthElement.RefreshToken];
            var redirectURI = queryString[OAuthElement.RedirectURI];
            var clientId = queryString[OAuthElement.ClientId];
            var clientSecret = queryString[OAuthElement.ClientIdSecret];


            using (var db = new OAuthClientModel())
            {
                var client = db.OAuthClients.FirstOrDefault(c => c.Id.ToString() == clientId && c.Secret == clientSecret
                                                            && c.RedirectURI == redirectURI);
                try
                {
                    if (client != null)
                    {
                        var accessTokenRequest = new AccessTokenRequest(grantType, code, client);

                        if (accessTokenRequest.GrantType == GrantType.AuthorizationCode)
                        {
                            var autorisatieCode =
                                await DocumentDbRepository<AutorisatieCode>.GetCodeByString(
                                    accessTokenRequest.TokenString);

                            if (autorisatieCode != null)
                            {
                                Validate(accessTokenRequest, autorisatieCode);

                                var refreshToken = new RefreshToken(autorisatieCode);
                                var accessToken = new BearerToken(autorisatieCode, refreshToken.TokenString);

                                await DocumentDbRepository<BearerToken>.CreateDocument(accessToken);
                                await DocumentDbRepository<RefreshToken>.CreateDocument(refreshToken);
                                await DocumentDbRepository<AutorisatieCode>.DeleteDocument(autorisatieCode.Id);

                                var currentUser = new OAuthUser()
                                {
                                    Client_id = autorisatieCode.ClientId,
                                    User_id = autorisatieCode.UserId,
                                    ClientEigenaar = false,
                                };

                                db.OAuthUsers.Add(currentUser);
                                db.SaveChanges();


                                return buildJsonResponse(accessToken);
                            }
                            else
                            {
                                throw new AutorisatieRequest.UnAuthorizedClientException(
                                    "Niet bestaande Autorisatiecode");
                            }
                        }

                        if (accessTokenRequest.GrantType == GrantType.RefreshToken)
                        {
                            if (string.IsNullOrEmpty(refreshTokenString))
                            {
                                throw new AutorisatieRequest.InvalidRequestException("Geen refresh_token opgegeven.");
                            }

                            accessTokenRequest.TokenString = refreshTokenString;

                            var refreshToken =
                                await DocumentDbRepository<RefreshToken>.GetTokenByString(accessTokenRequest.TokenString);

                            if (refreshToken != null)
                            {
                                var existingAccessToken = await DocumentDbRepository<BearerToken>
                                .GetAccessTokenByRefreshTokenString(
                                    refreshTokenString);

                                if (existingAccessToken != null)
                                {
                                    await DocumentDbRepository<BearerToken>.DeleteDocument(existingAccessToken.Id);
                                }


                                var accessToken = new BearerToken(refreshToken);

                                await DocumentDbRepository<BearerToken>.CreateDocument(accessToken);

                                return buildJsonResponse(accessToken);
                            }
                            else
                            {
                                throw new AutorisatieRequest.UnAuthorizedClientException(
                                    "Niet bestaande RefreshToken");
                            }

                        }

                        throw new AutorisatieRequest.InvalidRequestException("Niet valide grant type opgegeven.");
                    }
                    else
                    {
                        throw new AutorisatieRequest.UnAuthorizedClientException("Niet bestaande client");
                    }
                }
                catch (AutorisatieRequest.UnAuthorizedClientException e)
                {
                    return buildJsonErrorResponse(Error.UnauthorizedClient, e.Message);
                }
                catch (AutorisatieRequest.InvalidRequestException e)
                {
                    return buildJsonErrorResponse(Error.InvalidRequest, e.Message);
                }
            }
        }

        private JObject buildJsonResponse(BearerToken token)
        {
            var json = new JObject
            {
                [OAuthElement.AccessToken] = token.TokenString,
                [OAuthElement.TokenType] = TokenType.Bearer.ToString(),
                [OAuthElement.ExpiresIn] = token.TimeToLive,
                [OAuthElement.Scope] = string.Join(" ", token.Scopes.ToArray()),
                [OAuthElement.RefreshToken] = token.RefreshTokenString,
            };

            return json;
        }

        private JObject buildJsonErrorResponse(string name, string msg)
        {
            var json = new JObject
            {
                [OAuthElement.Error] = name,
                [OAuthElement.ErrorDescription] = msg,
            };


            return json;
        }

        private void Validate(AccessTokenRequest accessTokenRequest, AutorisatieCode autorisatieCode)
        {
            if (IsNullOrEmpty(accessTokenRequest.TokenString))
            {
                throw new AutorisatieRequest.InvalidRequestException("Autorisatie Code is verplicht");
            }

            if (!(autorisatieCode.ClientId == accessTokenRequest.Application.Id &&
                  autorisatieCode.RedirectURI.Equals(accessTokenRequest.Application.RedirectURI,
                      StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new AutorisatieRequest.InvalidRequestException(
                    "Autorisatie Code komt niet overeen met ingevoerde client_id en redirect_uri");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TFiN.OAuthLab.Areas.OAuth.Models;
using TFiN.OAuthLab.Helpers;
using static System.String;

namespace TFiN.OAuthLab.Areas.OAuth.Controllers
{
    public class AuthController : Controller
    {
        // GET: OAuth/Auth
        public ActionResult Authorize()
        {
            var requestedResponseType = Request.QueryString[OAuthElement.ResponseType];
            var clientId = Request.QueryString[OAuthElement.ClientId];
            var redirectURI = Request.QueryString[OAuthElement.RedirectURI];
            var scopes = Request.QueryString[OAuthElement.Scope];
            var state = Request.QueryString[OAuthElement.State];

            using (var db = new OAuthClientModel())
            {
                var client = db.OAuthClients.FirstOrDefault(c => c.Id.ToString() == clientId
                                                            && c.RedirectURI == redirectURI);
                try
                {
                    if (client != null)
                    {
                        AutorisatieRequest authorization = new AutorisatieRequest(requestedResponseType, clientId,
                            redirectURI, scopes, state);


                        var allowedClientScopes = JsonConvert.DeserializeObject<List<string>>(client.Scope);

                        var notAllowedScopes = authorization.Scopes.Except(allowedClientScopes);

                        if (notAllowedScopes.ToList().Count > 0)
                        {
                            throw new AutorisatieRequest.InvalidScopeException("Client heeft geen toegang tot een van de opgegeven scopes");
                        }
                        

                        Validate(authorization);

                        AutorisatieRequestViewModel model = new AutorisatieRequestViewModel(authorization, client);

                        model.ScopeBeschrijvingen = GetScopeBeschrijvingBijScope(model.Scopes);

                        return View(model);
                    }
                    throw new AutorisatieRequest.UnAuthorizedClientException(
                        "Client id en redirect URI combinatie bestaat niet");
                }

                catch (AutorisatieRequest.UnAuthorizedClientException e)
                {
                    ViewData["errorName"] = Error.InvalidRequest;
                    ViewData["error"] = e.Message;
                    return View("Error");
                }
                catch (AutorisatieRequest.InvalidRequestException e)
                {
                    ViewData["errorName"] = Error.InvalidRequest;
                    ViewData["error"] = e.Message;
                    return View("Error");
                }
                catch (AutorisatieRequest.UnsupportedResponseTypeException e)
                {
                    ViewData["errorName"] = Error.UnsupportedResponseType;
                    ViewData["error"] = e.Message;
                    return View("Error");
                }
                catch (AutorisatieRequest.InvalidScopeException e)
                {
                    ViewData["errorName"] = Error.InvalidScope;
                    ViewData["error"] = e.Message;
                    return View("Error");
                }
            }
        }

        [HttpPost]
        public virtual async Task<ActionResult> AuthorizationResult(AutorisatieRequestViewModel model,
            string authorizationResult)
        {
            try
            {
                switch (authorizationResult)
                {
                    case "success":

                        var code = new AutorisatieCode(model)
                        {
                            UserId = new Guid(),
                        };

                        var res = await DocumentDbRepository<RefreshToken>.GetCodeOrTokenByIds(code.ClientId,
                            code.UserId);

                        if (res != null)
                        {
                            //TODO
                            // Dit delete refreshtoken en genereert nieuw autorisatiecode, als deze autorisatie code niet meteen wordt gerequest wordt alles gedelete.
                            await DocumentDbRepository<RefreshToken>.DeleteDocument(res.Id);
                            
                        }

                        await DocumentDbRepository<AutorisatieCode>.CreateDocument(code);

                        var successRedirectURI = BuildRedirectURI(model, code);

                        return Redirect(successRedirectURI);

                    case "cancel":
                        var errorRedirectURI = BuildRedirectURI(model);

                        return Redirect(errorRedirectURI);

                    default:
                        throw new AutorisatieRequest.InvalidRequestException("Invalide submit waarde");
                }
            }
            catch (AutorisatieRequest.InvalidRequestException e)
            {
                ViewData["errorName"] = Error.InvalidRequest;
                ViewData["error"] = e.Message;
                return View("Error");
            }
        }

        private string BuildRedirectURI(AutorisatieRequestViewModel model, AutorisatieCode code = null)
        {
            var uriBuilder = new UriBuilder(model.RedirectURI);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            SetQueryParameters(model, code, query);
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private static void SetQueryParameters(AutorisatieRequestViewModel model, AutorisatieCode code,
            System.Collections.Specialized.NameValueCollection query)
        {
            if (code != null)
            {
                query[OAuthElement.Code] = code.CodeString;
            }
            else
            {
                query[OAuthElement.Error] = Error.AccessDenied;
            }

            if (!IsNullOrEmpty(model.State))
            {
                query[OAuthElement.State] = model.State;
            }
        }

        private List<string> GetScopeBeschrijvingBijScope(List<string> scopeNamen)
        {
            var beschrijvingen = new List<string>();

            foreach (var scope in scopeNamen)
            {
                beschrijvingen.Add(Scope.ScopeList[scope]);
            }

            return beschrijvingen;
        }

        private void Validate(AutorisatieRequest autorisatieRequest)
        {
            if (IsNullOrEmpty(autorisatieRequest.ClientId))
            {
                throw new AutorisatieRequest.InvalidRequestException("ClientId is verplicht");
            }
            else if (IsNullOrEmpty(autorisatieRequest.RedirectURI))
            {
                throw new AutorisatieRequest.InvalidRequestException("RedirectURI is verplicht");
            }
            else if (IsNullOrEmpty(autorisatieRequest.State))
            {
                throw new AutorisatieRequest.InvalidRequestException("State is verplicht");
            }
        }
    }
}
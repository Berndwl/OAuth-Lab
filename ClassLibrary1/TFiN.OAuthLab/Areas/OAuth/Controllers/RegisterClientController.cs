using System;
using System.Security.Authentication;
using System.Web.Mvc;
using TFiN.OAuthLab.Areas.OAuth.Models;

namespace TFiN.OAuthLab.Areas.OAuth.Controllers
{
    public class RegisterClientController : Controller
    {
        // GET: OAuth/Register
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(ClientViewModel client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new OAuthClientModel())
                    {
                        var clientEntity = new OAuthClient(client);

                        if (!isValidURL(client.RedirectURI))
                        {
                            throw new AutorisatieRequest.InvalidRequestException("Geen absolute TLS url opgegeven");
                        }

                        db.OAuthClients.Add(clientEntity);
                        db.SaveChanges();
                        return View(client);
                    }
                }
            }
            catch (AutorisatieRequest.InvalidRequestException e)
            {

                ViewData["errorName"] = Error.InvalidRequest;
                ViewData["error"] = e.Message;
                return View("~/Views/Shared/Error.cshtml");
            }

            return View("Error");
        }

        private bool isValidURL(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TFiN.OAuthLab.Areas.OAuth.Models;
using TFiN.OAuthLab.Helpers;

namespace TFiN.OAuthLab.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/value       
        [ScopeAuthorize(Scope.ReadDossier)]
        public async Task<IEnumerable<string>> Get()
        {
            var id = await GetUserIdByTokenDatabase();
            return new[] {"userid", id };
        }

        // GET api/values/5
        [ScopeAuthorize(Scope.CreateDossier)]
        public IEnumerable<string> Get(int num)
        {
            var id = GetUserIdByToken();
            return new[] { "userid", id.ToString() };
        }

        // POST api/values
        [ScopeAuthorize(Scope.UpdateDossier)]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        private Guid GetUserIdByToken()
        {
            var auth = ActionContext.Request.Headers.Authorization;
            var bearer = auth.Parameter;
            var rawText = BearerTokenHelper.GetUserIdFromToken(bearer, "samplekey");

            return new Guid(rawText);
        }

        private async Task<string> GetUserIdByTokenDatabase()
        {
            var auth = ActionContext.Request.Headers.Authorization;
            var bearer = auth.Parameter;
            var token = await DocumentDbRepository<BearerToken>.GetTokenByString(bearer);

            if (token != null)
            {
                return token.UserId.ToString();
            }

            return "";
        }
    }
}
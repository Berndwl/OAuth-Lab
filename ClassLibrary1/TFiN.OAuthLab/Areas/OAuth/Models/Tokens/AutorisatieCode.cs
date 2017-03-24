using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TFiN.OAuthLab.Helpers;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public sealed class AutorisatieCode
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        // Representatie in seconden , minuut default
        private const int ExpiryTime = 60;
        public string Type { get; set; }
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }
        public Guid ClientId { get; set; }
        public Guid UserId { get; set; }
        public string RedirectURI { get; set; }
        public List<string> Scopes { get; set; }
        public string State { get; set; }
        public string CodeString { get; set; }

        public AutorisatieCode()
        {
            
        }

        public AutorisatieCode(AutorisatieRequestViewModel model)
        {
            TimeToLive = ExpiryTime;
            Type = GetType().Name;        
            ClientId = model.Client.Id;
            RedirectURI = model.RedirectURI;
            Scopes = model.Scopes;
            State = model.State;
            CodeString = GenerateCode(ClientId, UserId, Type);

        }

        private string GenerateCode(Guid client, Guid user, string type)
        {
            return TokenGenerate.GenerateToken(client, user, type);
        }

    }
}
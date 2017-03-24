using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TFiN.OAuthLab.Helpers;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public sealed class RefreshToken : Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public override string Type { get; set; }
        public override Guid ClientId { get; set; }
        public override Guid UserId { get; set; }
        public override string TokenString { get; set; }
        public override List<string> Scopes { get; set; }

        public RefreshToken()
        {
            
        }

        public RefreshToken(AutorisatieCode code)
        {
            Type = GetType().Name;
            ClientId = code.ClientId;
            UserId = code.UserId;
            Scopes = code.Scopes;
            TokenString = GenerateToken(ClientId, UserId, Type);
        }

        protected override string GenerateToken(Guid client, Guid user, string type)
        {
            return TokenGenerate.GenerateToken(client, user, type);
        }
    }
}
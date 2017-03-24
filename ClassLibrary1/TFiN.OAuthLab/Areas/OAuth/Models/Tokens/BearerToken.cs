using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TFiN.OAuthLab.Helpers;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public sealed class BearerToken : Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public const int ExpiryTime = 3600;
        public override string Type { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }
        public string RefreshTokenString { get; set; }
        public override Guid ClientId { get; set; }
        public override Guid UserId { get; set; }
        public override string TokenString { get; set; }
        public override List<string> Scopes { get; set; }

        public BearerToken()
        {
            
        }

        public BearerToken(AutorisatieCode code, string refreshToken)
        {
            RefreshTokenString = refreshToken;
            TimeToLive = ExpiryTime;
            Type = GetType().Name;           
            ClientId = code.ClientId;
            UserId = code.UserId;
            Scopes = code.Scopes;
            TokenString = TokenGenerate.GenerateToken(ClientId, UserId, Type);
        }

        public BearerToken(RefreshToken token)
        {
            RefreshTokenString = token.TokenString;
            TimeToLive = ExpiryTime;
            Type = GetType().Name;
            ClientId = token.ClientId;
            UserId = token.UserId;
            Scopes = token.Scopes;
            TokenString = TokenGenerate.GenerateToken(ClientId, UserId, Type);
        }

        protected override string GenerateToken(Guid client, Guid user, string type)
        {
            return TokenGenerate.GenerateToken(client, user, type);
        }
    }
}
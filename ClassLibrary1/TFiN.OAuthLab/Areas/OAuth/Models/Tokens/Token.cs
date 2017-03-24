using System;
using System.Collections.Generic;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public abstract class Token
    {
        public abstract string Type { get; set; }
        public abstract Guid ClientId { get; set; }
        public abstract Guid UserId { get; set; }
        public abstract string TokenString { get; set; }
        public abstract List<string> Scopes { get; set; }   

        protected abstract string GenerateToken(Guid client, Guid user, string type);

    }
}
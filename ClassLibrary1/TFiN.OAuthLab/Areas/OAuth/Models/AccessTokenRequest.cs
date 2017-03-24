using System;
using System.Collections.Generic;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class AccessTokenRequest
    {
        public GrantType GrantType { get; set; }
        public string TokenString { get; set; }
        public OAuthClient Application { get; set; }

        public AccessTokenRequest(GrantType grantType, string code, OAuthClient application)
        {
            GrantType = grantType;
            TokenString = code;
            Application = application;
        }

        public AccessTokenRequest(string grantType, string code, OAuthClient application)
        {
            if (grantType.Equals(OAuthElement.AuthorizationCode, StringComparison.OrdinalIgnoreCase))
            {
                GrantType = GrantType.AuthorizationCode;
            }
            else if (grantType.Equals(OAuthElement.RefreshToken, StringComparison.OrdinalIgnoreCase))
            {
                GrantType = GrantType.RefreshToken;
            }
            else
            {
                throw new AutorisatieRequest.InvalidRequestException("Niet valide grant type opgegeven");
            }
            TokenString = code;
            Application = application;
        }
    }
}
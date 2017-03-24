using System;
using System.Collections.Generic;
using System.Linq;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class AutorisatieRequest
    {
        public ResponseType ResponseType { get; set; }
        public string ClientId { get; set; }
        public string RedirectURI { get; set; }

        public List<string> Scopes { get; set; }
        public string State { get; set; }

        public AutorisatieRequest(ResponseType responseType, string clientId, string redirectUri,
            List<string> scopes, string state)
        {
            ResponseType = responseType;
            ClientId = clientId;
            RedirectURI = redirectUri;
            Scopes = scopes;
            State = state;
        }

        public AutorisatieRequest(string response, string clientId, string redirectUri,
            string scopes, string state)
        {
            ResponseType responseType;
            if (Enum.TryParse(response, out responseType) && responseType == ResponseType.Code)
            {
                ResponseType = responseType;
                ClientId = clientId;
                RedirectURI = redirectUri;
                Scopes = ScopeStringToScopeList2(scopes);
                State = state;
            }
            else
            {
                throw new UnsupportedResponseTypeException("Niet valide response type opgegeven");
            }
        }

        private bool HasNotExistingScopes(IEnumerable<string> scopes)
        {
            var availableScopes = new List<string>(Scope.ScopeList.Keys);
            var invalidScopes = scopes.ToList().Except(availableScopes).ToList();

            return invalidScopes.Count != 0;
        }

        private bool HasDuplicatedScopes(List<string> scopes)
        {
            var filteredScopes = scopes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            return filteredScopes.Count != scopes.Count;
        }

        private List<string> ScopeStringToScopeList(string str)
        {
            var delimiterChar = new [] {' '};

            return str.Split(delimiterChar, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public List<string> ScopeStringToScopeList2(string scopes)
        {
            if (string.IsNullOrEmpty(scopes)) throw new InvalidScopeException("Geen scope opgegeven");
            var scopeList = ScopeStringToScopeList(scopes);

            if (HasDuplicatedScopes(scopeList))
            {
                throw new InvalidScopeException("Dubbele scopes opgegeven");
            }

            else if (HasNotExistingScopes(scopeList))
            {
                throw new InvalidScopeException("Niet bestaande scope(s) opgegeven");
            }

            return scopeList;
        }

        public class InvalidRequestException : Exception
        {
            public InvalidRequestException(string message) : base(message)
            {
            }
        }

        public class UnsupportedResponseTypeException : Exception
        {
            public UnsupportedResponseTypeException(string message) : base(message)
            {
            }
        }

        public class InvalidScopeException : Exception
        {
            public InvalidScopeException(string message) : base(message)
            {
            }
        }

        public class UnAuthorizedClientException : Exception
        {
            public UnAuthorizedClientException(string message) : base(message)
            {
            }
        }

        public class AccessDeniedException : Exception
        {
            public AccessDeniedException(string message) : base(message)
            {
            }
        }

        public class AutorisatieException : Exception
        {
            public AutorisatieException(string message) : base(message)
            {
            }
        }

        public AutorisatieRequest()
        {
        }
    }
}
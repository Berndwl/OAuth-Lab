using System.Collections.Generic;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class Error
    {
        public static readonly string InvalidRequest = "invalid_request";
        public static readonly string UnauthorizedClient = "unauthorized_client";
        public static readonly string AccessDenied = "access_denied";
        public static readonly string UnsupportedResponseType = "unsupported_response_type";
        public static readonly string InvalidScope = "invalid_scope";
        public static readonly string ServerError = "server_error";
        public static readonly string TemporarilyUnavailable = "temporarily_unavailable";


        //TODO default beschrijvingen bij errors
        public static readonly Dictionary<string, string> ErrorList = new Dictionary<string, string>()
        {
            {InvalidRequest, "Kan dossier aanmaken"},
            {UnauthorizedClient, "Kan dossier aanmaken"},
            {AccessDenied, "Kan dossier aanmaken"},
            {UnsupportedResponseType, "Kan dossier aanmaken"},
            {InvalidScope, "Kan dossier aanmaken"},
            {ServerError, "Kan dossier aanmaken"},
            {TemporarilyUnavailable, "aanmaken"},
        };
    }
}
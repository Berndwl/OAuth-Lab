using System.Collections.Generic;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class Scope
    {
        public const string CreateDossier = "dossier:create";
        public const string ReadDossier = "dossier:read";
        public const string UpdateDossier = "dossier:update";
        public const string DeleteDossier = "dossier:delete";
        public const string CreateProfile = "profile:create";
        public const string ReadProfile = "profile:read";
        public const string UpdateProfile = "profile:update";
        public const string DeleteProfile = "profile:delete";


        public static readonly Dictionary<string, string> ScopeList = new Dictionary<string, string>()
        {
            {CreateDossier, "Kan dossier aanmaken"},
            {ReadDossier, "Kan dossier lezen"},
            {UpdateDossier, "beschrijving"},
            {DeleteDossier, "beschrijving"},
            {CreateProfile, "beschrijving"},
            {ReadProfile, "beschrijving"},
            {UpdateProfile, "beschrijving"},
            {DeleteProfile, "beschrijving"}
        };
    }
}
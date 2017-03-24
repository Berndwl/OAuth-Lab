using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class AutorisatieRequestViewModel
    {
        [Required]
        [Display(Name = "Client")]
        public OAuthClient Client { get; set; }

        [Required]
        [Display(Name = "Response Type")]
        public ResponseType ResponseType { get; set; }

        [Required]
        [Display(Name = "Redirect URI")]
        public string RedirectURI { get; set; }

        [Required]
        [Display(Name = "Scope")]
        public List<string> Scopes { get; set; }

        [Required]
        [Display(Name = "Scope Beschrijvingen")]
        public List<string> ScopeBeschrijvingen { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        public AutorisatieRequestViewModel(AutorisatieRequest request, OAuthClient client)
        {
            RedirectURI = request.RedirectURI;
            ResponseType = request.ResponseType;         
            Scopes = request.Scopes;
            State = request.State;
            Client = client;
        }

        public AutorisatieRequestViewModel()
        {
        }
    }
}
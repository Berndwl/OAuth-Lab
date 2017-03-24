using System;
using System.ComponentModel.DataAnnotations;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class ClientViewModel
    {
        public Guid Id { get; set; }

        public string Secret { get; set; }

        [Required]
        [Display(Name = "Client naam")]
        public string Naam { get; set; }

        [Required]
        [Display(Name = "Client website")]
        public string Website { get; set; }

        [Required]
        [Display(Name = "Redirect URI")]
        public string RedirectURI { get; set; }

        public ClientViewModel(string naam, string website, string redirect)
        {
            Naam = naam;
            Website = website;
            RedirectURI = redirect;
        }

        public ClientViewModel()
        {
        }
    }
}
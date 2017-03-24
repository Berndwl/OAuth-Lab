using System;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using TFiN.Domain.Entities;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public class Client : Entity<Guid>
    {
        public string Secret { get; set; }
        public string Naam { get; set; }
        public string Website { get; set; }
        public string RedirectURI { get; set; }

        public Client(string naam, string website, string redirect)
        {
            Naam = naam;
            Website = website;
            RedirectURI = redirect;
            Secret = CalculateMD5Hash();
        }

        public Client()
        {
        }

        public Client(ClientViewModel model)
        {
            Naam = model.Naam;
            Website = model.Website;
            RedirectURI = model.RedirectURI;
            Secret = CalculateMD5Hash();
        }

        public string CalculateMD5Hash()
        {
            var secret = Id + DateTime.Now.ToString();

            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(secret);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {
                stringBuilder.Append(hash[i].ToString("X2"))
                ;
            }

            return stringBuilder.ToString();
        }
    }
}
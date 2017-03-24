using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;
using TFiN.OAuthLab.Areas.OAuth.Models;

namespace TFiN.OAuthLab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OAuthClient")]
    public partial class OAuthClient
    {
        public Guid Id { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        [StringLength(255)]
        public string Naam { get; set; }

        [Required]
        [StringLength(255)]
        public string Website { get; set; }

        [Required]
        [StringLength(255)]
        public string RedirectURI { get; set; }

        [Required]
        public string Scope { get; set; }

        public bool Actief { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OAuthUser> OAuthUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OAuthClient()
        {
            OAuthUsers = new HashSet<OAuthUser>();
        }

        public OAuthClient(ClientViewModel model)
        {
            Id = new Guid();
            Secret = CalculateMD5Hash();
            Naam = model.Naam;
            Website = model.Website;
            RedirectURI = model.RedirectURI;
            Scope =  JsonConvert.SerializeObject(TFiN.OAuthLab.Areas.OAuth.Models.Scope.ScopeList.Keys);
            Actief = true;
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


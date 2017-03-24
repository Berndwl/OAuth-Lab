namespace TFiN.OAuthLab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            OAuthUsers = new HashSet<OAuthUser>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [MaxLength(1024)]
        public byte[] Password { get; set; }

        [Required]
        [StringLength(255)]
        public string Achternaam { get; set; }

        [Required]
        [StringLength(16)]
        public string Voorletters { get; set; }

        [Required]
        [StringLength(35)]
        public string Voornaam { get; set; }

        [StringLength(10)]
        public string Tussenvoegsels { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OAuthUser> OAuthUsers { get; set; }
    }
}

namespace TFiN.OAuthLab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OAuthUser")]
    public partial class OAuthUser
    {
        [Key]
        [Column(Order = 0)]
        public Guid User_id { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid Client_id { get; set; }

        public bool ClientEigenaar { get; set; }

        public virtual OAuthClient OAuthClient { get; set; }

        public virtual User User { get; set; }
    }
}

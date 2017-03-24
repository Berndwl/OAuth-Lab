namespace TFiN.OAuthLab
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class OAuthClientModel : DbContext
    {
        public OAuthClientModel()
            : base("name=OAuthClientModel")
        {
        }

        public virtual DbSet<OAuthClient> OAuthClients { get; set; }
        public virtual DbSet<OAuthUser> OAuthUsers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OAuthClient>()
                .HasMany(e => e.OAuthUsers)
                .WithRequired(e => e.OAuthClient)
                .HasForeignKey(e => e.Client_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.OAuthUsers)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_id)
                .WillCascadeOnDelete(false);
        }
    }
}

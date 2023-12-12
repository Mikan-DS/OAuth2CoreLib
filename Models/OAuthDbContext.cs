using Microsoft.EntityFrameworkCore;
using OAuth2CoreLib.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Reflection.Emit;

namespace OAuth2CoreLib
{
    public class OAuthDbContext: DbContext
    {
        public OAuthDbContext(DbContextOptions<OAuthDbContext> options): base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Client>()
        //        .HasMany(c => c.ClientScope)
        //        .WithOne(e => e.Client)
        //        .HasForeignKey(c => c.Id);
        //}

        public DbSet<Client> Clients { get; set; }
        public DbSet<ResourceScope> ResourceScopes { get; set; }
        public DbSet<ClientScope> ClientScopes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAllowedScope> UserAllowedScopes { get; set; }
        public DbSet<AuthorizationCode> AuthorizationCodes { get; set; }
        public DbSet<AuthorizationCodeScope> AuthorizationCodeScopes { get; set; }

    }
}

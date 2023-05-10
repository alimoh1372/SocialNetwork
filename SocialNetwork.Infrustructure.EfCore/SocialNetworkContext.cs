using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.UserAgg;
using SocialNetwork.Domain.UserRelationAgg;
using SocialNetwork.Infrastructure.EfCore.Mapping;

namespace SocialNetwork.Infrastructure.EfCore
{
    public class SocialNetworkContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRelation> UserRelations { get; set; }

        public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Get the assembly of mapping
            Assembly assembly = typeof(UserMapping).Assembly;

            //apply all mapping to context 
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
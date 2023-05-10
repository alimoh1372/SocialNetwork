using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.UserAgg;

namespace SocialNetwork.Infrastructure.EfCore
{
    public class SocialNetworkContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}
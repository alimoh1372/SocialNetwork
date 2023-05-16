using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Domain.UserAgg;
using SocialNetwork.Infrastructure.EfCore;
using SocialNetwork.Infrastructure.EfCore.Repository;

namespace SocialNetwork.Infrastructure.Configuration
{
    public  class SocialNetworkBootstrapper
    {
        /// <summary>
        /// wire up the Social network needed injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">wire up the context of system with this connection string</param>
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IUserRepository,UserRepository>();
            services.AddTransient<IUserApplication, UserApplication>();
            services.AddDbContext<SocialNetworkContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
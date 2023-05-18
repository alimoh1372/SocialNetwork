using _01_SocialNetworkQuery.Contracts;
using _01_SocialNetworkQuery.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Application.Contracts.UserRelationContracts;
using SocialNetwork.Domain.UserAgg;
using SocialNetwork.Domain.UserRelationAgg;
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

            services.AddTransient<IUserRelationApplication, UserRelationApplication>();
            services.AddTransient<IUserRelationRepository, UserRelationRepository>();
            services.AddTransient<IUserQuery, UserQuery>();
            services.AddTransient<IUserRelationQuery, UserRelationQuery>();
            services.AddDbContext<SocialNetworkContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
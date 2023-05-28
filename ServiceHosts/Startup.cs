using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using _0_Framework.Application;
using _00_Framework.Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ServiceHosts.Hubs;
using ServiceHosts.Tools;
using ServiceHosts.Tools.Implementation;
using SocialNetwork.Infrastructure.Configuration;


namespace ServiceHosts
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             string connectionString = Configuration.GetConnectionString("socialNetworkConnectionStringHome");
           // string connectionString = Configuration.GetConnectionString("socialNetworkConnectionStringNoc");
            SocialNetworkBootstrapper.Configure(services,connectionString);

            //wire up and register the needed services
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthHelper, AuthHelper>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddTransient<IFileUpload, FileUpload>();
            //To register the signalR
            services.AddSignalR();

            //To set the cookie and policy behavior
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Set the authenticate pages
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/Index");
                    o.LogoutPath = new PathString("/Index");
                    o.AccessDeniedPath = new PathString("/AccessDenied");
                });
            //To support the persian words in client side
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic));


            //Define the policy needed 
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ChatPage",
                    builder => builder.RequireClaim("UserId"));
                options.AddPolicy("ChatHub",
                   builder => builder.RequireClaim("UserId"));
            });

            //To set the Authentication policy to some pages,folder ,...
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizePage("/ChatPage", "ChatPage");
                    options.Conventions.AuthorizeFolder("/Hubs", "ChatHub");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/ErrorWithRelationNumbers");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                

                //Define the signalR route
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;


namespace _00_Framework.Application
{
    /// <summary>
    /// A helper class to Authentication and authorization
    /// </summary>
    public class AuthHelper : IAuthHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Get the current user identity info
        /// </summary>
        /// <returns></returns>
        public AuthViewModel CurrentAccountInfo()
        {
            var result = new AuthViewModel();
            if (!IsAuthenticated())
                return result;

            var claims = _contextAccessor.HttpContext.User.Claims.ToList();
            result.Id = long.Parse(claims.FirstOrDefault(x => x.Type == "UserId")?.Value);
            result.Username = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            return result;
        }

      
        

        /// <summary>
        /// Specify the user was authenticated or not
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            
            return _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        }


        /// <summary>
        /// Sign in user and add claim principle in the cookie and redirect to main page
        /// </summary>
        /// <param name="account"></param>
        public void Signin(AuthViewModel account)
        {
           
            var claims = new List<Claim>
            {
                new Claim("UserId", account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }


        /// <summary>
        /// To sign out the current user
        /// </summary>
        public void SignOut()
        {
            _contextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
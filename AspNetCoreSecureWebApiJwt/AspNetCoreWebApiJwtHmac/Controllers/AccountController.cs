using AspNetCoreWebApiJwtHmac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreWebApiJwtHmac.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public AccountController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpPost]
        public IActionResult Authenticate([FromBody] AccountModel account)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Credentials cannot be verified!");
            }

            var claims = Verify(account.UserName, account.Password);

            if (claims is null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Credentials cannot be verified!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingSecurityKey = Encoding.UTF8.GetBytes(Configuration["Jwt:TokenSecurityKey"]);
            var tokenExpirationTime = DateTime.UtcNow.AddDays(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingSecurityKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new JsonResult(new JwtTokenModel { AccessToken = accessToken, ExpirationTime = tokenExpirationTime });
        }

        private IList<Claim> Verify(string username, string password)
        {
            // You can check here againest any identity system and for the demo we will assume they are valid

            var isValidCredentials = true;

            // After the verification return the claims that you want to use 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  username),
                new Claim(ClaimTypes.GivenName, "Ahmed"),
                new Claim(ClaimTypes.Surname, "Magdy"),
                new Claim(ClaimTypes.Webpage, "http://hackitright.com")
            };

            // Just for DEMO purposes, if the use is admin then we add it a role: Admins
            if (username.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admins"));
            }

            return isValidCredentials ? claims : null;
        }
    }
}

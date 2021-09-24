using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CustomerService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
//using Microsoft.IdentityModel.Tokens.Jwt;

namespace CustomerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configReader;
        private readonly IList<Credential> appusers = new List<Credential>
        {
            new Credential{FullName="Admin User",UserName="admin",Password="1234",UserRole="Admin" },
            new Credential{FullName="Test User",UserName="user",Password="1234",UserRole="User" }
        };
        public LoginController(IConfiguration config)
        {
            this.configReader = config;
        }    
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Credential credential)
        {
            IActionResult unauthorizedresponse=base.Unauthorized();
            var user= this.GetMatchingUserFromRepo(credential);
            if (user != null)
            {
                var tokenstring=this.GenerateJWTToken(user);
                unauthorizedresponse = base.Ok(new
                {
                    token = tokenstring,
                    user.UserRole,
                    user.FullName
                });
            }
            return unauthorizedresponse;
        }
        private Credential GetMatchingUserFromRepo(Credential loginCredential)
        {
            Credential matchinguserfound = this.appusers.SingleOrDefault(x => x.UserName == loginCredential.UserName &&
              x.Password == loginCredential.Password);
            return matchinguserfound;
        }
         private string GenerateJWTToken(Credential credential)
          {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configReader["Jwt:SecretKey"]));
            var SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //  var secretkey = this.configReader["Jwt:SecretKey"];
            // var jwtIssuer = this.configReader["Jwt:Issuer"];
            //var jwtAudience=this.configReader["Jwt:Audience"];
        //  const string encryption_key = "xecretKeywqejane";
        //    const string jwtIssuer = "[https://localhost:55163/]https://localhost:55163/";
         // const string jwtAudience= "[https://localhost:55163/]https://localhost:55163/";
            


            // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));
          //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryption_key));
            //var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
              var claims = new[]
              {
                  new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,credential.UserName),
                  new Claim("FullName",credential.FullName),
                  new Claim("role",credential.UserRole),
                  new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
              };
             var token = new JwtSecurityToken
              (
                  issuer: configReader["Jwt:Issuer"],
                audience: configReader["Jwt:Audience"],
                  claims: claims,
                  expires: DateTime.Now.AddMinutes(30),
                  signingCredentials:SigningCredentials
              );
              return new JwtSecurityTokenHandler().WriteToken(token);
          
         }
     
    }
}

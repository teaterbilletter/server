using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ticketsbackend.Models;
using ticketsbackend.SoapService;

namespace ticketsbackend.Controllers.v1
{
    [Route("/[controller]")]
    public class AuthenticateController : Controller
    {
        private List<string> admins = new List<string>
        {
            "Emil Glimø Vinkel", "Frederik Egegaard Hjorth", "Rasmus Kromann Hjorth", "Nicolas Kaveh Vahman",
            "Jaafar Fadhil Abdul-Mahdi", "Jacob Nordfalk", "Monica Lund Schmidt"
        };


        [HttpPost, Route("~/AdminLogin")]
        public async Task<IActionResult> AdminLogin([FromBody] Login login)
        {
            if (login == null)
            {
                return BadRequest("Invalid client request");
            }

            var log = await CheckUserWithSoap.SoapEnvelope(login.Name, login.Password);

            if (!admins.Contains(log)) return BadRequest("Ikke Administrator");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin")
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.key));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                "http://+:5000",
                "http://+",
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new LoginResult(log, tokenString));
        }

        [HttpPost, Route("~/UserLogin")]
        public async Task<IActionResult> UserLogin([FromBody] Login login)
        {
            if (login == null)
            {
                return BadRequest("Invalid client request");
            }

            var log = await CheckUserWithSoap.SoapEnvelope(login.Name, login.Password);


            if (log.Equals("Forkert brugernavn eller adgangskode!"))
                return BadRequest("Forkert brugernavn eller password!");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Name)
            };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.key));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                "http://+:5000",
                "http://+",
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new LoginResult(log, tokenString));
        }
    }
}
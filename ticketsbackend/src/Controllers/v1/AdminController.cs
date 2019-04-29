using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ticketsbackend.Models;
using ticketsbackend.SoapService;

namespace ticketsbackend.Controllers.v1
{
    [Authorize]
    [Route("/[controller]")]
    public class AdminController : Controller
    {
        private CustomerDB customerDb;
        private BookingDB bookingDb;

        private List<string> admins = new List<string>
        {
            "Emil Glimø Vinkel", "Frederik Egegaard Hjorth", "Rasmus Kromann Hjorth", "Nicolas Kaveh Vahman",
            "Jaafar Fadhil Abdul-Mahdi", "Jacob Nordfalk", "Monica Lund Schmidt"
        };

        public AdminController(IConfiguration configuration)
        {
            customerDb = new CustomerDB(configuration);
            bookingDb = new BookingDB(configuration);
        }

        [HttpPost, AllowAnonymous, Route("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (login == null)
            {
                return BadRequest("Invalid client request");
            }

            var log = await CheckUserWithSoap.SoapEnvelope(login.Name, login.Password);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, login.Name)
            };
            Console.WriteLine(log);
            if (admins.Contains(log))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.key));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    "http://+:5000",
                    "http://+:4200",
                    claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new LoginResult(log, tokenString));
            }

            return BadRequest("Forkert brugernavn eller password");
        }
    }
}
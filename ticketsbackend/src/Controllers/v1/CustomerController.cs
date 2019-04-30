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
    public class CustomerController : Controller
    {
        private CustomerDB customerDb;

        public CustomerController(IConfiguration configuration)
        {
            customerDb = new CustomerDB(configuration);
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
                new Claim(ClaimTypes.Name, login.Name)
            };

            if (!log.Equals("Forkert brugernavn eller adgangskode!"))
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


        [HttpPut, Route("UpdateCustomer")]
        public IActionResult UpdateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return NotFound();
            }

            int temp = customerDb.UpdateCustomer(customer);
            if (temp == -1)
            {
                return NotFound();
            }


            return Ok();
        }

        [HttpGet("{id:int}"), AllowAnonymous]
        public IActionResult GetCustomer(int id)
        {
            Customer customer = customerDb.GetCustomer(id);

            return Ok(customer);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteCustomer(int id)
        {
            customerDb.DeleteCustomer(id);

            return Ok();
        }

        [HttpPost, Route("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Ingen kunde data indtastet");
            }

            customerDb.CreateCustomer(customer);


            return Ok();
        }
    }
}
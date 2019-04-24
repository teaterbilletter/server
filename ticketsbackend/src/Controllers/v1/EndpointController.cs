using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ticketsbackend.Models;
using ticketsbackend.SoapService;

namespace ticketsbackend.Controllers.v1
{
    [Authorize]
    [Route("/[controller]")]
    public class EndpointController : Controller
    {
        // GET api/values/5

        [HttpGet, Route("GetHej")]
        public IActionResult GetById()
        {
            return Ok(new {Response = "Hej med dig"});
        }

        // POST api/values
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


        // PUT api/values/5
        [HttpPut]
        public IActionResult Update([FromBody] string value)
        {
            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return Ok($"{id}");
        }
    }
}
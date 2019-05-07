using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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


        [HttpPut]
        public IActionResult UpdateCustomer([FromBody] Customer customer)
        {
            int temp = customerDb.UpdateCustomer(customer);
            if (temp == -1)
            {
                return NotFound($"Kunden med navn {customer.name} blev ikke fundet og kan derfor ikke opdateres.");
            }


            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(string id)
        {
            Customer customer = customerDb.GetCustomer(id);
            if (customer == null)
            {
                return NotFound($"Ingen kunde med id {id} fundet");
            }

            return Ok(customer);
        }


        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Ingen kunde data indtastet");
            }

            var newcust = customerDb.CreateCustomer(customer);
            if (newcust == -1)
            {
                return BadRequest("Der opstod en database fejl under indlæsning af kunde, tjek formattet");
            }


            return Ok();
        }
    }
}
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

        [HttpGet("{id}")]
        public IActionResult GetCustomer(string id)
        {
            Customer customer = customerDb.GetCustomer(id);

            return Ok(customer);
        }


        [HttpPost]
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
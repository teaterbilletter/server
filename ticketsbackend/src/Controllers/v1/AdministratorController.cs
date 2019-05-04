using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

namespace ticketsbackend.Controllers.v1
{
    [Authorize]
    [Route("/[controller]")]
    public class AdministratorController : Controller
    {
        private CustomerDB customerDb;
        private BookingDB bookingDb;
        private ShowDB showDb;
        private HallDB hallDb;
        private TheaterDB theaterDb;


        public AdministratorController(IConfiguration configuration)
        {
            customerDb = new CustomerDB(configuration);
            bookingDb = new BookingDB(configuration);
            showDb = new ShowDB(configuration);
            hallDb = new HallDB(configuration);
            theaterDb = new TheaterDB(configuration);
        }


        [HttpPost("~/Show")]
        public IActionResult CreateShow([FromBody] Show show)
        {
            showDb.createShow(show);

            return Ok();
        }

        [HttpPut("~/Show/{oldshowtitle}")]
        public IActionResult UpdateShow([FromBody] Show show, string oldshowtitle)
        {
            showDb.updateShow(show, oldshowtitle);

            return Ok();
        }

        [HttpDelete("~/ShowDates/{title}")]
        public IActionResult DeleteShowDates(string title)
        {
            showDb.deleteShowDates(title);

            return Ok();
        }

        [HttpDelete("~/Customer/{id:int}")]
        public IActionResult DeleteCustomer(int id)
        {
            customerDb.DeleteCustomer(id);

            return Ok();
        }

        [HttpPost("~/Theater")]
        public IActionResult CreateTheater([FromBody] Theater theater)
        {
            theaterDb.createTheater(theater);

            return Ok();
        }

        [HttpPut("~/Theater/{oldtheatername}")]
        public IActionResult UpdateTheater([FromBody] Theater theater, string oldtheatername)
        {
            theaterDb.updateTheater(theater, oldtheatername);

            return Ok();
        }
    }
}
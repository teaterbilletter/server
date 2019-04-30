using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ticketsbackend.Controllers.v1
{
    [Authorize]
    [Route("/[controller]")]
    public class BookingController : Controller
    {
        private BookingDB bookingDb;

        public BookingController(IConfiguration configuration)
        {
            bookingDb = new BookingDB(configuration);
        }


        [HttpGet("{id:int}"), AllowAnonymous]
        public IActionResult GetBooking(int id)
        {
            Booking booking = bookingDb.GetBooking(id);

            return Ok(booking);
        }


        [HttpDelete("{id:int}")]
        public IActionResult DeleteBooking(int id)
        {
            bookingDb.DeleteBooking(id);

            return Ok(new {Response = "Booking" + id + "Slettet"});
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] Booking booking)
        {
            bookingDb.CreateBooking(booking);

            return Ok();
        }

        [HttpGet("getbookings/{customerid:int}")]
        public IActionResult GetBookings(int customerid)
        {
            return Ok(bookingDb.GetCustomerBookings(customerid));
        }
    }
}
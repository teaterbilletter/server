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
        private readonly BookingDB bookingDb;

        public BookingController(IConfiguration configuration)
        {
            bookingDb = new BookingDB(configuration);
        }


        [HttpGet("{bookingid:int}")]
        public IActionResult GetBooking(int bookingid)
        {
            return Ok(bookingDb.GetBooking(bookingid));
        }


        [HttpDelete("{bookingid:int}")]
        public IActionResult DeleteBooking(int bookingid)
        {
            bookingDb.DeleteBooking(bookingid);

            return Ok();
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] Booking booking)
        {
            bookingDb.CreateBooking(booking);

            return Ok();
        }

        [HttpGet("~/Bookings/{customerid:int}")]
        public IActionResult GetBookings(int customerid)
        {
            return Ok(bookingDb.GetCustomerBookings(customerid));
        }
    }
}
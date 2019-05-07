using System.Linq;
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
            var booking = bookingDb.GetBooking(bookingid);
            if (booking == null) return NotFound($"Ingen booking med id {bookingid} fundet");

            return Ok(booking);
        }


        [HttpDelete("{bookingid:int}")]
        public IActionResult DeleteBooking(int bookingid)
        {
            if (bookingDb.DeleteBooking(bookingid) == -1) return NotFound($"Ingen booking med id {bookingid} fundet");

            return Ok();
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] Booking booking)
        {
            int bookingID = bookingDb.CreateBooking(booking);

            Booking createdBooking = bookingDb.GetBooking(bookingID);
            if (createdBooking == null) return NotFound("Der opstod en databasefejl");


            return Ok(createdBooking);
        }

        [HttpGet("~/Bookings/{customerid}")]
        public IActionResult GetBookings(string customerid)
        {
            var bookings = bookingDb.GetCustomerBookings(customerid);
            if (bookings == null || !bookings.Any()) return NotFound("Ingen bookings fundet for denne kunde");


            return Ok(bookings);
        }

        [HttpPost("~/Bookings")]
        public IActionResult MakeTempBooking([FromBody] Booking booking)
        {
            var tempb = bookingDb.MakeTempBooking(booking);
            if (tempb == null) return NotFound("Der opstod en databasefejl");

            return Ok(tempb);
        }
    }
}
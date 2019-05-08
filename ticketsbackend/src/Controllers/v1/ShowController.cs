using System;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ticketsbackend.Controllers.v1
{
    [Route("/[controller]")]
    public class ShowController : Controller
    {
        private readonly ShowDB showDb;
        private readonly TheaterDB theaterDb;

        public ShowController(IConfiguration configuration)
        {
            showDb = new ShowDB(configuration);
            theaterDb = new TheaterDB(configuration);
        }


        [HttpGet("~/AllShows")]
        public IActionResult GetAllShows()
        {
            var show = showDb.getAllShows();
            if (show == null)
            {
                return NotFound("Ingen shows fundet");
            }

            return Ok(show);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetShow(int id)
        {
            var cookop = new CookieOptions
            {
                Path = "/", HttpOnly = false, IsEssential = true, Expires = DateTimeOffset.Now.AddMonths(1)
            };
            var show = showDb.getShow(id);
            if (show == null)
            {
                return NotFound($"Showet med id {id} findes ikke");
            }

            Response.Cookies.Append("lastselectedshow", show.title, cookop);
            return Ok(show);
        }

        [HttpGet("~/Theater/{theaterName}")]
        public IActionResult GetTheater(string theaterName)
        {
            var theater = theaterDb.getTheater(theaterName);
            if (theater == null)
            {
                return NotFound($"Theater med navnet {theaterName} findes ikke");
            }

            return Ok(theater);
        }

        [HttpGet("~/GetOccupiedSeats/")]
        public IActionResult GetOccupiedSeats([FromQuery] string dateTime, [FromQuery] int ShowID)
        {
            DateTime date = DateTime.Parse(dateTime);
            var getseats = showDb.getOccupiedSeats(date, ShowID);
            if (getseats == null) return NotFound("Der opstod en fejl ved henting af sæder");
            return Ok(getseats);
        }
        
        [HttpGet("~/GetAvailableDates/")]
        public IActionResult GetAvailableDates([FromQuery] int ShowID, [FromQuery] int seatStart, [FromQuery] int seatEnd, [FromQuery] int rowNumber)
        {
            var dates = showDb.getAvailableDates(ShowID, seatStart, seatEnd, rowNumber);
            
            if (dates == null) return NotFound("Der opstod en fejl ved henting af sæder");
            
            return Ok(dates);
        }
    }
}
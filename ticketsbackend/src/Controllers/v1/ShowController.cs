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
            return Ok(showDb.getAllShows());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetShow(int id)
        {
            var cookop = new CookieOptions()
            {
                Path = "/", HttpOnly = false, IsEssential = true, Expires = DateTimeOffset.Now.AddMonths(1)
            };
            Show show = showDb.getShow(id);
            Response.Cookies.Append("lastselectedshow", show.title, cookop);
            return Ok(show);
        }

        [HttpGet("~/Theater/{theaterName}")]
        public IActionResult GetTheater(string theaterName)
        {
            return Ok(theaterDb.getTheater(theaterName));
        }

        [HttpGet("~/GetOccupiedSeats/")]
        public IActionResult GetOccupiedSeats([FromQuery] string dateTime, [FromQuery] int ShowID)
        {
            DateTime date = DateTime.Parse(dateTime);
            Console.WriteLine(date);
            return Ok(showDb.getOccupiedSeats(date, ShowID));
        }
    }
}
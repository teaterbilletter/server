using System;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

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
            return Ok(showDb.getShow(id));
        }

        [HttpGet("~/Theater/{theaterName}")]
        public IActionResult GetTheater(string theaterName)
        {
            return Ok(theaterDb.getTheater(theaterName));
        }

        public IActionResult GetOccupiedSeats(DateTime dateTime, int ShowID)
        {
            return Ok(showDb.getOccupiedSeats(dateTime, ShowID));
        }
    }
}
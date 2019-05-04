using Database.Models;
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

        [HttpGet("{title}")]
        public IActionResult GetShow(string title)
        {
            return Ok(showDb.getShow(title));
        }

        [HttpGet("~/Theater/{theaterName}")]
        public IActionResult GetTheater(string theaterName)
        {
            return Ok(theaterDb.getTheater(theaterName));
        }
    }
}
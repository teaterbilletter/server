using System;
using System.Net;
using System.Net.Http;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

namespace ticketsbackend.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [Route("/[controller]")]
    public class AdministratorController : Controller
    {
        private CustomerDB customerDb;

        private ShowDB showDb;
        private HallDB hallDb;
        private TheaterDB theaterDb;

        private HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Modellen er ikke gyldig"
        };

        public AdministratorController(IConfiguration configuration)
        {
            customerDb = new CustomerDB(configuration);
            showDb = new ShowDB(configuration);
            hallDb = new HallDB(configuration);
            theaterDb = new TheaterDB(configuration);
        }


        [HttpPost("~/Show")]
        public IActionResult CreateShow([FromBody] Show show)
        {
            var newshow = showDb.createShow(show);

            if (newshow == -1)
            {
                return BadRequest(resp);
            }

            return Ok(newshow);
        }

        [HttpPut("~/Show")]
        public IActionResult UpdateShow([FromBody] Show show)
        {
            var updateShow = showDb.updateShow(show);
            if (updateShow == -1)
            {
                return BadRequest(resp);
            }


            return Ok();
        }

        [HttpDelete("~/ShowDates")]
        public IActionResult DeleteShowDates(int id, DateTime dateTime)
        {
            if (showDb.deleteShowDate(id, dateTime) == -1)
            {
                return BadRequest($"Showet med id {id} findes ikke");
            }

            return Ok();
        }

        [HttpDelete("~/Customer/{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            if (customerDb.DeleteCustomer(id) == 0)
            {
                return BadRequest($"Der findes ingen kunde med id {id}");
            }

            return Ok();
        }

        [HttpPost("~/Theater")]
        public IActionResult CreateTheater([FromBody] Theater theater)
        {
            if (theaterDb.createTheater(theater) == -1)
            {
                return BadRequest(resp);
            }

            return Ok();
        }

        [HttpPut("~/Theater/{oldtheatername}")]
        public IActionResult UpdateTheater([FromBody] Theater theater, string oldtheatername)
        {
            if (theaterDb.updateTheater(theater, oldtheatername) == -1)
            {
                return BadRequest(resp);
            }

            return Ok();
        }

        [HttpPost("~/Hall")]
        public IActionResult CreateHall([FromBody] Hall hall)
        {
            if (hallDb.createHall(hall) == -1) return BadRequest(resp);

            return Ok();
        }
    }
}
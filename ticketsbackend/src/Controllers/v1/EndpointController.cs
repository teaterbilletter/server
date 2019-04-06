using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ticketsbackend.SoapService;

namespace ticketsbackend.Controllers.v1
{  
    [Route("api/v1/[controller]")]
    public class EndpointController : Controller
    {
       
        // GET api/values/5
        
        [HttpGet]
        public async Task<IActionResult> GetById(string name, string password)
        {
            var result = await CheckUserWithSoap.SoapEnvelope(name, password);

            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate([FromBody] string name, [FromBody] string password)
        {       Console.WriteLine(name +"pw"+password);
            var result = await CheckUserWithSoap.SoapEnvelope(name, password);


            return Ok(result);
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Update([FromBody] string value)
        {
            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return Ok($"{id}");
        }
    }
}
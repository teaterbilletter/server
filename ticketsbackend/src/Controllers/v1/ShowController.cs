using Database.Models;
using Microsoft.Extensions.Configuration;

namespace ticketsbackend.Controllers.v1
{
    public class ShowController
    {
        private ShowDB showDb;

        public ShowController(IConfiguration configuration)
        {
            showDb = new ShowDB(configuration);
        }
    }
}
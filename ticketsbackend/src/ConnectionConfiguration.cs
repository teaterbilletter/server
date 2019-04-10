using Microsoft.EntityFrameworkCore;
using ticketsbackend;

namespace Database.DatabaseConnector
{
    public class ConnectionConfiguration
    {
        public string ConnectionString { get; set; }
        public string DataProviderType { get; set; } 
    }
}
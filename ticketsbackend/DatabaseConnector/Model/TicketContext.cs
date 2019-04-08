using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Model
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options)
        {
        }
        
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Show> Shows { get; set; }
        
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Model
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Booking> Booking { get; set; }
        
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Model
{
    public class Booking
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int ShowID { get; set; }
        
        public Customer Customer { get; set; }
        public Show Show { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Model
{
    public class Show
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public List<DateTime> showDates { get; set; }
        public List<Customer> Customer;
    }
}
using System.Collections.Generic;
using ticketsbackend.Models;

namespace Database.Models
{
    public class Theater
    {
        public string name { get; set; }
        public string address { get; set; }
        public bool active { get; set; }
        

        public Theater()
        {
        }
    }
}
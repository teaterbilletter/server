using System.Collections.Generic;
using ticketsbackend.Models;

namespace Database.Models
{
    public class Theater
    {
        public string name { get; set; }
        public string address { get; set; }
        public bool active { get; set; }


        public Theater(string name, string address, bool active)
        {
            this.name = name;
            this.address = address;
            this.active = active;
        }

        public Theater()
        {
        }
    }
}
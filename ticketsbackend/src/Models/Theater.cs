using System.Collections.Generic;

namespace Database.Models
{
    public class Theater
    {
        public string name { get; set; }
        public string address { get; set; }

        public Theater(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        public Theater()
        {
        }
    }
}
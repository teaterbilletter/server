using System.Collections.Generic;

namespace Database.Models
{
    public class Theater
    {
        public string name { get; set; }
        public string address { get; set; }
        
        public List<Hall> halls { get; set; }

        public Theater(string name, string address, List<Hall> halls)
        {
            this.name = name;
            this.address = address;
            this.halls = halls;
        }

        public Theater()
        {
        }
    }
}
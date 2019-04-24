using System.Collections.Generic;

namespace Database.Models
{
    public class Hall
    {
        public int HallNumber { get; set; }
        public List<Seat> seats { get; set; }

        public Hall(int hallNumber, List<Seat> seats)
        {
            HallNumber = hallNumber;
            this.seats = seats;
        }

        public Hall()
        {
        }
    }
}
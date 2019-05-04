using Database.Models;

namespace ticketsbackend.Models
{
    public class Hall
    {
        public int hallNum { get; set; }
        public int[] seats { get; set; }
        
        public Theater theater { get; set; }

        public Hall(int hallNum, int[] seats, Theater theater)
        {
            this.hallNum = hallNum;
            this.seats = seats;
            this.theater = theater;
        }

        public Hall()
        {
        }
    }
}
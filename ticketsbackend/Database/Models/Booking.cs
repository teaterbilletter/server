using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Booking
    {
        public int customerID { get; set; }
        public List<Seat> seats { get; set; }
        public string Title { get; set; }
        public Show show { get; set; }
        public Theater theater { get; set; }
        public DateTime date { get; set; }

        public Booking()
        {
            
        }

        public Booking(int customerId, List<Seat> seats, string title, Show show, Theater theater, DateTime date)
        {
            customerID = customerId;
            this.seats = seats;
            Title = title;
            this.show = show;
            this.theater = theater;
            this.date = date;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Booking
    {
        public int bookingID { get; set; }
        public int customerID { get; set; }
        public List<Seat> seats { get; set; }
        public Show show { get; set; }
        public Theater theater { get; set; }
        public DateTime date { get; set; }

        public Booking()
        {
            
        }

        public Booking(int bookingId, int customerId, List<Seat> seats, Show show, Theater theater, DateTime date)
        {
            this.bookingID = bookingId;
            this.customerID = customerId;
            this.seats = seats;
            this.show = show;
            this.theater = theater;
            this.date = date;
        }
    }
}
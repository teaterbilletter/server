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
        public DateTime date { get; set; }
        public decimal totalPrice { get; set; }

        public Booking(int bookingId, int customerId, List<Seat> seats, Show show, DateTime date, decimal totalPrice)
        {
            bookingID = bookingId;
            customerID = customerId;
            this.seats = seats;
            this.show = show;
            this.date = date;
            this.totalPrice = totalPrice;
        }

        public Booking()
        {
            
        }
    }
}
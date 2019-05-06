using System;
using Database.Models;

namespace ticketsbackend.BusinessLogic
{
    public class PriceCalculation
    {

        public decimal calculatePrice(Booking booking, decimal basePrice)
        {
            decimal multiplication = 2;
            int amountOfSeats = booking.seats.Count;


            //Some days are more expensive than others
            switch (booking.date.DayOfWeek)
            {
                case DayOfWeek.Thursday:
                case DayOfWeek.Sunday:
                    multiplication += 0.3m;
                    break;
                case DayOfWeek.Friday:
                    multiplication += 0.4m;
                    break;
                case DayOfWeek.Saturday:
                    multiplication += 0.45m;
                    break;
            }

            //It is more expensive to sit on a higher row
            multiplication += booking.seats[0].row_number / 20.0m;

            
            //It is cheaper pr. ticket to buy more tickets! The discount has a limit at 10 tickets.
            if (amountOfSeats >= 10)
            {
                multiplication -= 1.0m;
            }
            else
            {
                multiplication -= amountOfSeats / 10.0m;
            }
            
            
            
            
            
            return basePrice * amountOfSeats  * multiplication;
        }
    }
}
namespace Database.Models
{
    public class Seat
    {
        
        public int seat_number { get; set; }
        public int row_number { get; set; }

        public Seat(int seatNumber, int rowNumber)
        {
            seat_number = seatNumber;
            row_number = rowNumber;
        }

        public Seat()
        {
        }
    }
}
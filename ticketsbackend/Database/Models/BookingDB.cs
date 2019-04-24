using System;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Database.Models
{
    public class BookingDB
    {
        /*
            GetBooking(BookingID)
            GetBookings(CustomerID)
            GetCustomer(CustomerID)
            CreateBooking(Booking)
            CreateCustomer(Customer)
         */
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public BookingDB(IConfiguration configuration)
        {
            
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }

        public DataSet test()
        {
            return dataAccessLayer.ExecuteDataSet("show tables;", CommandType.Text);   
        }

        public Booking GetBooking(int bookingID)
        {
            Booking b = new Booking();

            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "BookingID", bookingID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetBookingDetails", CommandType.StoredProcedure);

            b.Title = ds.Tables[0].Rows[0]["Title"].ToString();
            b.customerID = int.Parse(ds.Tables[0].Rows[0]["Customer_ID"].ToString().Trim());
            b.date = DateTime.Parse(ds.Tables[0].Rows[0]["BookedDate"].ToString());
            
            return b;
        }
        
        public Booking[] GetCustomerBookings(int customerID)
        {
            Booking[] bookings;

            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetCustomerBookings", CommandType.StoredProcedure);

            bookings = new Booking[ds.Tables[0].Rows.Count];

            for (int i = 0; i < bookings.Length; i++)
            {
                Booking booking = new Booking();
                booking.Title = ds.Tables[0].Rows[0]["Title"].ToString();
                booking.customerID = int.Parse(ds.Tables[0].Rows[0]["Customer_ID"].ToString().Trim());
                booking.date = DateTime.Parse(ds.Tables[0].Rows[0]["BookedDate"].ToString());
            }
            
            return bookings;
        }
        
        
        public int CreateBooking(Booking booking)
        {
            dataAccessLayer.CreateParameters(6);
            dataAccessLayer.AddParameters(0, "CustomerID", booking.customerID);
            dataAccessLayer.AddParameters(1, "ShowTitle", booking.show.title);
            dataAccessLayer.AddParameters(2, "Date", booking.date);
            dataAccessLayer.AddParameters(3, "TheaterName", booking.theater.name);
            dataAccessLayer.AddParameters(4, "SeatStart", booking.seats[0]);
            dataAccessLayer.AddParameters(5, "SeatEnd", booking.seats[booking.seats.Count-1]);
            int affectedRows = dataAccessLayer.ExecuteQuery("spCreateBooking", CommandType.StoredProcedure);
            
            return affectedRows;
        }
    }
}
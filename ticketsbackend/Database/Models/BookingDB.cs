using System;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Database.Models
{
    public class BookingDB
    {
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public BookingDB(IConfiguration configuration)
        {
            
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
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
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

namespace Database.Models
{
    public class BookingDB
    {
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public BookingDB(IConfiguration configuration)
        {
            
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }

        public DataSet test()
        {
            return dataAccessLayer.ExecuteDataSet("show tables;", CommandType.Text);   
        }

        /// <summary>
        /// Gets a specific booking based by booking ID
        /// </summary>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public Booking GetBooking(int bookingID)
        {
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "BookingID", bookingID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetBookingDetails", CommandType.StoredProcedure);

            Booking b = new Booking
            {
                bookingID = bookingID,
                customerID = int.Parse(ds.Tables[0].Rows[0]["Customer_ID"].ToString().Trim()),
                date = DateTime.Parse(ds.Tables[0].Rows[0]["BookedDate"].ToString()),
                show = new Show
                {
                    title = ds.Tables[0].Rows[0]["Title"].ToString(),
                    hall = new Hall
                    {
                        hallNum = int.Parse(ds.Tables[0].Rows[0]["Hall"].ToString().Trim()),
                        theater = new Theater
                        {
                            name = ds.Tables[0].Rows[0]["Theater"].ToString(),
                            address = ds.Tables[0].Rows[0]["Address"].ToString()
                        }
                    },
                    imgUrl = ds.Tables[0].Rows[0]["ImgUrl"].ToString(),
                    basePrice = decimal.Parse(ds.Tables[0].Rows[0]["BasePrice"].ToString())
                }
            };
            b.seats = new List<Seat>();
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                b.seats.Add(new Seat(int.Parse(row["SeatNumber"].ToString()), int.Parse(row["RowNumber"].ToString())));
            }
            
            return b;
        }
        
        /// <summary>
        /// Gets an overview of a customers bookings.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<Booking> GetCustomerBookings(int customerID)
        {
            List<Booking> bookings = new List<Booking>();

            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetCustomerBookings", CommandType.StoredProcedure);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                bookings.Add(new Booking
                    {
                        bookingID = int.Parse(row["ID"].ToString()),
                        date = DateTime.Parse(row["BookedDate"].ToString()),
                        show = new Show
                        {
                            title = row["Title"].ToString(),
                            hall = new Hall
                            {
                                hallNum = int.Parse(row["Hall"].ToString().Trim()),
                                theater = new Theater
                                {
                                    name = row["Name"].ToString(),
                                    address = row["Address"].ToString()
                                },
                                
                            },
                            imgUrl = row["ImgUrl"].ToString(),
                            basePrice = decimal.Parse(row["BasePrice"].ToString())
                        } 
                    }
                );
            }
            
            return bookings;
        }
        
        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public int CreateBooking(Booking booking)
        {
            dataAccessLayer.BeginTransaction();

            try
            {
                dataAccessLayer.CreateParameters(7);
                dataAccessLayer.AddParameters(0, "CustomerID", booking.customerID);
                dataAccessLayer.AddParameters(1, "ShowTitle", booking.show.title);
                dataAccessLayer.AddParameters(2, "BookingDate", booking.date);
                dataAccessLayer.AddParameters(3, "TheaterName", booking.show.hall.theater.name);
                dataAccessLayer.AddParameters(4, "SeatStart", booking.seats[0].seat_number);
                dataAccessLayer.AddParameters(5, "SeatEnd", booking.seats[booking.seats.Count-1].seat_number);
                dataAccessLayer.AddParameters(6, "RowNumber", booking.seats[0].row_number);
            
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateBooking", CommandType.StoredProcedure);
            
                dataAccessLayer.CommitTransaction();
                return affectedRows;
            }
            catch (Exception e)
            {
                dataAccessLayer.RollbackTransaction();
                Console.WriteLine(e);
                throw;
            }
           
        }
        
        /// <summary>
        /// Delete a specific booking
        /// </summary>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public int DeleteBooking(int bookingID)
        {
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "BookingID", bookingID);
            int affectedRows = dataAccessLayer.ExecuteQuery("spDeleteBooking", CommandType.StoredProcedure);
            
            return affectedRows;
        }
    }
}
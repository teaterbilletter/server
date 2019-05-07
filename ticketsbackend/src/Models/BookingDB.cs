using System;
using System.Collections.Generic;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;
using ticketsbackend.BusinessLogic;
using ticketsbackend.Models;

namespace Database.Models
{
    public class BookingDB
    {
        private readonly DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        private readonly PriceCalculation priceCalculation;
        private readonly ShowDB showDb;

        public BookingDB(IConfiguration configuration)
        {
            priceCalculation = new PriceCalculation();
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
            showDb = new ShowDB(configuration);
        }

        /// <summary>
        /// Gets a specific booking based by booking ID
        /// </summary>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public Booking GetBooking(int bookingID)
        {
            Booking b;
            try
            {
                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "BookingID", bookingID);
                DataSet ds = dataAccessLayer.ExecuteDataSet("spGetBookingDetails", CommandType.StoredProcedure);

                b = new Booking
                {
                    bookingID = bookingID,
                    customerID = ds.Tables[0].Rows[0]["Customer_ID"].ToString().Trim(),
                    date = DateTime.Parse(ds.Tables[0].Rows[0]["BookedDate"].ToString()),
                    show = new Show
                    {
                        ID = int.Parse(ds.Tables[0].Rows[0]["ShowID"].ToString()),
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
                    },
                    totalPrice = decimal.Parse(ds.Tables[0].Rows[0]["TotalPrice"].ToString())
                };
                b.seats = new List<Seat>();
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    b.seats.Add(new Seat(int.Parse(row["SeatNumber"].ToString()),
                        int.Parse(row["RowNumber"].ToString())));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


            return b;
        }

        /// <summary>
        /// Gets an overview of a customers bookings.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<Booking> GetCustomerBookings(string customerID)
        {
            List<Booking> bookings = new List<Booking>();
            try
            {
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
                            },
                            totalPrice = decimal.Parse(row["TotalPrice"].ToString()),
                            customerID = customerID
                        }
                    );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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
                decimal basePrice = showDb.getShow(booking.show.ID).basePrice;
                decimal totalBookingPrice = priceCalculation.calculatePrice(booking, basePrice);

                dataAccessLayer.CreateParameters(8);
                dataAccessLayer.AddParameters(0, "CustomerID", booking.customerID);
                dataAccessLayer.AddParameters(1, "ShowTitle", booking.show.title);
                dataAccessLayer.AddParameters(2, "BookingDate", booking.date);
                dataAccessLayer.AddParameters(3, "TheaterName", booking.show.hall.theater.name);
                dataAccessLayer.AddParameters(4, "SeatStart", booking.seats[0].seat_number);
                dataAccessLayer.AddParameters(5, "SeatEnd", booking.seats[booking.seats.Count - 1].seat_number);
                dataAccessLayer.AddParameters(6, "RowNumber", booking.seats[0].row_number);
                dataAccessLayer.AddParameters(7, "TotalPrice", totalBookingPrice);

                var customerID = dataAccessLayer.ExecuteScalar("spCreateBooking", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();
                return int.Parse(customerID.ToString().Trim());
            }
            catch (Exception e)
            {
                dataAccessLayer.RollbackTransaction();
                Console.WriteLine(e);
                return -1;
            }
        }

        /// <summary>
        /// Delete a specific booking
        /// </summary>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public int DeleteBooking(int bookingID)
        {
            try
            {
                dataAccessLayer.BeginTransaction();

                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "BookingID", bookingID);
                int affectedRows = dataAccessLayer.ExecuteQuery("spDeleteBooking", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();
                return affectedRows;
            }
            catch (Exception e)
            {
                dataAccessLayer.RollbackTransaction();
                Console.WriteLine(e);
                return -1;
            }
        }


        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public Booking MakeTempBooking(Booking booking)
        {
            try
            {
                decimal basePrice = showDb.getShow(booking.show.ID).basePrice;
                decimal totalBookingPrice = priceCalculation.calculatePrice(booking, basePrice);

                booking.totalPrice = totalBookingPrice;

                return booking;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
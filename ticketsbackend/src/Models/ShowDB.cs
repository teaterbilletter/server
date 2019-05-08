using System;
using System.Collections.Generic;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

namespace Database.Models
{
    public class ShowDB
    {
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;

        public ShowDB(IConfiguration configuration)
        {
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }

        public int createShow(Show show)
        {
            try
            {
                dataAccessLayer.BeginTransaction();

                dataAccessLayer.CreateParameters(6);
                dataAccessLayer.AddParameters(0, "Title", show.title);
                dataAccessLayer.AddParameters(1, "ImgUrl", show.imgUrl);
                dataAccessLayer.AddParameters(2, "Hall_ID", show.hall.hallNum);


                List<string> date_strings = new List<string>();
                show.dates.ForEach(d => date_strings.Add(d.ToString("yyyy-MM-dd hh:mm:ss")));
                string dates = string.Join(",", date_strings);

                dataAccessLayer.AddParameters(3, "Dates", dates);
                dataAccessLayer.AddParameters(4, "basePrice", show.basePrice);
                dataAccessLayer.AddParameters(5, "description", show.description);
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateShow", CommandType.StoredProcedure);


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

        public int deleteShowDates(int id)
        {
            int temp;
            try
            {
                temp = dataAccessLayer.ExecuteQuery("spDeleteShowDates", CommandType.StoredProcedure);
                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "ShowID", id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return temp;
        }

        public int updateShow(Show show)
        {
            int affectedRows;
            try
            {
                dataAccessLayer.BeginTransaction();
                dataAccessLayer.CreateParameters(8);
                dataAccessLayer.AddParameters(0, "ShowID", show.ID);
                dataAccessLayer.AddParameters(1, "Title", show.title);
                dataAccessLayer.AddParameters(2, "ImgUrl", show.imgUrl);
                dataAccessLayer.AddParameters(3, "Hall_ID", show.hall.hallNum);

                List<string> date_strings = new List<string>();
                show.dates.ForEach(d => date_strings.Add(d.ToString("yyyy-MM-dd HH:mm:ss")));
                string dates = string.Join(",", date_strings);

                dataAccessLayer.AddParameters(4, "Dates", dates);
                dataAccessLayer.AddParameters(5, "Active", show.active);
                dataAccessLayer.AddParameters(6, "basePrice", show.basePrice);
                dataAccessLayer.AddParameters(7, "Description", show.description);
                affectedRows = dataAccessLayer.ExecuteQuery("spUpdateShow", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return affectedRows;
        }

        public Show getShow(int id)
        {
            try
            {
                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "ShowID", id);
                DataSet ds = dataAccessLayer.ExecuteDataSet("spGetShow", CommandType.StoredProcedure);

                Show show = new Show
                {
                    ID = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString()),
                    title = ds.Tables[0].Rows[0]["Title"].ToString(),
                    imgUrl = ds.Tables[0].Rows[0]["ImgUrl"].ToString(),
                    hall = new Hall
                    {
                        hallNum = int.Parse(ds.Tables[0].Rows[0]["Hall_ID"].ToString()),
                        theater = new Theater
                        {
                            address = ds.Tables[0].Rows[0]["Address"].ToString(),
                            name = ds.Tables[0].Rows[0]["Theater"].ToString()
                        },
                        rows = int.Parse(ds.Tables[2].Rows[0]["RowCount"].ToString()),
                        seats = int.Parse(ds.Tables[2].Rows[0]["SeatCount"].ToString())
                    },
                    basePrice = decimal.Parse(ds.Tables[0].Rows[0]["BasePrice"].ToString()),
                    description = ds.Tables[0].Rows[0]["Description"].ToString()
                };

                show.dates = new List<DateTime>();
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    show.dates.Add(DateTime.Parse(dr["ShowDate"].ToString()));
                }

                return show;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<Show> getAllShows()
        {
            List<Show> shows = new List<Show>();
            try
            {
                DataTable dt = dataAccessLayer.ExecuteDataSet("spGetAllShows", CommandType.StoredProcedure).Tables[0];


                foreach (DataRow dr in dt.Rows)
                {
                    shows.Add(new Show
                    {
                        ID = int.Parse(dr["ID"].ToString()),
                        title = dr["Title"].ToString(),
                        imgUrl = dr["ImgUrl"].ToString(),
                        description = dr["Description"].ToString(),
                        hall = new Hall
                        {
                            //var Hall_ID
                            hallNum = int.Parse(dr["ID"].ToString()),
                            theater = new Theater
                            {
                                address = dr["Address"].ToString(),
                                name = dr["Theater"].ToString()
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


            return shows;
        }

        public List<Seat> getOccupiedSeats(DateTime dateTime, int showID)
        {
            List<Seat> seats = new List<Seat>();
            try
            {
                dataAccessLayer.CreateParameters(2);
                dataAccessLayer.AddParameters(0, "ShowDate", dateTime);
                dataAccessLayer.AddParameters(1, "ShowID", showID);
                DataSet ds = dataAccessLayer.ExecuteDataSet("spGetBookedSeats", CommandType.StoredProcedure);


                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    seats.Add(new Seat
                    {
                        row_number = int.Parse(dataRow["RowNumber"].ToString()),
                        seat_number = int.Parse(dataRow["SeatNumber"].ToString())
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


            return seats;
        }
        
        public List<DateTime> getAvailableDates(int showID, int seatStart, int seatEnd, int rowNumber)
        {
            List<DateTime> dates = new List<DateTime>();
            try
            {
                dataAccessLayer.CreateParameters(4);
                dataAccessLayer.AddParameters(0, "ShowID", showID);
                dataAccessLayer.AddParameters(1, "RowNumber", rowNumber);
                dataAccessLayer.AddParameters(2, "SeatStart", seatStart);
                dataAccessLayer.AddParameters(3, "seatEnd", seatEnd);
                DataSet ds = dataAccessLayer.ExecuteDataSet("spGetDatesBySeats", CommandType.StoredProcedure);


                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    dates.Add(DateTime.Parse(dataRow.ToString()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


            return dates;
        }
    }
}
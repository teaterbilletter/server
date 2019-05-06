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

                dataAccessLayer.CreateParameters(5);
                dataAccessLayer.AddParameters(0, "Title", show.title);
                dataAccessLayer.AddParameters(1, "ImgUrl", show.imgUrl);
                dataAccessLayer.AddParameters(2, "Hall_ID", show.hall.hallNum);


                List<string> date_strings = new List<string>();
                show.dates.ForEach(d => date_strings.Add(d.ToString("yyyy-MM-dd hh:mm:ss")));
                string dates = string.Join(",", date_strings);

                dataAccessLayer.AddParameters(3, "Dates", dates);
                dataAccessLayer.AddParameters(4, "basePrice", show.basePrice);
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateShow", CommandType.StoredProcedure);


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

        public int deleteShowDates(int id)
        {
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "ShowID", id);
            return dataAccessLayer.ExecuteQuery("spDeleteShowDates", CommandType.StoredProcedure);
        }

        public int updateShow(Show show)
        {
            try
            {
                dataAccessLayer.BeginTransaction();
                dataAccessLayer.CreateParameters(7);
                dataAccessLayer.AddParameters(0, "ShowID", show.ID);
                dataAccessLayer.AddParameters(1, "Title", show.title);
                dataAccessLayer.AddParameters(2, "ImgUrl", show.imgUrl);
                dataAccessLayer.AddParameters(3, "Hall_ID", show.hall.hallNum);

                List<string> date_strings = new List<string>();
                show.dates.ForEach(d => date_strings.Add(d.ToString("yyyy-MM-dd hh:mm:ss")));
                string dates = string.Join(",", date_strings);

                dataAccessLayer.AddParameters(4, "Dates", dates);
                dataAccessLayer.AddParameters(5, "Active", show.active);
                dataAccessLayer.AddParameters(6, "basePrice", show.basePrice);
                int affectedRows = dataAccessLayer.ExecuteQuery("spUpdateShow", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();

                return affectedRows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Show getShow(int id)
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
                desciption = ds.Tables[0].Rows[0]["Description"].ToString()
            };

            show.dates = new List<DateTime>();
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                show.dates.Add(DateTime.Parse(dr["ShowDate"].ToString()));
            }

            return show;
        }

        public List<Show> getAllShows()
        {
            DataTable dt = dataAccessLayer.ExecuteDataSet("spGetAllShows", CommandType.StoredProcedure).Tables[0];
            List<Show> shows = new List<Show>();

            foreach (DataRow dr in dt.Rows)
            {
                shows.Add(new Show
                {
                    ID = int.Parse(dr["ID"].ToString()),
                    title = dr["Title"].ToString(),
                    imgUrl = dr["ImgUrl"].ToString(),
                    desciption = dr["Description"].ToString(),
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

            return shows;
        }

        public List<Seat> getOccupiedSeats(DateTime dateTime, int ShowID)
        {
            dataAccessLayer.CreateParameters(2);
            dataAccessLayer.AddParameters(0, "ShowDate", dateTime);
            dataAccessLayer.AddParameters(1, "ShowID", ShowID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetBookedSeats", CommandType.StoredProcedure);


            List<Seat> seats = new List<Seat>();

            foreach (DataRow dataRow in ds.Tables[0].Rows)
            {
                seats.Add(new Seat
                {
                    row_number = int.Parse(dataRow["RowNumber"].ToString()),
                    seat_number = int.Parse(dataRow["SeatNumber"].ToString())
                });
            }


            return seats;
        }
    }
}
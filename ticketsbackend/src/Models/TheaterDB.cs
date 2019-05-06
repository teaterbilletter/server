using System;
using System.Data;
using System.Xml;
using Database.DatabaseConnector;
using MessagePack.Resolvers;
using Microsoft.Extensions.Configuration;
using ticketsbackend.Models;

namespace Database.Models
{
    public class TheaterDB
    {
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public TheaterDB(IConfiguration configuration)
        {
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }
        public int createTheater(Theater theater)
        {
            try
            {
                dataAccessLayer.BeginTransaction();
                
                dataAccessLayer.CreateParameters(3);
                dataAccessLayer.AddParameters(0, "Theater", theater.name);
                dataAccessLayer.AddParameters(1, "Address", theater.address);
                dataAccessLayer.AddParameters(2, "Active", theater.active);
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateTheater", CommandType.StoredProcedure);

                
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

        public int updateTheater(Theater newTheater, string oldTheaterName)
        {             
            dataAccessLayer.CreateParameters(4);
            dataAccessLayer.AddParameters(0, "Theater", newTheater.name);
            dataAccessLayer.AddParameters(1, "Address", newTheater.address);
            dataAccessLayer.AddParameters(2, "Active", newTheater.active);
            dataAccessLayer.AddParameters(3, "OldName", oldTheaterName);
            return dataAccessLayer.ExecuteQuery("spUpdateTheater", CommandType.StoredProcedure);
        }

        public Theater getTheater(string theaterName)
        {
                       
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "Theater", theaterName);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetTheater", CommandType.StoredProcedure);

            Theater theater = new Theater
            {
                name = ds.Tables[0].Rows[0]["Theater"].ToString(),
                address = ds.Tables[0].Rows[0]["Address"].ToString(),
                active = ds.Tables[0].Rows[0]["Active"].ToString() == "1"
            };
           
            return theater;
        }
    }
}
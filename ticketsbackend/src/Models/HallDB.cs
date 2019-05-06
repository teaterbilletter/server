using System;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;

namespace ticketsbackend.Models
{
    public class HallDB
    {
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public HallDB(IConfiguration configuration)
        {
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }
        public int createHall(Hall hall)
        {
            
            try
            {
                dataAccessLayer.BeginTransaction();
                
                dataAccessLayer.CreateParameters(4);
                dataAccessLayer.AddParameters(0, "HallNum", hall.hallNum);
                dataAccessLayer.AddParameters(1, "Theater", hall.theater.name);
                dataAccessLayer.AddParameters(2, "SeatCount", hall.seats);
                dataAccessLayer.AddParameters(3, "RowCount", hall.rows);
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateHall", CommandType.StoredProcedure);

                
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
    }
}
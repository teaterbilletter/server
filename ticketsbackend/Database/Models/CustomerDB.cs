using System;
using System.Data;
using Database.DatabaseConnector;
using Microsoft.Extensions.Configuration;

namespace Database.Models
{
    public class CustomerDB
    {        
        private DataAccessLayer.DataAccessLayerBaseClass dataAccessLayer;
        public CustomerDB(IConfiguration configuration)
        { 
            dataAccessLayer = DataAccessLayer.DataAccessLayerFactory.GetDataAccessLayer(configuration);
        }

        public Customer GetCustomer(int customerID)
        {
            Customer customer = new Customer();

            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetCustomer", CommandType.StoredProcedure);

            customer.name = ds.Tables[0].Rows[0]["Name"].ToString();
            customer.email = ds.Tables[0].Rows[0]["Email"].ToString().Trim();
            customer.ID = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString().Trim());
            
            return customer;
        }
        
        public int CreateCustomer(Customer customer)
        {
            
            dataAccessLayer.CreateParameters(3);
            dataAccessLayer.AddParameters(0, "Name", customer.name);
            dataAccessLayer.AddParameters(1, "Email", customer.email);
            dataAccessLayer.AddParameters(2, "ID", customer.ID);
            int affectedRows = dataAccessLayer.ExecuteQuery("spCreateCustomer", CommandType.StoredProcedure);
            
            return affectedRows;
        }
        
        public int UpdateCustomer(Customer customer)
        {
            dataAccessLayer.CreateParameters(3);
            dataAccessLayer.AddParameters(0, "Name", customer.name);
            dataAccessLayer.AddParameters(1, "Email", customer.email);
            dataAccessLayer.AddParameters(2, "ID", customer.ID);
            int affectedRows = dataAccessLayer.ExecuteQuery("spUpdateCustomer", CommandType.StoredProcedure);
            
            return affectedRows;
        }
        
        public int DeleteCustomer(int customerID)
        {
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            int affectedRows = dataAccessLayer.ExecuteQuery("spDeleteCustomer", CommandType.StoredProcedure);
            
            return affectedRows;
        }
    }
}
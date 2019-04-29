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

        /// <summary>
        /// Gets a customer from the database
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public Customer GetCustomer(int customerID)
        {
            

            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            DataSet ds = dataAccessLayer.ExecuteDataSet("spGetCustomer", CommandType.StoredProcedure);

            
            Customer customer = new Customer
            {
                ID = customerID,
                name = ds.Tables[0].Rows[0]["Name"].ToString(),
                email = ds.Tables[0].Rows[0]["Mail"].ToString().Trim(),
                phone = ds.Tables[0].Rows[0]["Phone"]?.ToString(),
                Address = ds.Tables[0].Rows[0]["Address"]?.ToString(),
                Gender = ds.Tables[0].Rows[0]["Gender"]?.ToString(),
                Age = int.Parse(ds.Tables[0].Rows[0]["Name"]?.ToString().Trim())
            };
            
            return customer;
        }
        
        /// <summary>
        /// Creates a customer in the database
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int CreateCustomer(Customer customer)
        {
            dataAccessLayer.BeginTransaction();
            try
            {
                dataAccessLayer.CreateParameters(6);
                dataAccessLayer.AddParameters(0, "Name", customer.name);
                dataAccessLayer.AddParameters(1, "Email", customer.email);
                dataAccessLayer.AddParameters(2, "Phone", customer.ID);
                dataAccessLayer.AddParameters(3, "Address", customer.ID);
                dataAccessLayer.AddParameters(4, "Gender", customer.ID);
                dataAccessLayer.AddParameters(5, "Name", customer.ID);
                
                int affectedRows = dataAccessLayer.ExecuteQuery("spCreateCustomer", CommandType.StoredProcedure);

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
        /// Updates information for a customer in the database
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int UpdateCustomer(Customer customer)
        {
            try
            {
                dataAccessLayer.CreateParameters(7);
                dataAccessLayer.AddParameters(0, "ID", customer.ID);
                dataAccessLayer.AddParameters(1, "Name", customer.name);
                dataAccessLayer.AddParameters(2, "Email", customer.email);
                dataAccessLayer.AddParameters(3, "ID", customer.phone);
                dataAccessLayer.AddParameters(4, "ID", customer.Address);
                dataAccessLayer.AddParameters(5, "ID", customer.Gender);
                dataAccessLayer.AddParameters(6, "ID", customer.Age);
                return dataAccessLayer.ExecuteQuery("spUpdateCustomer", CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        
        /// <summary>
        /// Deletes a customer from the database
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int DeleteCustomer(int customerID)
        {
            dataAccessLayer.CreateParameters(1);
            dataAccessLayer.AddParameters(0, "CustomerID", customerID);
            return dataAccessLayer.ExecuteQuery("spDeleteCustomer", CommandType.StoredProcedure);
        }
    }
}
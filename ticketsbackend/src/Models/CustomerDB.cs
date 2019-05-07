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
        public Customer GetCustomer(string customerID)
        {
            Customer customer;
            try
            {
                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "CustomerID", customerID);
                DataSet ds = dataAccessLayer.ExecuteDataSet("spGetCustomer", CommandType.StoredProcedure);


                customer = new Customer
                {
                    ID = customerID,
                    name = ds.Tables[0].Rows[0]["CustomerName"].ToString(),
                    email = ds.Tables[0].Rows[0]["Mail"].ToString().Trim(),
                    phone = ds.Tables[0].Rows[0]["Phone"]?.ToString(),
                    Address = ds.Tables[0].Rows[0]["Address"]?.ToString(),
                    Gender = ds.Tables[0].Rows[0]["Gender"]?.ToString(),
                    Age = int.Parse(ds.Tables[0].Rows[0]["Age"]?.ToString().Trim())
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


            return customer;
        }

        /// <summary>
        /// Creates a customer in the database
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int CreateCustomer(Customer customer)
        {
            int affectedRows;
            dataAccessLayer.BeginTransaction();
            try
            {
                dataAccessLayer.CreateParameters(7);
                dataAccessLayer.AddParameters(0, "ID", customer.ID);
                dataAccessLayer.AddParameters(1, "CustomerName", customer.name);
                dataAccessLayer.AddParameters(2, "Email", customer.email);
                dataAccessLayer.AddParameters(3, "Phone", customer.phone);
                dataAccessLayer.AddParameters(4, "Address", customer.Address);
                dataAccessLayer.AddParameters(5, "Gender", customer.Gender);
                dataAccessLayer.AddParameters(6, "Age", customer.Age);

                affectedRows = dataAccessLayer.ExecuteQuery("spCreateCustomer", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();
            }
            catch (Exception e)
            {
                dataAccessLayer.RollbackTransaction();
                Console.WriteLine(e);
                return -1;
            }

            return affectedRows;
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
                dataAccessLayer.BeginTransaction();
                dataAccessLayer.CreateParameters(7);
                dataAccessLayer.AddParameters(0, "CustomerID", customer.ID);
                dataAccessLayer.AddParameters(1, "CustomerName", customer.name);
                dataAccessLayer.AddParameters(2, "Email", customer.email);
                dataAccessLayer.AddParameters(3, "Phone", customer.phone);
                dataAccessLayer.AddParameters(4, "Address", customer.Address);
                dataAccessLayer.AddParameters(5, "Gender", customer.Gender);
                dataAccessLayer.AddParameters(6, "Age", customer.Age);
                int affectedRows = dataAccessLayer.ExecuteQuery("spUpdateCustomer", CommandType.StoredProcedure);

                dataAccessLayer.CommitTransaction();
                return affectedRows;
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
        public int DeleteCustomer(string customerID)
        {
            int temp;
            try
            {
                dataAccessLayer.CreateParameters(1);
                dataAccessLayer.AddParameters(0, "CustomerID", customerID);
                temp = dataAccessLayer.ExecuteQuery("spDeleteCustomer", CommandType.StoredProcedure);
                Console.WriteLine(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }

            return temp;
        }
    }
}
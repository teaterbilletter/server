using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// This class is created with inspiration from work and from 
// http://aspalliance.com/articleViewer.aspx?aId=837&pId=-1
namespace Database.DatabaseConnector
{
    public class DataAccessLayer
    {
        public static string ConnectionString;
        public static string DataProviderTypeString;

        /// <summary>
        /// Contains data provider types. Add new provider types here.
        /// </summary>
        public enum DataProviderType
        {
            Mysql
        }

        public abstract class DataAccessLayerBaseClass
        {
            private string strConnectionString;

            private IDataParameter[] parameters = null;
            private IDbCommand command;
            private IDbDataAdapter dataAdapter;
            private IDbTransaction transaction;
            private IDbConnection connection;


            public int CommandTimeout = 15; //Seconds

            [Description("Connection string")]
            public string ConnectionString
            {
                get
                {
                    if (String.IsNullOrEmpty(strConnectionString))
                        throw new ArgumentException("Connection string is not valid");

                    return strConnectionString;
                }
                set => strConnectionString = value;
            }


            #region Parameters

            public static IDataParameter[] GetParameters(DataProviderType dataProviderType, int paramsCount)
            {
                IDataParameter[] idbParams = new IDataParameter[paramsCount];

                switch (dataProviderType)
                {
                    case DataProviderType.Mysql:
                        for (int i = 0; i < paramsCount; ++i)
                        {
                            idbParams[i] = new MySqlParameter();
                        }

                        break;
                    default:
                        idbParams = null;
                        break;
                }

                return idbParams;
            }

            [Description("Creates parameters")]
            public void CreateParameters(int count)
            {
                DataProviderType dataProvider = (DataProviderType) Enum.Parse(typeof(DataProviderType),
                    DataProviderTypeString, true);
                parameters = new IDataParameter[count];
                parameters = GetParameters(dataProvider, count);
            }

            [Description("Adds parameter")]
            public void AddParameters(int index, string paramName, object objValue)
            {
                if (index < parameters.Length)
                {
                    parameters[index] = new MySqlParameter(paramName, objValue == null ? DBNull.Value : objValue);
                }
            }

            #endregion


            /// <summary>
            /// Prepares a command to access database.
            /// </summary>
            private void PrepareCommand(CommandType commandType, string commandText)
            {
                try
                {
                    // provide the specific data provider connection object, if the connection object is null
                    if (connection == null)
                    {
                        connection = GetDataProviderConnection();
                        connection.ConnectionString = ConnectionString;
                    }

                    // if the provided connection is not open, then open it
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    // Provide the specific data provider command object, if the command object is null
                    if (command == null)
                        command = GetDataProviderCommand();

                    // associate the connection with the command
                    command.Connection = connection;
                    // set the command text (stored procedure name or SQL statement)
                    command.CommandText = commandText;
                    // set the command type
                    command.CommandType = commandType;
                    // set time timeout of the command
                    command.CommandTimeout = CommandTimeout;

                    // if a transaction is provided, then assign it.
                    if (transaction != null)
                        command.Transaction = transaction;

                    //Attach the parameters if any provided
                    if (parameters != null)
                    {
                        foreach (IDataParameter param in parameters)
                            command.Parameters.Add(param);
                    }
                }
                catch (DbException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            #region Abstract Methods

            internal abstract IDbConnection GetDataProviderConnection();

            internal abstract IDbCommand GetDataProviderCommand();

            internal abstract IDbDataAdapter GetDataProviderDataAdapter();

            #endregion

            #region Generic methods

            #region Database Transaction

            [Description("Starts a transaction")]
            public void BeginTransaction()
            {
                if (transaction != null)
                    return;

                try
                {
                    // instantiate a connection object
                    connection = GetDataProviderConnection();
                    connection.ConnectionString = ConnectionString;
                    // open connection
                    connection.Open();
                    // begin a database transaction with a read committed isolation level
                    transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
                }
                catch
                {
                    connection.Close();

                    throw;
                }
            }

            [Description("Commits a transaction")]
            public void CommitTransaction()
            {
                if (transaction == null)
                    return;

                try
                {
                    // Commit transaction
                    transaction.Commit();
                }
                catch
                {
                    // rollback transaction
                    RollbackTransaction();
                    throw;
                }
                finally
                {
                    connection.Close();
                    transaction = null;
                }
            }


            [Description("Rolls back a transaction")]
            public void RollbackTransaction()
            {
                if (transaction == null)
                    return;

                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }
                finally
                {
                    connection.Close();
                    transaction = null;
                }
            }

            #endregion

            #region ExecuteDataReader

            [Description("Execute and return IDataRead")]
            public IDataReader ExecuteDataReader(string commandText, CommandType commandType)
            {
                try
                {
                    PrepareCommand(commandType, commandText);

                    IDataReader dr;

                    if (transaction == null)
                        // Generate the reader. CommandBehavior.CloseConnection causes the
                        // the connection to be closed when the reader object is closed
                        dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                    else
                        dr = command.ExecuteReader();

                    return dr;
                }
                catch (DbException ex)
                {
                    if (transaction == null)
                    {
                        connection.Close();
                        command.Dispose();
                    }
                    else
                    {
                        RollbackTransaction();
                    }

                    throw ex;
                }
                finally
                {
                    if (parameters != null)
                        parameters = null;
                }
            }

            #endregion

            #region ExecuteDataSet

            [Description("Execute query and returns dataset")]
            public DataSet ExecuteDataSet(string commandText, CommandType commandType)
            {
                try
                {
                    PrepareCommand(commandType, commandText);
                    //create the DataAdapter & DataSet
                    dataAdapter = GetDataProviderDataAdapter();
                    dataAdapter.SelectCommand = command;
                    DataSet ds = new DataSet();

                    //fill the DataSet using default values for DataTable names, etc.
                    dataAdapter.Fill(ds);

                    //return the dataset
                    return ds;
                }
                catch (DbException ex)
                {
                    if (transaction == null)
                        connection.Close();
                    else
                    {
                        RollbackTransaction();
                    }

                    throw ex;
                }
                finally
                {
                    if (transaction == null)
                        connection.Close();

                    if (parameters != null)
                        parameters = null;

                    if (command != null)
                        if (command.Parameters != null && command.Parameters.Count > 0)
                            command.Parameters.Clear();
                }
            }

            #endregion

            #region ExecuteQuery

            [Description("Execute a query and returns the number of rows affected")]
            public int ExecuteQuery(string commandText, CommandType commandType)
            {
                try
                {
                    PrepareCommand(commandType, commandText);

                    // execute command
                    int intAffectedRows = command.ExecuteNonQuery();
                    // return no of affected records
                    return intAffectedRows;
                }
                catch (DbException ex)
                {
                    if (transaction != null)
                    {
                        RollbackTransaction();
                    }

                    throw ex;
                }
                finally
                {
                    if (transaction == null)
                    {
                        connection.Close();
                        command.Dispose();
                    }

                    if (parameters != null)
                        parameters = null;

                    if (command != null)
                        if (command.Parameters != null && command.Parameters.Count > 0)
                            command.Parameters.Clear();
                }
            }

            #endregion

            #region ExecuteScalar

            
            [Description("Executes a stored procedure, and returns the first column of the first row in the resultset returned by the query")]
            public object ExecuteScalar(string commandText, CommandType commandType)
            {
                try
                {
                    PrepareCommand(commandType, commandText);

                    // execute command
                    object objValue = command.ExecuteScalar();
                    // check on value
                    if (objValue != DBNull.Value)
                        // return value
                        return objValue;
                    else
                        // return null instead of dbnull value
                        return null;
                }
                catch (DbException ex)
                {
                    if (transaction != null)
                        RollbackTransaction();
                    throw ex;
                }
                finally
                {
                    if (transaction == null)
                    {
                        connection.Close();
                        command.Dispose();
                    }

                    if (parameters != null)
                        parameters = null;

                    if (command != null)
                        if (command.Parameters != null && command.Parameters.Count > 0)
                            command.Parameters.Clear();
                }
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// Create the data acces layer defined in appsettings
        /// </summary>
        public sealed class DataAccessLayerFactory
        {
            private DataAccessLayerFactory()
            {
            }

            public static DataAccessLayerBaseClass GetDataAccessLayer(IConfiguration configuration)
            {
                ConnectionString = configuration.GetSection("ConnectionStrings")["ConnectionString"];
                DataProviderTypeString = configuration.GetSection("ConnectionStrings")["DataProviderType"];
                // Make sure application configuration file contains required configuration keys
                if (DataProviderTypeString == null || ConnectionString == null)
                    throw new ArgumentNullException(
                        "Please specify a 'DataProviderType' and 'ConnectionString' configuration keys in appsettings.json.");

                DataProviderType dataProvider;

                try
                {
                    // try to parse the data provider type from configuration file
                    dataProvider =
                        (DataProviderType) Enum.Parse(typeof(DataProviderType),
                            DataProviderTypeString,
                            true);
                }
                catch
                {
                    throw new ArgumentException("Invalid data access layer provider type.");
                }

                // return data access layer provider
                return GetDataAccessLayer(
                    dataProvider,
                    ConnectionString);
            }
            
            public static DataAccessLayerBaseClass GetDataAccessLayer(DataProviderType dataProviderType,
                string connectionString)
            {
                // construct specific data access provider class
                switch (dataProviderType)
                {
                    case DataProviderType.Mysql:
                        return new MySqlDataAccessLayer(connectionString);

                    default:
                        throw new ArgumentException("Invalid data access layer provider type.");
                }
            }
        }
    }
}
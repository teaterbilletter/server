using System;
using System.ComponentModel;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Database.DatabaseConnector
{
    public class DataAccessLayer
    {
        public static string ConnectionString;
        public static string DataProviderTypeString;

        public enum DataProviderType
        {
            Mysql
        }

        public abstract class DataAccessLayerBaseClass
        {
            protected DataAccessLayerBaseClass()
            {
            }


            private string strConnectionString;

            private IDataParameter[] parameters = null;
            private IDbCommand command;
            private IDbDataAdapter dataAdapter;
            private IDbTransaction transaction;
            private IDbConnection connection;


            public int CommandTimeout = 15; //Seconds

            [Description("Property for the connection string")]
            public string ConnectionString
            {
                get
                {
                    if (String.IsNullOrEmpty(strConnectionString))
                        throw new ArgumentException("Connection string is not valid");

                    return strConnectionString;
                }
                set { strConnectionString = value; }
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

            [Description("Creates parameters to pass to a procedure")]
            public void CreateParameters(int count)
            {
                DataProviderType dataProvider = (DataProviderType) Enum.Parse(typeof(DataProviderType),
                    DataProviderTypeString, true);
                parameters = new IDataParameter[count];
                parameters = GetParameters(dataProvider, count);
            }

            [Description("Adds parameter to pass to a procedure")]
            public void AddParameters(int index, string paramName, object objValue)
            {
                if (index < parameters.Length)
                {
                    parameters[index] = new MySqlParameter(paramName, objValue == null ? DBNull.Value : objValue);
                    //idbParameters[index].ParameterName = paramName;
                    //idbParameters[index].Value = objValue == null ? DBNull.Value : objValue;
                }
            }

            [Description("Adds parameter to pass to a procedure")]
            public void AddParameters(int index, MySqlParameter parameter)
            {
                if (index < parameters.Length)
                {
                    parameters[index] = parameter;
                }
            }

            [Description("Adds parameter to pass to a procedure on the next available space in the parameter array")]
            public void AddParameters(MySqlParameter parameter)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] == null)
                    {
                        parameters[i] = parameter;
                        return;
                    }
                }

                throw new Exception("Parameter collection cannot contain anymore parameters");
            }

            #endregion


            /// <summary>
            /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
            /// to the provided command.
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
                catch (MySqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            #region Abstract Methods

            /// <summary>
            /// Data provider specific implementation for accessing relational databases.
            /// </summary>
            internal abstract IDbConnection GetDataProviderConnection();

            /// <summary>
            /// Data provider specific implementation for executing SQL statement while connected to a data source.
            /// </summary>
            internal abstract IDbCommand GetDataProviderCommand();

            /// <summary>
            /// Data provider specific implementation for filling the DataSet.
            /// </summary>
            internal abstract IDbDataAdapter GetDataProviderDataAdapter();

            #endregion

            #region Generic methods

            #region Database Transaction

            [Description("Iniciate the transaction")]
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
                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                }
                catch
                {
                    connection.Close();

                    throw;
                }
            }

            [Description("Commit the transaction")]
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

            /// <summary>
            /// Rolls back a transaction from a pending state.
            /// </summary>
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
                catch (MySqlException ex)
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
                catch (MySqlException ex)
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
                catch (MySqlException ex)
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

            /// <summary>
            /// Executes a stored procedure, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
            /// </summary>
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
                catch (MySqlException ex)
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
        /// Loads different data access layer provider depending on the configuration settings file or the caller defined data provider type.
        /// </summary>
        public sealed class DataAccessLayerFactory
        {
            /// <summary>
            /// Since this class provides only static methods, make the default constructor private to prevent 
            /// instances from being created with "new DataAccessLayerFactory()"
            /// </summary>
            private DataAccessLayerFactory()
            {
            }

            /// <summary>
            /// Constructs a data access layer data provider based on application configuration settings.
            /// Application configuration file must contain two keys: 
            ///		1. "DataProviderType" key, with one of the DataProviderType enumerator.
            ///		2. "ConnectionString" key, holds the database connection string.
            /// </summary>
            public static DataAccessLayerBaseClass GetDataAccessLayer(IConfiguration configuration)
            {
                ConnectionString = configuration.GetSection("ConnectionStrings")["ConnectionString"];
                DataProviderTypeString = configuration.GetSection("ConnectionStrings")["DataProviderType"];
                // Make sure application configuration file contains required configuration keys
                if (DataProviderTypeString == null || ConnectionString == null)
                    throw new ArgumentNullException(
                        "Please specify a 'DataProviderType' and 'ConnectionString' configuration keys in the application configuration file.");

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

            /// <summary>
            /// Constructs a data access layer based on caller specific data provider.
            /// Caller of this method must provide the database connection string through ConnectionString property.
            /// </summary>
            public static DataAccessLayerBaseClass GetDataAccessLayer(DataProviderType dataProviderType)
            {
                return GetDataAccessLayer(dataProviderType, null);
            }

            /// <summary>
            /// Constructs a data access layer data provider.
            /// </summary>
            public static DataAccessLayerBaseClass GetDataAccessLayer(DataProviderType dataProviderType,
                string connectionString)
            {
                // construct specific data access provider class
                switch (dataProviderType)
                {
                    //case DataProviderType.OleDb:
                    //    return new OleDbDataAccessLayer(connectionString);

                    //case DataProviderType.Odbc:
                    //    return new OdbcDataAccessLayer(connectionString);

                    //case DataProviderType.Oracle:
                    //    return new OracleDataAccessLayer(connectionString);

                    case DataProviderType.Mysql:
                        return new MySqlDataAccessLayer(connectionString);

                    default:
                        throw new ArgumentException("Invalid data access layer provider type.");
                }
            }
        }
    }
}
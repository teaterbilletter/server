using System.Data;
using MySql.Data.MySqlClient;

namespace Database.DatabaseConnector
{
    public class MySqlDataAccessLayer : DataAccessLayer.DataAccessLayerBaseClass
    {
        public MySqlDataAccessLayer() { }

        public MySqlDataAccessLayer(string connectionString)
        {
            ConnectionString = connectionString;
        }
        internal override IDbDataAdapter GetDataProviderDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        internal override IDbConnection GetDataProviderConnection()
        {
            return new MySqlConnection();
        }

        internal override IDbCommand GetDataProviderCommand()
        {
            return new MySqlCommand();
        }

    }
}
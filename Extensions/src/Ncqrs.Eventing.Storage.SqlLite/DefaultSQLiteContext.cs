using System;
using System.Data;
using System.Data.SQLite;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class DefaultSQLiteContext : ISQLiteContext
    {
        readonly string _connectionString;
        SQLiteConnection _connection;

        public DefaultSQLiteContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void WithConnection(Action<SQLiteConnection> action)
        {
            using (var connection = Connection)
                action(connection);
        }

        public void WithTransaction(SQLiteConnection connection, Action<SQLiteTransaction> action)
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    action(transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        SQLiteConnection Connection
        {
            get
            {
                _connection = _connection ?? new SQLiteConnection(_connectionString);
                
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                return _connection;
            }
        }
    }
}
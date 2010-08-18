using System;
using System.Data.SQLite;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class ManualSQLiteContext : ISQLiteContext
    {
        readonly SQLiteConnection _connection;
        readonly SQLiteTransaction _transaction;

        public ManualSQLiteContext(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void WithConnection(Action<SQLiteConnection> action)
        {
            action(_connection);
        }

        public void WithTransaction(SQLiteConnection connection, Action<SQLiteTransaction> action)
        {
            action(_transaction);
        }
    }
}
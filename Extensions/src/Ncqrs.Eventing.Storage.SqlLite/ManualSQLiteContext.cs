using System;
using System.Data.SQLite;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class ManualSQLiteContext : ISQLiteContext
    {
        SQLiteConnection _connection;
        SQLiteTransaction _transaction;

        public void SetContext(SQLiteConnection connection)
        {
            SetContext(connection, null);
        }

        public void SetContext(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void WithConnection(Action<SQLiteConnection> action)
        {
            if (_connection == null)
                throw new MissingSQLiteContextConnectionException();

            action(_connection);
        }

        public void WithTransaction(SQLiteConnection connection, Action<SQLiteTransaction> action)
        {
            if (_transaction == null)
                throw new MissingSQLiteContextTransactionException();

            action(_transaction);
        }
    }
}
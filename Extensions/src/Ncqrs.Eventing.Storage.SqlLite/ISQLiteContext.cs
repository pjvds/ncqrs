using System;
using System.Data.SQLite;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public interface ISQLiteContext
    {
        void WithConnection(Action<SQLiteConnection> action);
        void WithTransaction(SQLiteConnection connection, Action<SQLiteTransaction> action);
    }
}
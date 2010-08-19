using System;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class MissingSQLiteContextConnectionException : Exception
    {
        public MissingSQLiteContextConnectionException() : 
            base("You have attempted to use the connection on a ManualSQLiteContext without supplying a SQLiteConnection to SetContext.")
        {
        }
    }
}
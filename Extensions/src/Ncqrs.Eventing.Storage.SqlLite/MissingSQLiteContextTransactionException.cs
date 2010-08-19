using System;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class MissingSQLiteContextTransactionException : Exception
    {
        public MissingSQLiteContextTransactionException() :
            base("You have attempted to use the transaction on a ManualSQLiteContext without supplying a SQLiteTransaction to SetContext.")
        {
        }
    }
}
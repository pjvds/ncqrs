namespace Ncqrs.Eventing.Storage.SQLite{
    using System;
    using System.Data.SQLite;

    internal static class CommandExtensions{
        internal static SQLiteCommand SetTransaction(this SQLiteCommand cmd, SQLiteTransaction transaction)
        {
            if (cmd == null) throw new ArgumentNullException();
            cmd.Transaction = transaction;
            return cmd;
        }

        internal static SQLiteCommand AddParam(this SQLiteCommand cmd, string key, object value)
        {
            if (cmd == null || string.IsNullOrEmpty(key) || value == null) throw new ArgumentNullException();
            cmd.Parameters.AddWithValue(key, value);
            return cmd;
        }
    }
}
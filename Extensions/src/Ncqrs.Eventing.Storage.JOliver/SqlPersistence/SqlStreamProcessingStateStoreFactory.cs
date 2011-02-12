using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class SqlStreamProcessingStateStoreFactory
    {
        private readonly string _connectionName;
        private readonly IStreamProcessingStateStoreSqlDialect _dialect;

        public SqlStreamProcessingStateStoreFactory(string connectionName)
            : this(connectionName, null)
        {
        }
        public SqlStreamProcessingStateStoreFactory(string connectionName, IStreamProcessingStateStoreSqlDialect dialect)
        {
            _connectionName = connectionName;
            _dialect = dialect;
        }

        protected virtual string Name
        {
            get { return _connectionName; }
        }

        public virtual IStreamProcessingStateStore Build()
        {
            return new SqlStreamProcessingStateStore(
                new DelegateStreamProcessingStateStoreConnectionFactory(OpenConnection), this.GetDialect());
        }
        protected virtual IDbConnection OpenConnection()
        {
            var settings = GetConnectionSettings();
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            var connection = factory.CreateConnection() ?? new SqlConnection();
            connection.ConnectionString = TransformConnectionString(settings.ConnectionString);
            connection.Open();
            return connection;
        }
        protected virtual ConnectionStringSettings GetConnectionSettings()
        {
            // streamId allows use to change the connection based upon some kind of sharding strategy.
            return ConfigurationManager.ConnectionStrings[this.Name];
        }
        protected virtual string TransformConnectionString(string connectionString)
        {
            return connectionString;
        }
        protected virtual IStreamProcessingStateStoreSqlDialect GetDialect()
        {
            if (_dialect != null)
            {
                return this._dialect;
            }

            var settings = this.GetConnectionSettings();
            var connectionString = (settings.ConnectionString ?? string.Empty).ToUpperInvariant();
            var providerName = (settings.ProviderName ?? string.Empty).ToUpperInvariant();

            //if (providerName.Contains("MYSQL"))
            //    return new MySqlDialect();

            //if (providerName.Contains("SQLITE"))
            //    return new SqliteDialect();

            //if (providerName.Contains("SQLSERVERCE"))
            //    return new SqlCeDialect();

            //if (providerName.Contains("FIREBIRD"))
            //    return new FirebirdSqlDialect();

            //if (providerName.Contains("POSTGRES") || providerName.Contains("NPGSQL"))
            //    return new PostgreSqlDialect();

            //if (providerName.Contains("FIREBIRD"))
            //    return new FirebirdSqlDialect();

            //if (providerName.Contains("OLEDB") && connectionString.Contains("MICROSOFT.JET"))
            //    return new AccessDialect();

            return new MsSqlStreamProcessingStateStoreSqlDialect();
        }
    }
}
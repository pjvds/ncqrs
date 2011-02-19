using System;
using EventStore.Persistence;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class AbsoluteOrderingSqlPersistenceFactory : SqlPersistenceFactory
    {
        private readonly bool _transactional;
        private readonly IPipelineStoreSqlDialect _dialect;

        public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer, bool transactional)
            : this(connectionName, serializer, transactional, null)
		{
		}
		public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer, bool transactional, ISqlDialect dialect)
            : this(new ConfigurationConnectionFactory(connectionName), serializer, transactional, dialect)
		{
		}
		public AbsoluteOrderingSqlPersistenceFactory(IConnectionFactory factory, ISerialize serializer, bool transactional)
            : this(factory, serializer, transactional, null)
		{
		}
        public AbsoluteOrderingSqlPersistenceFactory(IConnectionFactory factory, ISerialize serializer, bool transactional, ISqlDialect dialect)
            :base(factory, serializer, dialect)
        {
            _transactional = transactional;
        }

        public override IPersistStreams Build()
        {
            return new AbsoluteOrderingSqlPersistenceEngine(ConnectionFactory, GetDialect(), GetPipelineStoreDialect(), Serializer, _transactional);
        }

        protected virtual IPipelineStoreSqlDialect GetPipelineStoreDialect()
        {
            if (_dialect != null)
            {
                return _dialect;
            }

            var settings = ConnectionFactory.Settings;
            var providerName = (settings.ProviderName ?? string.Empty).ToUpperInvariant();

            if (providerName.Contains("SQLSERVERCE"))
            {
                return new SqlCePipelineStoreSqlDialect();
            }

            return new MsSqlPipelineStoreSqlDialect();
        }
    }
}
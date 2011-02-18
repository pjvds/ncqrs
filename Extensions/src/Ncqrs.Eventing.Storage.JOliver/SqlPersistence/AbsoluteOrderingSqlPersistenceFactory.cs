using System;
using EventStore.Persistence;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class AbsoluteOrderingSqlPersistenceFactory : SqlPersistenceFactory
    {
        private readonly ISerialize _serializer;
        private readonly bool _transactional;
        private readonly IPipelineStoreSqlDialect _dialect;

        public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer, bool transactional) : base(connectionName, serializer)
        {
            _serializer = serializer;
            _transactional = transactional;
        }

        public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer, bool transactional, ISqlDialect dialect, IPipelineStoreSqlDialect pipelineStoreSqlDialect) : base(connectionName, serializer, dialect)
        {
            _serializer = serializer;
            _transactional = transactional;
            _dialect = pipelineStoreSqlDialect;
        }

        public override IPersistStreams Build()
        {
            return new AbsoluteOrderingSqlPersistenceEngine(
                new DelegateConnectionFactory(OpenConnection), GetDialect(), GetPipelineStoreDialect(), _serializer, _transactional);
        }

        protected virtual IPipelineStoreSqlDialect GetPipelineStoreDialect()
        {
            if (_dialect != null)
            {
                return _dialect;
            }

            var settings = this.GetConnectionSettings(Guid.Empty);
            var providerName = (settings.ProviderName ?? string.Empty).ToUpperInvariant();

            if (providerName.Contains("SQLSERVERCE"))
            {
                return new SqlCePipelineStoreSqlDialect();
            }

            return new MsSqlPipelineStoreSqlDialect();
        }
    }
}
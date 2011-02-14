using System;
using EventStore.Persistence;
using EventStore.Persistence.SqlPersistence;
using EventStore.Serialization;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class AbsoluteOrderingSqlPersistenceFactory : SqlPersistenceFactory
    {
        private readonly ISerialize _serializer;
        private readonly IPipelineStoreSqlDialect _dialect;

        public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer) : base(connectionName, serializer)
        {
            _serializer = serializer;
        }

        public AbsoluteOrderingSqlPersistenceFactory(string connectionName, ISerialize serializer, ISqlDialect dialect, IPipelineStoreSqlDialect pipelineStoreSqlDialect) : base(connectionName, serializer, dialect)
        {
            _serializer = serializer;
            _dialect = pipelineStoreSqlDialect;
        }

        public override IPersistStreams Build()
        {
            return new AbsoluteOrderingSqlPersistenceEngine(
                new DelegateConnectionFactory(OpenConnection), GetDialect(), GetPipelineStoreDialect(), _serializer);
        }

        protected virtual IPipelineStoreSqlDialect GetPipelineStoreDialect()
        {
            if (_dialect != null)
            {
                return _dialect;
            }

            return new MsSqlPipelineStoreSqlDialect();
        }
    }
}
using System;
using System.Data;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class DelegateStreamProcessingStateStoreConnectionFactory : IStreamProcessingStateStoreConnectionFactory
    {
        private readonly Func<IDbConnection> _openConnection;

        public DelegateStreamProcessingStateStoreConnectionFactory(Func<IDbConnection> openConnection)
        {
            _openConnection = openConnection;
        }

        public virtual IDbConnection OpenConnection()
        {
            return _openConnection();
        }        
    }
}
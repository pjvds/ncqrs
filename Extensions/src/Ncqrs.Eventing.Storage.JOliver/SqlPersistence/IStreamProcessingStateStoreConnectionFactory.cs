using System.Data;

namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public interface IStreamProcessingStateStoreConnectionFactory
    {
        IDbConnection OpenConnection();
    }
}
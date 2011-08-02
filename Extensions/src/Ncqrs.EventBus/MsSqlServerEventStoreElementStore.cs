using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.EventBus
{
    public class MsSqlServerEventStoreElementStore : IBrowsableElementStore
    {
        private const string MarkLastProcessedEventQuery = "INSERT INTO [PipelineState] ([PipelineName], [LastProcessedEventId]) VALUES (@PipelineName, @LastProcessedEventId)";
        private const string GetLastProcessedEventQuery = "SELECT TOP 1 [LastProcessedEventId] FROM [PipelineState] WHERE [PipelineName] = @PipelineName ORDER BY [BatchId] DESC";

        private readonly String _connectionString;
        private readonly MsSqlServerEventStore _wrappedStore;
        private Guid? _lastEventId;

        public MsSqlServerEventStoreElementStore(string connectionString)
        {
            _wrappedStore = new MsSqlServerEventStore(connectionString);
            _connectionString = connectionString;
        }
                
        public IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount)
        {
            if (!_lastEventId.HasValue)
            {
                _lastEventId = GetLastProcessedEvent(pipelineName);
            }            
            var result = _wrappedStore.GetEventsAfter(_lastEventId, maxCount);
            foreach (var evnt in result)
            {
                _lastEventId = evnt.EventIdentifier;
                yield return new SourcedEventProcessingElement(evnt);
            }
        }

        private Guid? GetLastProcessedEvent(string pipelineName)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(GetLastProcessedEventQuery, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("PipelineName", pipelineName);
                object result = command.ExecuteScalar();
                return (Guid?)result;
            }
        }

        public void MarkLastProcessedElement(string pipelineName, IProcessingElement processingElement)
        {
            var typedElement = (SourcedEventProcessingElement) processingElement;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(MarkLastProcessedEventQuery, connection))
            {
                command.Parameters.AddWithValue("LastProcessedEventId", typedElement.Event.EventIdentifier);
                command.Parameters.AddWithValue("PipelineName", pipelineName);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }        
    }
}
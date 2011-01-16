using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.SQL;

namespace Ncqrs.EventBus
{
    public class MsSqlServerBrowsableElementStore : IBrowsableElementStore
    {
        private const string MarkLastProcessedEventQuery = "INSERT INTO [PipelineState] ([LastProcessedEventId]) VALUES (@LastProcessedEventId)";
        private const string GetLastProcessedEventQuery = "SELECT TOP 1 [LastProcessedEventId] FROM [PipelineState] ORDER BY [BatchId] DESC";

        private readonly String _connectionString;
        private readonly MsSqlServerEventStore _wrappedStore;
        private Guid? _lastEventId;

        public MsSqlServerBrowsableElementStore(string connectionString)
        {
            _wrappedStore = new MsSqlServerEventStore(connectionString);
            _connectionString = connectionString;
        }
                
        public IEnumerable<IProcessingElement> Fetch(int maxCount)
        {
            if (!_lastEventId.HasValue)
            {
                _lastEventId = GetLastProcessedEvent();
            }            
            var result = _wrappedStore.GetEventsAfter(_lastEventId, maxCount);
            foreach (var evnt in result)
            {
                _lastEventId = evnt.EventIdentifier;
                yield return new SourcedEventProcessingElement(evnt);
            }
        }

        private Guid? GetLastProcessedEvent()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(GetLastProcessedEventQuery, connection))
            {
                connection.Open();

                object result = command.ExecuteScalar();
                return (Guid?)result;
            }
        }

        public void MarkLastProcessedEvent(IProcessingElement processingElement)
        {
            var typedElement = (SourcedEventProcessingElement) processingElement;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(MarkLastProcessedEventQuery, connection))
            {
                command.Parameters.AddWithValue("LastProcessedEventId", typedElement.Event.EventIdentifier);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }        
    }
}
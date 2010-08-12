using System;
using System.Data.SqlClient;

namespace Ncqrs.EventBus
{
    public class MsSqlServerPipelineStateStore : IPipelineStateStore
    {
        private readonly String _connectionString;

        public MsSqlServerPipelineStateStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        private const string MarkLastProcessedEventQuery = "INSERT INTO [PipelineState] ([LastProcessedEventId]) VALUES (@LastProcessedEventId)";
        private const string GetLastProcessedEventQuery = "SELECT TOP 1 [LastProcessedEventId] FROM [PipelineState] ORDER BY [BatchId] DESC";

        public void MarkLastProcessedEvent(SequencedEvent evnt)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(MarkLastProcessedEventQuery, connection))
            {
                command.Parameters.AddWithValue("LastProcessedEventId", evnt.Event.EventIdentifier);                
                connection.Open();
                command.ExecuteNonQuery();                
            }
        }

        public Guid? GetLastProcessedEvent()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(GetLastProcessedEventQuery, connection))
            {
                connection.Open();

                object result = command.ExecuteScalar();
                return (Guid?) result;
            }
        }
    }
}
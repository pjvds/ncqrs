using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage.SQL;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage.SQL
{
    public class SimpleMicrosoftSqlServerEventStoreTests
    {
        [Test]
        public void Retrieving_table_creation_queries_should_return_dll()
        {
            var dllQueries = SimpleMicrosoftSqlServerEventStore.GetTableCreationQueries();
            
            dllQueries.Should().NotBeNull().And.NotBeEmpty();
        }
    }
}

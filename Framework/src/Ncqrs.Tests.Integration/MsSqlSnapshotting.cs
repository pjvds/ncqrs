using System.Configuration;
using System.Data.SqlClient;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class MsSqlSnapshotting : Snapshotting
    {
        [SetUp]
        public void CleaDatabase()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "TRUNCATE TABLE [Events]";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "TRUNCATE TABLE [EventSources]";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "TRUNCATE TABLE [Snapshots]";
                cmd.ExecuteNonQuery();
            }
        }
       
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MsSqlEventStore"].ConnectionString;
        }

        protected override IEventStore BuildEventStore()
        {
            var store = new MsSqlServerEventStore(GetConnectionString());
            NcqrsEnvironment.SetDefault<ISnapshotStore>(store);
            return store;
        }
    }
}
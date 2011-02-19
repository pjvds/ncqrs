using System.Configuration;
using System.Data.SqlClient;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.SQL;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class MsSqlFetureTests : FetureTests
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

        protected override void InitializeEnvironment()
        {
            var store = new MsSqlServerEventStore(GetConnectionString());
            NcqrsEnvironment.SetDefault<ISnapshotStore>(store);
            NcqrsEnvironment.SetDefault<IEventStore>(store);
            NcqrsEnvironment.SetDefault<IUnitOfWorkFactory>(new UnitOfWorkFactory());
        }
    }
}
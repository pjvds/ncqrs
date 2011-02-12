namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class MsSqlStreamProcessingStateStoreSqlDialect : CommonStreamProcessingStateStoreSqlDialect
    {
        public override string Initialize
        {
            get { return MsSqlStatements.Initialize; }
        }
    }
}
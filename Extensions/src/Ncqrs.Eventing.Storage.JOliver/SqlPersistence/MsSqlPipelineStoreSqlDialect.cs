namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class MsSqlPipelineStoreSqlDialect : CommonPipelineStoreSqlDialect
    {
        public override string Initialize
        {
            get { return MsSqlStatements.Initialize; }
        }
    }
}
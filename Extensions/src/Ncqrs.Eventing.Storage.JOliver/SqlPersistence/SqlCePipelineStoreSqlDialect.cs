namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public class SqlCePipelineStoreSqlDialect : CommonPipelineStoreSqlDialect
    {
        public override string Initialize
        {
            get { return SqlCeStatements.Initialize; }
        }

        public override string GetLastProcessedCommit
        {
            get { return SqlCeStatements.GetLastProcessedCommit; }
        }

        public override string MarkLastProcessedCommit
        {
            get { return SqlCeStatements.MarkLastProcessedCommit; }
        }
    }
}
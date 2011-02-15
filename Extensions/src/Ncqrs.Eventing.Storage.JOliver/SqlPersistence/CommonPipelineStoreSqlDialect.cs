namespace Ncqrs.Eventing.Storage.JOliver.SqlPersistence
{
    public abstract class CommonPipelineStoreSqlDialect : IPipelineStoreSqlDialect
    {
        public abstract string Initialize { get; }

        public virtual string GetCommitsAfter
        {
            get { return CommonSqlStatements.GetCommitsAfter; }
        }

        public virtual string MarkLastProcessedCommit
        {
            get { return CommonSqlStatements.MarkLastProcessedCommit; }
        }

        public virtual string GetLastProcessedCommit
        {
            get { return CommonSqlStatements.GetLastProcessedCommit; }
        }

        public virtual string RegisterSequentialId
        {
            get { return CommonSqlStatements.RegisterSequentialId; }
        }

        public string PipelineName { get { return "@PipelineName"; } }
        public string CommitId { get { return "@CommitId"; } }
        public string SequentialIdColumn { get { return "SequentialId"; } }
        public string SequentialId { get { return "@SequentialId"; } }
    }
}
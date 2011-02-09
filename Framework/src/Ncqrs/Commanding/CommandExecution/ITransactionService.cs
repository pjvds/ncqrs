using System;

namespace Ncqrs.Commanding.CommandExecution
{
    public interface ITransactionService
    {
        void ExecuteInTransaction(Action action);
    }
}
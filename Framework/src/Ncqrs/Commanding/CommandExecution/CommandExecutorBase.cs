using System;
using Ncqrs.Domain;

namespace Ncqrs.Commanding.CommandExecution
{
    /// <summary>
    /// Represents a command executor.
    /// </summary>
    /// <code lang="c#">
    /// public class AddProductToShoppingCartExecutor : CommandExecutorBase
    /// {
    ///     protected override void ExecuteInContext(IUnitOfWorkContext context, AddProductToShoppingCart command)
    ///     {
    ///         // Get the shopping cart.
    ///         var shoppingCart = context.GetById(command.ShoppingCartId);
    /// 
    ///         // Add the product to the shopping cart.
    ///         shoppingCart.AddProduct(command.ProductId, command.Amount);
    ///
    ///         // Accept all the work we just did.
    ///         context.Accept();
    ///     }
    /// }
    /// </code>
    /// <typeparam name="TCommand">The type of the commands to execute.</typeparam>
    public abstract class CommandExecutorBase<TCommand> : ICommandExecutor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Holds the <see cref="IUnitOfWorkFactory"/>. This instance should never be null.
        /// </summary>
        private readonly IUnitOfWorkFactory _factory;

        protected CommandExecutorBase()
            : this(NcqrsEnvironment.Get<IUnitOfWorkFactory>())
        {
        }

        protected CommandExecutorBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if(unitOfWorkFactory == null) throw new ArgumentNullException("unitOfWorkFactory");
            _factory = unitOfWorkFactory;
        }
        
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command to execute. This should not be null.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>command</i> is null.</exception>
        public void Execute(TCommand command)
        {
            using (var work = _factory.CreateUnitOfWork(command.CommandIdentifier))
            {
                ExecuteInContext(work, command);
            }
        }

        /// <summary>
        /// Executes the command withing an unit of work context.
        /// <remarks>Make sure you call <see cref="IUnitOfWorkContext.Accept"/> to accept the changes that has been made in the context.</remarks>
        /// </summary>
        /// <example>
        /// <code lang="c#">
        /// public class AddProductToShoppingCartExecutor : CommandExecutorBase
        /// {
        ///     protected override void ExecuteInContext(IUnitOfWorkContext context, AddProductToShoppingCart command)
        ///     {
        ///         // Get the shopping cart.
        ///         var shoppingCart = context.GetById(command.ShoppingCartId);
        /// 
        ///         // Add the product to the shopping cart.
        ///         shoppingCart.AddProduct(command.ProductId, command.Amount);
        ///
        ///         // Accept all the work we just did.
        ///         context.Accept();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="context">The work context.</param>
        /// <param name="command">The command to execute.</param>
        protected abstract void ExecuteInContext(IUnitOfWorkContext context, TCommand command);
    }
}
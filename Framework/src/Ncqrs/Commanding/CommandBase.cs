using System;
using Ncqrs.Commanding.CommandExecution.Mapping.Reflection;

namespace Ncqrs.Commanding
{
    /// <summary>
    /// The base of a command message. A command should contain all the
    /// information and intend that is needed to execute an corresponding
    /// action.
    /// </summary>
    [Serializable]
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Gets the unique identifier for this command.
        /// </summary>
        [ExcludeInMapping] // TODO: Why do we need this when it is also declared in the ICommand interface?
        public Guid CommandIdentifier
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <remarks>
        /// This uses the <see cref="NcqrsEnvironment.Get{IUniqueIdentifierGenerator}"/> to get
        /// the generator to use to generate the command identifier.
        /// </remarks>
        protected CommandBase()
        {
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            CommandIdentifier = idGenerator.GenerateNewId();
        }
    }
}

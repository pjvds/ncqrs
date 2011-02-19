using System;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Commanding
{
    /// <summary>
    /// A command message. A command should contain all the information and
    /// intend that is needed to execute an corresponding action.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the unique identifier for this command.
        /// </summary>
        [ExcludeInMapping]
        Guid CommandIdentifier
        { 
            get;
        }

        /// <summary>
        /// Gets the known version of the aggregate root.
        /// This can be used for optimistic concurrency.
        /// When set, the command should only be executed
        /// when the current version of the aggregate root
        /// is the same as the known version.
        /// </summary>
        long? KnownVersion
        {
            get;
        }
    }
}

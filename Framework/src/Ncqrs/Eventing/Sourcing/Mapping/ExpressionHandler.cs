using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    /// <summary>
    /// The base implementation for the handling of expression based mapping.
    /// </summary>
    public abstract class ExpressionHandler
    {
        /// <summary>
        /// Gets the action that is to be invoked.
        /// </summary>
        public MethodInfo ActionMethodInfo
        { get; protected set; }

        /// <summary>
        /// Gets the value that determines if the given mapping should match exactly.
        /// </summary>
        public bool Exact
        { get; protected set; }
    }

    /// <summary>
    /// The generic base implementation for the handling of expression based mapping.
    /// </summary>
    /// <typeparam name="T">This should always be a <see cref="ISourcedEvent"/>.</typeparam>
    public class ExpressionHandler<T> : ExpressionHandler
    {
        /// <summary>
        /// Stores the given <see cref="Action{T}"/> action.
        /// </summary>
        /// <param name="mappingaction">The <see cref="Action{T}"/> to map.</param>
        /// <returns>Itself <see cref="ExpressionHandler{T}"/>.</returns>
        public ExpressionHandler<T> ToHandler(Action<T> mappingaction)
        {
            Contract.Requires<ArgumentNullException>(mappingaction != null, "The mappingaction can not be null.");
            ActionMethodInfo = mappingaction.Method;

            return this;
        }

        /// <summary>
        /// Determines that the action should map <b>exactly</b> on the given method in the given type.
        /// </summary>
        public void MatchExact()
        {
            Exact = true;
        }
    }
}
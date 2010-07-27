namespace Ncqrs.Commanding.ServiceModel
{
    /// <summary>
    /// Allows users to intercept commands before or after they has been
    /// executed in a command service. This allows them to add additional
    /// behavior before or after the execution of commands. The 
    /// <see cref = "OnBeforeExecution" /> method will be called before every
    /// command execution and <see cref = "OnAfterExecution" /> will be called
    /// after every command execution.
    /// </summary>
    /// <example>
    /// The following code explains the different things that could happen in
    /// the context of interception.
    /// <code lang="c#">
    /// public class SampleInterceptor : ICommandServiceInterceptor
    /// {
    ///     public void OnBeforeBeforeExecutorResolving(CommandContext context)
    ///     {
    ///         if(context.TheCommandType == typeof(MyCommand))
    ///         {
    ///             // Convert the command to MyCommandV2.
    ///             MyCommand cmd = (MyCommand)context.TheCommand;
    ///             context.TheCommandType = MyCommandV2.CreateFrom(cmd);
    ///         }
    ///     }
    /// 
    ///     public void OnBeforeExecution(CommandContext context)
    ///     {
    ///         // Null when no executor was found for the command; otherwise, it hold
    ///         // the executor that will be called to execute the command after all
    ///         // interceptors has been called.
    ///         var executor = context.TheCommandExecutor;
    /// 
    ///         // This is allways null in OnBeforeExecution.
    ///         var exception = context.Exception;
    /// 
    ///         // This is allways false in OnBeforeExecution.
    ///         var isExecuted = context.IsExecuted;
    /// 
    ///         // Holds the command that will be executed.
    ///         // This is never a null dereference.
    ///         var command = context.TheCommand;
    /// 
    ///         // Holds the type of the command that will be
    ///         // executed. This is never a null dereference.
    ///         var commandType = context.TheCommandType;
    ///     }
    /// 
    ///     public void OnAfterExecution(CommandContext context)
    ///     {
    ///         // Null when no executor was found for the command; otherwise, it hold
    ///         // the executor that was called to execute the command.
    ///         var executor = context.TheCommandExecutor;
    /// 
    ///         // If occurred, it holds the exception that was been thrown in the context; otherwise, null.
    ///         // Use the context.IsExecuted method to dermine whether the 
    ///         var exception = context.Exception;
    /// 
    ///         // Holds true whenever the command executor is called to execute the command. This does
    ///         // not mean the execution was succesfull. Check the context.Exception method to dermine
    ///         // whether there was an exception on execution.
    ///         // When this holds false, this means the command was not executed due the fact that there
    ///         // was not handler found, or that an interceptor throwed an exception.
    ///         var isExecuted = context.IsExecuted;
    /// 
    ///         // Holds the command that was been executed.
    ///         var command = context.TheCommand;
    /// 
    ///         // Holds the type of the command that will be
    ///         // executed. This is never a null dereference.
    ///         var commandType = context.TheCommandType;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ICommandServiceInterceptor
    {
        /// <summary>
        /// Intercepts a command execution before the command executor is
        /// resolved.
        /// <para>In this method the interceptor may override the command by
        /// setting the <see cref="CommandContext.TheCommand"/> property.</para>
        /// </summary>
        /// <param name="context">The context of the current command execution.
        /// </param>
        void OnBeforeBeforeExecutorResolving(CommandContext context);

        /// <summary>
        /// Intercepts a command execution before the command will be executed,
        /// but after the executor has been resolved.
        /// </summary>
        /// <param name = "context">The context of the current command execution.
        /// </param>
        void OnBeforeExecution(CommandContext context);

        /// <summary>
        /// Intercepts a command execution when a exception has been occurred or
        /// after the command has been executed. See the 
        /// <see cref = "CommandContext.ExecutorHasBeenCalled" /> to determine whether the
        /// command has been executed.
        /// </summary>
        /// <param name = "context">The context of the current command
        /// execution.
        /// </param>
        void OnAfterExecution(CommandContext context);
    }
}
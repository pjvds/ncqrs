namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public interface IMappingAttributeHandler<in T>
    {
        void Map(T attribute, ICommand command, IMappedCommandExecutor executor);
    }
}
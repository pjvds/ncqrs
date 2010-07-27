namespace Ncqrs.Commanding.ServiceModel
{
    public interface ICommandService
    {
        void Execute(ICommand command);
    }
}
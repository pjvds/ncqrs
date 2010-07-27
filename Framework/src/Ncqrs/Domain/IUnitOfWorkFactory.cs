namespace Ncqrs.Domain
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWorkContext CreateUnitOfWork();
    }
}

using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing
{
	public abstract class EventServiceMappedByConvention : AggregateRootMappedByConvention
	{
		public void Raise(ISourcedEvent e)
		{
			var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
			var context = factory.CreateUnitOfWork(e.EventIdentifier);

			ApplyEvent(e);

			context.Accept();
		}
	}
}
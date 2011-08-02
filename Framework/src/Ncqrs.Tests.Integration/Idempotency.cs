using System;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Tests.Integration.Domain;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public abstract class Idempotency
    {
        

        protected abstract void InitializeEnvironment();
    }
}
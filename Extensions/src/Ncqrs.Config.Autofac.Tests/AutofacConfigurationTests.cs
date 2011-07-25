using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Autofac;
using FluentAssertions;

namespace Ncqrs.Config.Autofac.Tests
{
    [TestFixture]
    public class AutofacConfigurationTests
    {
        [Test]
        public void When_component_is_registered_it_should_be_retrievable()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<Nexus6>().As<IReplicant>();

            var container = containerBuilder.Build();

            var configuration = new AutofacConfiguration(container);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeTrue();
            component.Should().NotBeNull();
            component.Should().BeOfType<Nexus6>();
        }

        [Test]
        public void When_component_is_not_registered_it_should_not_be_retrievable()
        {
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            var configuration = new AutofacConfiguration(container);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeFalse();
            component.Should().BeNull();
        }
    }

    public interface IReplicant { }
    public class Nexus6 : IReplicant { }
}

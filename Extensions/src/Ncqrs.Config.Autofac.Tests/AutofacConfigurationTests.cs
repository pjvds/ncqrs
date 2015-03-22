using Autofac;
using FluentAssertions;
using NUnit.Framework;

namespace Ncqrs.Config.Autofac.Tests
{
    [TestFixture]
    public class AutofacConfigurationTests
    {
        [Test]
        public void When_component_is_registered_it_should_be_retrievable()
        {
            var kernel = new ContainerBuilder();
            kernel.RegisterType<Nexus6>().As<IReplicant>();

            var container = kernel.Build();
            var configuration = new AutofacConfiguration(container);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeTrue();
            component.Should().NotBeNull();
            component.Should().BeOfType<IReplicant>();
        }

        [Test]
        public void When_component_is_not_registered_it_should_not_be_retrievable()
        {
            var kernel = new ContainerBuilder();
            var container = kernel.Build();
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
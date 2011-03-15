using FluentAssertions;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Ncqrs.Config.Unity.Tests
{
    [TestFixture]
    public class UnityConfigurationTest
    {
        [Test]
        public void When_component_is_registered_it_should_be_retrievable()
        {
            var container = new UnityContainer();
            var nex = new Nexus6();
            container.RegisterInstance<IReplicant>(nex);

            var configuration = new UnityConfiguration(container);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeTrue();
            component.Should().NotBeNull();
            component.Should().BeAssignableTo<IReplicant>();
            component.Should().BeSameAs(nex);
        }

        [Test]
        public void When_component_is_not_registered_it_should_not_be_retrievable()
        {
            var container = new UnityContainer();
            var configuration = new UnityConfiguration(container);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeFalse();
            component.Should().BeNull();
        }
    }

    public interface IReplicant { }
    public class Nexus6 : IReplicant { }
}

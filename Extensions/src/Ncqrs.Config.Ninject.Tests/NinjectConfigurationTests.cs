using Ninject;
using FluentAssertions;
using Xunit;

namespace Ncqrs.Config.Ninject.Tests
{
    public class NinjectConfigurationTests
    {
        [Fact]
        public void When_component_is_registered_it_should_be_retrievable()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IReplicant>().To<Nexus6>();

            var configuration = new NinjectConfiguration(kernel);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeTrue();
            component.Should().NotBeNull();
            component.Should().BeAssignableTo<IReplicant>();
        }

        [Fact]
        public void When_component_is_not_registered_it_should_not_be_retrievable()
        {
            var kernel = new StandardKernel();
            var configuration = new NinjectConfiguration(kernel);

            IReplicant component;
            var success = configuration.TryGet(out component);

            success.Should().BeFalse();
            component.Should().BeNull();
        }
    }

    public interface IReplicant { }
    public class Nexus6 : IReplicant { }
}

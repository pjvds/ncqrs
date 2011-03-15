using Microsoft.Practices.Unity;

namespace Ncqrs.Config.Unity
{
    public class UnityConfiguration : IEnvironmentConfiguration
    {
        private readonly IUnityContainer _container;

        public UnityConfiguration(IUnityContainer container)
        {
            _container = container;
        }

        public bool TryGet<T>(out T result) where T : class
        {
            result = _container.IsRegistered<T>() ? _container.Resolve<T>() : default(T);

            return result != null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace Ncqrs.Eventing.Denormalization
{
    public class DenormalizerFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<IDenormalizer> CreateDenormalizersFromAssembly(Assembly asm)
        {
            Log.DebugFormat("Creating denormalizers from assembly {0}.", asm.FullName);

            var query = from t in asm.GetTypes()
                        where t.IsClass && typeof(IDenormalizer).IsAssignableFrom(t)
                        select t;

            foreach (var denormalizerType in query)
            {
                Log.DebugFormat("Found potential denormalizer {0}.", denormalizerType.FullName);

                var defaultCtor = denormalizerType.GetConstructor(Type.EmptyTypes);

                if (defaultCtor == null)
                {
                    Log.WarnFormat("Skipped type {0} because it has no public empty constructor.", denormalizerType.FullName);
                    continue;
                }

                var denormalizer = (IDenormalizer)Activator.CreateInstance(denormalizerType);

                Log.DebugFormat("Denormalizer created of type {0}.", denormalizerType.FullName);
                yield return denormalizer;
            }
        }
    }
}

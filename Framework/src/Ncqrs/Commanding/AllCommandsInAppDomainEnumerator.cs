using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Commanding
{
    public class AllCommandsInAppDomainEnumerator : IKnownCommandsEnumerator
    {
        public IEnumerable<Type> GetAllCommandTypes()
        {
            return from asm in AppDomain.CurrentDomain.GetAssemblies()
                         from type in asm.GetTypes()
                         where typeof(CommandBase).IsAssignableFrom(type)
                         select type;
        }
    }
}

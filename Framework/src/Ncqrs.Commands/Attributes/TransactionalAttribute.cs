using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TransactionalAttribute : Attribute
    {
    }
}
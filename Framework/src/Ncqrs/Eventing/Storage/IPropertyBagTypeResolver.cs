using System;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Resolves types that no longer exist, have been renamed or moved to a different namespace
    /// to their current representation.
    /// </summary>
    /// <remarks>
    /// Is called during the conversion process in <see cref="IPropertyBagConverter.Convert"/> to 
    /// construct the correct type of an event or snapshot.
    /// </remarks>
    public interface IPropertyBagTypeResolver
    {
        Type Resolve(string className, string nameSpace, string assemblyName);
    }
}
using System;

namespace Ncqrs
{
    public interface IUniqueIdentifierGenerator
    {
        Guid GenerateNewId();
 
    }
}
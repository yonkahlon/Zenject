using System;
using System.Collections.Generic;

namespace Zenject
{
    public interface IValidatableFactory
    {
        Type ConstructedType
        {
            get;
        }

        IEnumerable<Type> ProvidedTypes
        {
            get;
        }
    }
}

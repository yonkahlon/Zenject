using System;

namespace Zenject
{
    public interface IValidatableFactory
    {
        Type ConstructedType
        {
            get;
        }

        Type[] ProvidedTypes
        {
            get;
        }
    }
}

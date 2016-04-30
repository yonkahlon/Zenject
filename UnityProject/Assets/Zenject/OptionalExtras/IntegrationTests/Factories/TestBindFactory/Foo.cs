using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.TestBindFactory
{
    public interface IFoo
    {
    }

    public class IFooFactory : Factory<IFoo>
    {
    }

    public class Foo : MonoBehaviour, IFoo
    {
        public class Factory : Factory<Foo>
        {
        }
    }
}

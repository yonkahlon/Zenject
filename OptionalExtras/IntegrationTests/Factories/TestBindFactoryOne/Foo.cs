using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.TestBindFactoryOne
{
    public interface IFoo
    {
        string Value
        {
            get;
        }
    }

    public class IFooFactory : Factory<string, IFoo>
    {
    }

    public class Foo : MonoBehaviour, IFoo
    {
        [PostInject]
        public void Init(string value)
        {
            Value = value;
        }

        public string Value
        {
            get;
            private set;
        }

        public class Factory : Factory<string, Foo>
        {
        }
    }
}

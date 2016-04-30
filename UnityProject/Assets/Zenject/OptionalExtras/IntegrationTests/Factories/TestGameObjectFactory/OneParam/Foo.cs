using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.TestGameObjectFactory.OneParams
{
    public class Foo : MonoBehaviour
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
    }
}

using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToGameObject
{
    public class Foo : MonoBehaviour
    {
        [Inject]
        public void Init(Bar bar)
        {
            Assert.IsNotNull(bar);

            Log.Trace("Received bindings from main scene in Foo");
        }
    }
}

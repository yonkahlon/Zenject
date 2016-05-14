using ModestTree;
using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToGameObject
{
    public class Bar : MonoBehaviour
    {
        [Inject]
        public void Init(Foo foo, Gorp gorp)
        {
            Assert.IsNotNull(gorp);
            Assert.IsNotNull(foo);

            Log.Trace("Received decorated bindings in Bar");
        }
    }
}

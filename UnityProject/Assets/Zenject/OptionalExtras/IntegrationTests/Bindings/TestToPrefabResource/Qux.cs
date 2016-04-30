using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToPrefabResource
{
    public class Qux : MonoBehaviour
    {
        [Inject]
        int _arg;

        [PostInject]
        public void Initialize()
        {
            Log.Trace("Received arg '{0}' in Qux", _arg);
        }
    }
}

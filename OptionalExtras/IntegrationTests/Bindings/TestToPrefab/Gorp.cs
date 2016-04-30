using UnityEngine;
using Zenject;

namespace ModestTree.Tests.Zenject.ToPrefab
{
    public class Gorp : MonoBehaviour
    {
        [Inject]
        string _arg;

        [PostInject]
        public void Initialize()
        {
            Log.Trace("Received arg '{0}' in Gorp", _arg);
        }
    }
}

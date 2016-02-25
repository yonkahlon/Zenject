using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class FooMonoFacade : MonoFacade
    {
        FooMonoFacadeRunner _runner;

        [PostInject]
        public void Constrct(FooMonoFacadeRunner runner)
        {
            _runner = runner;
        }

        public FooMonoFacadeRunner Runner
        {
            get
            {
                return _runner;
            }
        }

        public class Factory : MonoFacadeFactory<FooMonoFacade>
        {
        }
    }
}

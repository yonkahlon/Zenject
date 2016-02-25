using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class FooMonoFacadeWithArg : MonoFacade
    {
        FooMonoFacadeRunner _runner;
        string _arg;

        [PostInject]
        public void Construct(FooMonoFacadeRunner runner, string arg)
        {
            _runner = runner;
            _arg = arg;
        }

        public string Arg
        {
            get
            {
                return _arg;
            }
        }

        public FooMonoFacadeRunner Runner
        {
            get
            {
                return _runner;
            }
        }

        public class Factory : MonoFacadeFactory<string, FooMonoFacadeWithArg>
        {
        }
    }
}

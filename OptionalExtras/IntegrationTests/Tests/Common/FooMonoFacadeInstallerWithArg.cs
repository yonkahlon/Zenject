using System;
using Zenject;

namespace ModestTree
{
    public class FooMonoFacadeInstallerWithArg : MonoInstaller
    {
        string _value;

        [PostInject]
        public void Construct(string value)
        {
            _value = value;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_value).WhenInjectedInto<FooMonoFacadeWithArg>();

            Container.BindAllInterfaces<FooMonoFacadeRunner>().ToSingle<FooMonoFacadeRunner>();
            Container.Bind<FooMonoFacadeRunner>().ToSingle();
        }
    }
}

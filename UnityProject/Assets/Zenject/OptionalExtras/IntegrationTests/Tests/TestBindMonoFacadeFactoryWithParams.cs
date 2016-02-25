using System;
using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class TestBindMonoFacadeFactoryWithParams : MonoInstallerTestFixture
    {
        public GameObject FooMonoFacadePrefab;

        [InstallerTest]
        public void Test1()
        {
            Container.BindMonoFacadeFactory<FooMonoFacadeInstallerWithArg, FooMonoFacadeWithArg.Factory>(FooMonoFacadePrefab);

            Container.BindAllInterfaces<Runner>().ToSingle<Runner>();
        }

        public class Runner : IInitializable, IDisposable
        {
            readonly FooMonoFacadeWithArg.Factory _factory;
            FooMonoFacadeWithArg _facade;

            public Runner(FooMonoFacadeWithArg.Factory factory)
            {
                _factory = factory;
            }

            public void Initialize()
            {
                _facade = _factory.Create("asdf");

                Assert.IsEqual(_facade.Arg, "asdf");

                // Apparently objects that are created within the Start() event do not get their Start()
                // called immediately, which makes some sense
                Assert.That(!_facade.Runner.HasInitialized);
                Assert.That(!_facade.Runner.HasDisposed);
                Assert.That(!_facade.Runner.HasTicked);
            }

            public void Tick()
            {
                Assert.That(_facade.Runner.HasInitialized);
            }

            public void Dispose()
            {
                Assert.That(_facade.Runner.HasInitialized);
                Assert.That(_facade.Runner.HasTicked);

                // Facades get their OnDestroy called based on their own execution order etc.
                // so there is no guarantee of this
                //Assert.That(_facade.Runner.HasDisposed);
            }
        }
    }
}


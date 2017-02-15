using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject;

#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace ZenjectSignalsAndSignals.Tests
{
    [TestFixture]
    public class TestSignals2 : ZenjectIntegrationTestFixture
    {
        [SetUp]
        public void CommonInstall()
        {
            Container.BindInterfacesAndSelfTo<SignalManager>().AsSingle();
        }

        [Test]
        public void TestToSingle1()
        {
            Bar.WasTriggered = false;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsSingle();

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsSingle();

            Initialize();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.That(!Bar.WasTriggered);

            cmd.Fire();

            Assert.That(Bar.WasTriggered);
            Assert.IsEqual(Bar.InstanceCount, 1);
        }

        [Test]
        public void TestToCached1()
        {
            Bar.WasTriggered = false;
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsCached();

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsCached();

            Initialize();

            Container.Resolve<Bar>();
            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.IsEqual(Bar.InstanceCount, 1);
            Assert.That(!Bar.WasTriggered);

            cmd.Fire();

            Assert.That(Bar.WasTriggered);
            Assert.IsEqual(Bar.InstanceCount, 2);

            cmd.Fire();
            Assert.IsEqual(Bar.InstanceCount, 2);
        }

        [Test]
        public void TestNoHandlerDefault()
        {
            Container.DeclareSignal<DoSomethingSignal>();

            Initialize();

            var cmd = Container.Resolve<DoSomethingSignal>();
            cmd.Fire();
        }

        [Test]
        public void TestNoHandlerRequiredFailure()
        {
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            Initialize();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => cmd.Fire());
        }

        [Test]
        public void TestNoHandlerRequiredSuccess()
        {
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();
            Container.BindSignal<DoSomethingSignal>()
                .To<Bar>(x => x.Execute).AsCached();

            Initialize();

            var cmd = Container.Resolve<DoSomethingSignal>();
            cmd.Fire();
        }

        [Test]
        public void TestHandlerRequiredEventStyle()
        {
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            Initialize();

            var signal = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => signal.Fire());

            bool received = false;

            Action receiveSignal = () => received = true;

            signal += receiveSignal;

            Assert.That(!received);
            signal.Fire();
            Assert.That(received);

            signal -= receiveSignal;

            Assert.Throws(() => signal.Fire());
        }

#if ZEN_SIGNALS_ADD_UNIRX
        [Test]
        public void TestHandlerRequiredUniRxStyle()
        {
            Container.DeclareSignal<DoSomethingSignal>().RequireHandler();

            Initialize();

            var signal = Container.Resolve<DoSomethingSignal>();

            Assert.Throws(() => signal.Fire());

            bool received = false;

            var subscription = signal.AsObservable.Subscribe((x) => received = true);

            Assert.That(!received);
            signal.Fire();
            Assert.That(received);

            subscription.Dispose();

            Assert.Throws(() => signal.Fire());
        }
#endif

        [Test]
        public void TestToMethod()
        {
            bool wasCalled = false;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled = true);

            Initialize();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.That(!wasCalled);
            cmd.Fire();
            Assert.That(wasCalled);
        }

        [Test]
        public void TestMultipleHandlers()
        {
            bool wasCalled1 = false;
            bool wasCalled2 = false;

            Container.DeclareSignal<DoSomethingSignal>();
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled1 = true);
            Container.BindSignal<DoSomethingSignal>()
                .To(() => wasCalled2 = true);

            Initialize();

            var cmd = Container.Resolve<DoSomethingSignal>();

            Assert.That(!wasCalled1);
            Assert.That(!wasCalled2);
            cmd.Fire();
            Assert.That(wasCalled1);
            Assert.That(wasCalled2);
        }

        public class DoSomethingSignal : Signal<DoSomethingSignal>
        {
        }

        public class Bar
        {
            public static int InstanceCount = 0;

            public Bar()
            {
                InstanceCount ++;
            }

            public static bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
            }
        }
    }
}


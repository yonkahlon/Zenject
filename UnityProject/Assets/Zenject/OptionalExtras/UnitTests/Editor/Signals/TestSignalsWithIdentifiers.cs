using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSignalsWithIdentifiers : ZenjectUnitTestFixture
    {
        [Test]
        public void RunTest()
        {
            Container.BindSignal<SomethingHappenedSignal>();
            Container.BindTrigger<SomethingHappenedSignal.Trigger>();

            Container.Bind<Foo>().AsSingle();
            Container.Bind<Bar>().AsSingle();

            Container.BindSignal<SomethingHappenedSignal>("special");
            Container.BindTrigger<SomethingHappenedSignal.Trigger>("special");

            Container.Bind<FooSpecial>().AsSingle();
            Container.Bind<BarSpecial>().AsSingle();

            var foo = Container.Resolve<Foo>();
            var bar = Container.Resolve<Bar>();

            var fooSpecial = Container.Resolve<FooSpecial>();
            var barSpecial = Container.Resolve<BarSpecial>();

            bar.Initialize();
            barSpecial.Initialize();

            Assert.IsNull(bar.ReceivedValue);
            Assert.IsNull(barSpecial.ReceivedValue);

            foo.DoSomething("asdf");

            Assert.IsEqual(bar.ReceivedValue, "asdf");
            Assert.IsNull(barSpecial.ReceivedValue);

            bar.ReceivedValue = null;

            fooSpecial.DoSomething("zxcv");

            Assert.IsEqual(barSpecial.ReceivedValue, "zxcv");
            Assert.IsNull(bar.ReceivedValue);

            bar.Dispose();
            barSpecial.Dispose();
        }

        public class SomethingHappenedSignal : Signal<string>
        {
            public class Trigger : TriggerBase
            {
            }
        }

        public class Foo
        {
            readonly SomethingHappenedSignal.Trigger _trigger;

            public Foo(
                SomethingHappenedSignal.Trigger trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething(string value)
            {
                _trigger.Fire(value);
            }
        }

        public class Bar
        {
            readonly SomethingHappenedSignal _signal;

            public Bar(SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public string ReceivedValue
            {
                get;
                set;
            }

            public void Initialize()
            {
                _signal.Event += OnStarted;
            }

            public void Dispose()
            {
                _signal.Event -= OnStarted;
            }

            void OnStarted(string value)
            {
                ReceivedValue = value;
            }
        }

        public class FooSpecial
        {
            readonly SomethingHappenedSignal.Trigger _trigger;

            public FooSpecial(
                [Inject(Id = "special")]
                SomethingHappenedSignal.Trigger trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething(string value)
            {
                _trigger.Fire(value);
            }
        }

        public class BarSpecial
        {
            readonly SomethingHappenedSignal _signal;

            public BarSpecial(
                [Inject(Id = "special")]
                SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public string ReceivedValue
            {
                get;
                set;
            }

            public void Initialize()
            {
                _signal.Event += OnStarted;
            }

            public void Dispose()
            {
                _signal.Event -= OnStarted;
            }

            void OnStarted(string value)
            {
                ReceivedValue = value;
            }
        }
    }
}


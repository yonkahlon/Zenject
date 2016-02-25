using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Commands;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSignalsWithIdentifiers : TestWithContainer
    {
        [Test]
        public void RunTest()
        {
            Container.BindSignal<SomethingHappenedSignal>();
            Container.BindTrigger<SomethingHappenedSignal.Trigger>();

            Container.Bind<Foo>().ToSingle();
            Container.Bind<Bar>().ToSingle();

            Container.BindSignal<SomethingHappenedSignal>("special");
            Container.BindTrigger<SomethingHappenedSignal.Trigger>("special");

            Container.Bind<FooSpecial>().ToSingle();
            Container.Bind<BarSpecial>().ToSingle();

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
                [Inject("special")]
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
                [Inject("special")]
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


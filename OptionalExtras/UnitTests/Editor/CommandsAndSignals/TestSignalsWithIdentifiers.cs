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
    public class TestSignalsWithIdentifiers
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();
        }

        [Test]
        public void RunTest()
        {
            _container.BindSignal<SomethingHappenedSignal>();
            _container.BindTrigger<SomethingHappenedSignal.Trigger>();

            _container.Bind<Foo>().ToSingle();
            _container.Bind<Bar>().ToSingle();

            _container.BindSignal<SomethingHappenedSignal>("special");
            _container.BindTrigger<SomethingHappenedSignal.Trigger>("special");

            _container.Bind<FooSpecial>().ToSingle();
            _container.Bind<BarSpecial>().ToSingle();

            var foo = _container.Resolve<Foo>();
            var bar = _container.Resolve<Bar>();

            var fooSpecial = _container.Resolve<FooSpecial>();
            var barSpecial = _container.Resolve<BarSpecial>();

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


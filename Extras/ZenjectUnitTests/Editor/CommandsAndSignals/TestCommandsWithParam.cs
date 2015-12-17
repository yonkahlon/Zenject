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
    public class TestCommandsWithParam
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();
        }

        [Test]
        public void TestSingle()
        {
            _container.Bind<Bar>().ToSingle();
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand, string>()
                .HandleWithSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = _container.Resolve<Bar>();
            var foo = _container.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");
        }

        [Test]
        public void TestTransient()
        {
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand, string>().HandleWithTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = _container.Resolve<Foo>();

            Assert.IsEqual(Bar.Instances.Count, 0);
            foo.Trigger("asdf");
            Assert.IsEqual(Bar.Instances.Count, 1);

            var bar1 = Bar.Instances.Single();
            Assert.IsEqual(bar1.ReceivedValue, "asdf");

            bar1.ReceivedValue = null;
            foo.Trigger("zcxv");

            Assert.IsEqual(Bar.Instances.Count, 2);
            Assert.IsEqual(Bar.Instances.Last().ReceivedValue, "zcxv");
            Assert.IsNull(bar1.ReceivedValue);
        }

        public class DoSomethingCommand : Command<string>
        {
        }

        public class Foo
        {
            readonly DoSomethingCommand _doSomethingCommand;

            public Foo(DoSomethingCommand doSomethingCommand)
            {
                _doSomethingCommand = doSomethingCommand;
            }

            public void Trigger(string value)
            {
                _doSomethingCommand.Execute(value);
            }
        }


        public class Bar
        {
            public static List<Bar> Instances = new List<Bar>();

            public Bar()
            {
                Instances.Add(this);
            }

            public string ReceivedValue
            {
                get;
                set;
            }

            public void Execute(string value)
            {
                ReceivedValue = value;
            }
        }
    }
}


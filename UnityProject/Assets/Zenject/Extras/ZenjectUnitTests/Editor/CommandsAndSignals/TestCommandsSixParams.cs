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
    public class TestCommandsSixParam
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
            _container.BindCommand<DoSomethingCommand, string, int, float, string, int, float>()
                .HandleWithSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = _container.Resolve<Bar>();
            var foo = _container.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf", 5, 1.2f, "zxcv", 5, 123.0f);
            Assert.IsEqual(bar.ReceivedValue, "zxcv");
        }

        [Test]
        public void TestTransient()
        {
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand, string, int, float, string, int, float>().HandleWithTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = _container.Resolve<Foo>();

            Assert.IsEqual(Bar.Instances.Count, 0);
            foo.Trigger("zxcv", 5, 1.2f, "asdf", 5, 123.0f);
            Assert.IsEqual(Bar.Instances.Count, 1);

            var bar1 = Bar.Instances.Single();
            Assert.IsEqual(bar1.ReceivedValue, "asdf");

            bar1.ReceivedValue = null;
            foo.Trigger("asdf", 5, 1.2f, "zxcv", 5, 123.0f);

            Assert.IsEqual(Bar.Instances.Count, 2);
            Assert.IsEqual(Bar.Instances.Last().ReceivedValue, "zxcv");
            Assert.IsNull(bar1.ReceivedValue);
        }

        public class DoSomethingCommand : Command<string, int, float, string, int, float>
        {
        }

        public class Foo
        {
            readonly DoSomethingCommand _doSomethingCommand;

            public Foo(DoSomethingCommand doSomethingCommand)
            {
                _doSomethingCommand = doSomethingCommand;
            }

            public void Trigger(string value1, int value2, float value3, string value4, int value5, float value6)
            {
                _doSomethingCommand.Execute(value1, value2, value3, value4, value5, value6);
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

            public void Execute(string value1, int value2, float value3, string value4, int value5, float value6)
            {
                ReceivedValue = value4;
            }
        }
    }
}


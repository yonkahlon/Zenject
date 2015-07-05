using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Zenject;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;
using ModestTree.Util;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestTaskUpdater
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();

            _container.Bind<TaskUpdater<ITickable>>().ToSingleInstance(new TaskUpdater<ITickable>(t => t.Tick()));
        }

        public void BindTickable<TTickable>(int priority) where TTickable : ITickable
        {
            _container.Bind<ITickable>().ToSingle<TTickable>();
            _container.Bind<ModestTree.Util.Tuple<Type, int>>().ToInstance(ModestTree.Util.Tuple.New(typeof(TTickable), priority));
        }

        [Test]
        public void TestTickablesAreOptional()
        {
            Assert.That(_container.ValidateResolve<TaskUpdater<ITickable>>().IsEmpty());
            Assert.IsNotNull(_container.Resolve<TaskUpdater<ITickable>>());
        }

        [Test]
        // Test that tickables get called in the correct order
        public void TestOrder()
        {
            _container.Bind<Tickable1>().ToSingle();
            _container.Bind<Tickable2>().ToSingle();
            _container.Bind<Tickable3>().ToSingle();

            BindTickable<Tickable3>(2);
            BindTickable<Tickable1>(0);
            BindTickable<Tickable2>(1);

            Assert.That(_container.ValidateResolve<TaskUpdater<ITickable>>().IsEmpty());
            var kernel = _container.Resolve<TaskUpdater<ITickable>>();

            Assert.That(_container.ValidateResolve<Tickable1>().IsEmpty());
            var tick1 = _container.Resolve<Tickable1>();
            Assert.That(_container.ValidateResolve<Tickable2>().IsEmpty());
            var tick2 = _container.Resolve<Tickable2>();
            Assert.That(_container.ValidateResolve<Tickable3>().IsEmpty());
            var tick3 = _container.Resolve<Tickable3>();

            int tickCount = 0;

            tick1.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 0);
                tickCount++;
            };

            tick2.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 1);
                tickCount++;
            };

            tick3.TickCalled += delegate
            {
                Assert.IsEqual(tickCount, 2);
                tickCount++;
            };

            kernel.UpdateAll();
        }

        class Tickable1 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable2 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }

        class Tickable3 : ITickable
        {
            public event Action TickCalled = delegate {};

            public void Tick()
            {
                TickCalled();
            }
        }
    }
}

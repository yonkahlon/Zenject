using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject.TestFramework;

namespace Zenject.Tests.ToMonoBehaviour
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        [Test]
        public void TestBasic()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind<Foo>().FromComponent(gameObject).AsSingle();
            Container.BindRootResolve<Foo>();

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestTransient()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind<Foo>().FromComponent(gameObject).AsTransient();
            Container.Bind<IFoo>().To<Foo>().FromComponent(gameObject).AsTransient();

            Container.BindRootResolve(typeof(IFoo), typeof(Foo));

            FixtureUtil.AssertComponentCount<Foo>(Container, 2);
        }

        [Test]
        public void TestSingle()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind<Foo>().FromComponent(gameObject).AsSingle();
            Container.Bind<IFoo>().To<Foo>().FromComponent(gameObject).AsSingle();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<Foo>();
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestCached1()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind<Foo>().FromComponent(gameObject).AsCached();
            Container.Bind<IFoo>().To<Foo>().FromComponent(gameObject).AsCached();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<Foo>();
            FixtureUtil.AssertComponentCount<Foo>(Container, 2);
        }

        [Test]
        public void TestCached2()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>().FromComponent(gameObject).AsCached();

            Container.BindRootResolve<IFoo>();
            Container.BindRootResolve<Foo>();
            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
        }

        [Test]
        public void TestCachedMultipleConcrete()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind(typeof(IFoo), typeof(IBar))
                .To(new List<Type>() { typeof(Foo), typeof(Bar) }).FromComponent(gameObject).AsCached();

            Container.BindRootResolve(typeof(IFoo), typeof(IBar));

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        [Test]
        public void TestSingleMultipleConcrete()
        {
            var gameObject = Container.CreateEmptyGameObject("Foo");

            Container.BindInstance("Foo", gameObject);

            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() { typeof(Foo), typeof(Bar) })
                .FromComponent(gameObject).AsSingle();
            Container.Bind<IFoo2>().To<Foo>().FromComponent(gameObject).AsSingle();

            Container.BindRootResolve(typeof(IFoo), typeof(IFoo2), typeof(IBar));

            FixtureUtil.AssertComponentCount<Foo>(Container, 1);
            FixtureUtil.AssertComponentCount<Bar>(Container, 1);
        }

        public interface IBar
        {
        }

        public interface IFoo2
        {
        }

        public interface IFoo
        {
        }

        public class Foo : MonoBehaviour, IFoo, IBar, IFoo2
        {
        }

        public class Bar : MonoBehaviour, IFoo, IBar, IFoo2
        {
        }
    }
}

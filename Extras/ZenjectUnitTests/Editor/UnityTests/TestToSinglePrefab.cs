using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    // The test class FooMonoBehaviour aren't included in the project so this
    // won't run - left here to show usage
    //[TestFixture]
    //public class TestToSinglePrefab : TestWithContainer
    //{
        //GameObject _prefab;

        //public interface IFoo
        //{
        //}

        //public override void Setup()
        //{
            //base.Setup();

            //Container.AllowNullBindings = true;

            //Container.Bind<GameObjectInstantiator>().ToSingle();
            //Container.Bind<Transform>().To((Transform)null).WhenInjectedInto<GameObjectInstantiator>();

            //_prefab = Resources.Load<GameObject>("TestPrefab1");
        //}

        //public override void Destroy()
        //{
            //// Make sure to always destroy all temporary game objects!
            //var prefabMgr = Container.Resolve<PrefabSingletonProviderMap>();

            //foreach (var creator in prefabMgr.Creators)
            //{
                //GameObject.DestroyImmediate(creator.Prefab);
            //}

            //base.Destroy();
        //}

        //[Test]
        //public void TestBasic()
        //{
            //Container.Bind<FooMonoBehaviour1>().ToSinglePrefab(_prefab);

            //var foo = Container.Resolve<FooMonoBehaviour1>();
            //Assert.IsNotNull(foo);
        //}

        //[Test]
        //public void TestInterfaces()
        //{
            //Container.Bind<FooMonoBehaviour1>().ToSinglePrefab(_prefab);
            //Container.Bind<IFooMonoBehaviour1>().ToSinglePrefab(_prefab);

            //var foo1 = Container.Resolve<IFooMonoBehaviour1>();
            //var foo2 = Container.Resolve<FooMonoBehaviour1>();

            //Assert.IsNotNull(foo1);
            //Assert.IsEqual(foo1, foo2);
        //}

        //[Test]
        //public void TestSameIdentifierSameInstance()
        //{
            //Container.Bind<FooMonoBehaviour1>().ToSinglePrefab("foo1", _prefab);
            //Container.Bind<IFooMonoBehaviour1>().ToSinglePrefab("foo1", _prefab);

            //var foo1 = Container.Resolve<IFooMonoBehaviour1>();
            //var foo2 = Container.Resolve<FooMonoBehaviour1>();

            //Assert.IsNotNull(foo1);
            //Assert.IsEqual(foo1, foo2);
        //}

        //[Test]
        //public void TestDifferentIdentifierDifferentInstances()
        //{
            //Container.Bind<FooMonoBehaviour1>().ToSinglePrefab("foo1", _prefab);
            //Container.Bind<IFooMonoBehaviour1>().ToSinglePrefab("foo2", _prefab);

            //var foo1 = Container.Resolve<IFooMonoBehaviour1>();
            //var foo2 = Container.Resolve<FooMonoBehaviour1>();

            //Assert.IsNotNull(foo1);
            //Assert.IsNotNull(foo2);

            //Assert.IsNotEqual(foo1, foo2);
        //}

        //[Test]
        //public void TestMultipleComponentsOnSamePrefab()
        //{
            //Container.Bind<FooMonoBehaviour1>().ToSinglePrefab(_prefab);
            //Container.Bind<FooMonoBehaviour2>().ToSinglePrefab(_prefab);

            //var foo1 = Container.Resolve<FooMonoBehaviour1>();
            //var foo2 = Container.Resolve<FooMonoBehaviour2>();

            //Assert.IsNotNull(foo1);
            //Assert.IsNotNull(foo2);

            //Assert.IsEqual(foo1.transform.parent, foo2.transform.parent);

            //Assert.That(Container.ValidateResolve<FooMonoBehaviour1>().IsEmpty());
            //Assert.That(Container.ValidateResolve<FooMonoBehaviour2>().IsEmpty());
        //}

        //[Test]
        //public void TestValidationMissingComponent()
        //{
            //Container.Bind<FooMonoBehaviour3>().ToSinglePrefab(_prefab);

            //Assert.Throws<ZenjectResolveException>(delegate { Container.Resolve<FooMonoBehaviour3>(); });

            //Assert.That(!Container.ValidateResolve<FooMonoBehaviour3>().IsEmpty());
        //}
    //}
}


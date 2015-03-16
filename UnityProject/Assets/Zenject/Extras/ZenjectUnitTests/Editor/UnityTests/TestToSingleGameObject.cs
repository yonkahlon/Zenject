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
    //public class TestToSingleGameObject : TestWithContainer
    //{
        //public interface IFoo
        //{
        //}

        //public class FooMonoBehaviour : MonoBehaviour, IFoo
        //{
        //}

        //public override void Setup()
        //{
            //base.Setup();

            //Container.AllowNullBindings = true;

            //Container.Bind<GameObjectInstantiator>().ToSingle();
            //Container.Bind<Transform>().To((Transform)null).WhenInjectedInto<GameObjectInstantiator>();
        //}

        //[Test]
        //public void Test1()
        //{
            //Container.Bind<FooMonoBehaviour>().ToSingleGameObject();
            //Container.Bind<IFoo>().ToLookup<FooMonoBehaviour>();

            //var foo1 = Container.Resolve<FooMonoBehaviour>();
            //var foo2 = Container.Resolve<IFoo>();

            //Assert.IsEqual(foo1, foo2);

            //GameObject.DestroyImmediate(foo1.gameObject);
        //}
    //}
}

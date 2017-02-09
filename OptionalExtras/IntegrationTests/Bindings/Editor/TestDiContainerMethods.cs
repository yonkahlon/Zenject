using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.DiContainerMethods;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestDiContainerMethods : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return GetPrefab("Foo"); }
        }

        GameObject GorpPrefab
        {
            get { return GetPrefab("Gorp"); }
        }

        GameObject CameraPrefab
        {
            get { return GetPrefab("Camera"); }
        }

        [Test]
        public void InjectGameObject()
        {
            Initialize();

            var go = GameObject.Instantiate(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(!foo.WasInjected);
            Container.InjectGameObject(go);
            Assert.That(foo.WasInjected);
        }

        [Test]
        public void InjectGameObjectForMonoBehaviour()
        {
            Initialize();

            var go = GameObject.Instantiate(GorpPrefab);

            Assert.Throws(() => Container.InjectGameObject(go));

            var gorp = Container.InjectGameObjectForComponent<Gorp>(go, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void InjectGameObjectForComponent()
        {
            Initialize();

            var go = GameObject.Instantiate(CameraPrefab);

            Container.InjectGameObjectForComponent<Camera>(go, new object[0]);
        }

        [Test]
        public void InjectGameObjectForComponentMistake()
        {
            Initialize();

            var go = GameObject.Instantiate(CameraPrefab);

            Assert.Throws(() => Container.InjectGameObjectForComponent<Camera>(go, new object[] { "sdf" }));
        }

        [Test]
        public void InstantiatePrefab()
        {
            Initialize();

            var go = Container.InstantiatePrefab(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(foo.WasInjected);
        }

        [Test]
        public void InstantiatePrefabForMonoBehaviour()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiatePrefab(GorpPrefab));

            var gorp = Container.InstantiatePrefabForComponent<Gorp>(GorpPrefab, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void InstantiatePrefabForComponent()
        {
            Initialize();

            var camera = Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[0]);
            Assert.IsNotNull(camera);
        }

        [Test]
        public void InstantiatePrefabForComponentMistake()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[] { "sdf" }));
        }

        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab("TestDiContainerMethods/{0}".Fmt(name));
        }
    }
}


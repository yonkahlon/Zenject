#if !ZEN_NOT_UNITY3D

using NUnit.Framework;
using UnityEngine;
using Assert = ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestToSingleWrongMonoBehaviour : TestWithContainer
    {
        [ExpectedException]
        public void TestCase1()
        {
            Container.Bind<Foo>().ToSingle();
        }

        [ExpectedException]
        public void TestCase2()
        {
            Container.Bind<Bar>().ToSingleGameObject();
        }

        public class Foo : MonoBehaviour
        {
        }

        public class Bar
        {
        }
    }
}

#endif

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
            Binder.Bind<Foo>().ToSingle();
        }

        [ExpectedException]
        public void TestCase2()
        {
            Binder.Bind<Bar>().ToSingleGameObject();
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

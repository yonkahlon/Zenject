using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestDiContainer : TestWithContainer
    {
        [Test]
        public void TestSimple()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();
            Container.Bind<Bar>().ToSingle();

            AssertHasContracts(
                new List<Type>() { typeof(Bar), typeof(IFoo) });

            AssertHasConcreteTypes(
                new List<Type>() { typeof(Bar), typeof(Foo) });
        }

        void AssertHasConcreteTypes(IEnumerable<Type> expectedValues)
        {
            var concreteList = Container.AllConcreteTypes.ToList();
            var expectedList = GetStandardConcreteTypeInclusions().Concat(expectedValues).ToList();

            Assert.That(
                TestListComparer.ContainSameElements(
                    concreteList, expectedList),
                    "Unexpected list: " + TestListComparer.PrintList(concreteList) + "\nExpected: " + TestListComparer.PrintList(expectedList));
        }

        void AssertHasContracts(IEnumerable<Type> expectedValues)
        {
            var contractList = Container.AllContracts.Select(x => x.Type).ToList();
            var expectedList = GetStandardContractTypeInclusions().Concat(expectedValues).ToList();

            Assert.That(
                TestListComparer.ContainSameElements(
                    contractList, expectedList),
                    "Unexpected list: " + TestListComparer.PrintList(contractList) + "\nExpected: " + TestListComparer.PrintList(expectedList));
        }

        List<Type> GetStandardContractTypeInclusions()
        {
            return new List<Type>()
            {
                typeof(IInstantiator), typeof(DiContainer), typeof(IBinder), typeof(IResolver),
            };
        }

        List<Type> GetStandardConcreteTypeInclusions()
        {
            return new List<Type>()
            {
                typeof(DiContainer),
            };
        }

        [Test]
        public void TestComplex()
        {
            Container.Bind<IFoo>().ToSingle<Foo>();
            Container.Bind<IFoo>().ToSingle<Foo2>();

            Container.Bind<Bar>().ToInstance(new Bar());
            Container.Bind<Bar>().ToInstance(new Bar());

            AssertHasContracts(
                new List<Type>() { typeof(Bar), typeof(IFoo) });

            AssertHasConcreteTypes(
                new List<Type>() { typeof(Bar), typeof(Foo2), typeof(Foo) });
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        class Foo2 : IFoo
        {
        }

        class Bar
        {
        }
    }
}



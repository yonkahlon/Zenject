using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestAllInjectionTypes : TestWithContainer
    {
        [Test]
        // Test all variations of injection
        public void TestCase1()
        {
            Container.Bind<Test0>().To(new Test0());
            Container.Bind<IFoo>().ToSingle<FooDerived>();

            Assert.That(Container.ValidateResolve<IFoo>().IsEmpty());
            var foo = Container.Resolve<IFoo>();

            Assert.That(foo.DidPostInjectBase);
            Assert.That(foo.DidPostInjectDerived);
        }

        class Test0
        {
        }

        interface IFoo
        {
            bool DidPostInjectBase
            {
                get;
            }

            bool DidPostInjectDerived
            {
                get;
            }
        }

        abstract class FooBase : IFoo
        {
            bool _didPostInjectBase;

            [Inject]
            public static Test0 BaseStaticFieldPublic = null;

            [Inject]
            private static Test0 BaseStaticFieldPrivate = null;

            [Inject]
            protected static Test0 BaseStaticFieldProtected = null;

            [Inject]
            public static Test0 BaseStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private static Test0 BaseStaticPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected static Test0 BaseStaticPropertyProtected
            {
                get;
                set;
            }

            // Instance
            [Inject]
            public Test0 BaseFieldPublic = null;

            [Inject]
            private Test0 BaseFieldPrivate = null;

            [Inject]
            protected Test0 BaseFieldProtected = null;

            [Inject]
            public Test0 BasePropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private Test0 BasePropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 BasePropertyProtected
            {
                get;
                set;
            }

            [PostInject]
            public void PostInjectBase()
            {
                Assert.IsNull(BaseStaticFieldPublic);
                Assert.IsNull(BaseStaticFieldPrivate);
                Assert.IsNull(BaseStaticFieldProtected);
                Assert.IsNull(BaseStaticPropertyPublic);
                Assert.IsNull(BaseStaticPropertyPrivate);
                Assert.IsNull(BaseStaticPropertyProtected);

                Assert.IsNotNull(BaseFieldPublic);
                Assert.IsNotNull(BaseFieldPrivate);
                Assert.IsNotNull(BaseFieldProtected);
                Assert.IsNotNull(BasePropertyPublic);
                Assert.IsNotNull(BasePropertyPrivate);
                Assert.IsNotNull(BasePropertyProtected);

                _didPostInjectBase = true;
            }

            public bool DidPostInjectBase
            {
                get
                {
                    return _didPostInjectBase;
                }
            }

            public abstract bool DidPostInjectDerived
            {
                get;
            }
        }

        class FooDerived : FooBase
        {
            public bool _didPostInject;
            public Test0 ConstructorParam;

            public override bool DidPostInjectDerived
            {
                get
                {
                    return _didPostInject;
                }
            }

            [Inject]
            public static Test0 DerivedStaticFieldPublic = null;

            [Inject]
            private static Test0 DerivedStaticFieldPrivate = null;

            [Inject]
            protected static Test0 DerivedStaticFieldProtected = null;

            [Inject]
            public static Test0 DerivedStaticPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private static Test0 DerivedStaticPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected static Test0 DerivedStaticPropertyProtected
            {
                get;
                set;
            }

            // Instance
            public FooDerived(Test0 param)
            {
                ConstructorParam = param;
            }

            [PostInject]
            public void PostInject()
            {
                Assert.IsNull(DerivedStaticFieldPublic);
                Assert.IsNull(DerivedStaticFieldPrivate);
                Assert.IsNull(DerivedStaticFieldProtected);
                Assert.IsNull(DerivedStaticPropertyPublic);
                Assert.IsNull(DerivedStaticPropertyPrivate);
                Assert.IsNull(DerivedStaticPropertyProtected);

                Assert.IsNotNull(DerivedFieldPublic);
                Assert.IsNotNull(DerivedFieldPrivate);
                Assert.IsNotNull(DerivedFieldProtected);
                Assert.IsNotNull(DerivedPropertyPublic);
                Assert.IsNotNull(DerivedPropertyPrivate);
                Assert.IsNotNull(DerivedPropertyProtected);
                Assert.IsNotNull(ConstructorParam);

                _didPostInject = true;
            }

            [Inject]
            public Test0 DerivedFieldPublic = null;

            [Inject]
            private Test0 DerivedFieldPrivate = null;

            [Inject]
            protected Test0 DerivedFieldProtected = null;

            [Inject]
            public Test0 DerivedPropertyPublic
            {
                get;
                set;
            }

            [Inject]
            private Test0 DerivedPropertyPrivate
            {
                get;
                set;
            }

            [Inject]
            protected Test0 DerivedPropertyProtected
            {
                get;
                set;
            }
        }
    }
}



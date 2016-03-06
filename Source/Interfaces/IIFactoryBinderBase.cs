using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IIFactoryBinderBase<TContract>
    {
        // --------------------------------------------------
        // [ ToMethod ] - Create dynamic dependency from a method
        // --------------------------------------------------
        //
        //  Results in a dependency of type IFactory<TContract> that invokes the given method.
        //  Method must return a new instance of type TContract.
        //
        //  Examples:
        //
        //      Container.BindIFactory<IFoo>().ToMethod(MyMethod);
        //
        //      // Using a lambda:
        //      Container.BindIFactory<IFoo>().ToMethod(c => new Foo());
        //
        //      // With a parameter:
        //      Container.BindIFactory<string, IFoo>().ToMethod((text, c) => new Foo(text));
        //
        //BindingConditionSetter ToMethod(Func<DiContainer, TContract> method);

        // --------------------------------------------------
        // [ ToInstance<>  ] - Create dynamic dependency from an existing instance
        // --------------------------------------------------
        //
        // Results in a dependency of type IFactory<TContract> that just returns
        // the given instance
        //
        // Examples:
        //
        //    Container.BindIFactory<IFoo>().ToInstance(new Foo());
        //
        //BindingConditionSetter ToInstance(TContract instance);

        // --------------------------------------------------
        // [ ToFactory  ] - Create dynamic dependency for a concrete type
        // --------------------------------------------------
        //
        // Results in a dependency of type IFactory<TContract> that will create a new instance
        // of type TContract. TContract must be a concrete class in this case.
        //
        // Examples:
        //
        //      Container.BindIFactory<Foo>().ToFactory();
        //
        //      // With some parameters:
        //      Container.BindIFactory<string, int, int, Foo>().ToFactory();
        //
        //  In this example, any calls to IFactory<Foo>.Create() will return a new instance
        //  of type Foo. Note that of course requires that the generic argument to
        //  BindIFactory<> be non-abstract.
        //
        BindingConditionSetter ToFactory();

        // --------------------------------------------------
        // [ ToFactory<> ] - Create dynamic dependency for an abstract type
        // --------------------------------------------------
        //
        // Results in a dependency of type IFactory<TContract> that will create a new instance
        // of type TConcrete. TConcrete must derive from TContract in this case.
        //
        // Examples:
        //
        //      // Example:
        //      Container.BindIFactory<IFoo>().ToFactory<Foo>();
        //
        //      // With some parameters
        //      Container.BindIFactory<string, int, IFoo>().ToFactory<Foo>();
        //
        BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract;

        // --------------------------------------------------
        // [ ToIFactory  ] - Create dynamic dependency via lookup on another factory
        // --------------------------------------------------
        //
        // Results in a dependency of type IFactory<TContract> that will return an instance
        // of type TConcrete. It does this by looking up IFactory<TConcrete> and calling
        // Create() to create an instance of type TConcrete. TConcrete must derive from
        // TContract for this binding. Also, it is assumed that IFactory<TConcrete> is
        // declared in a separate binding.
        //
        // Examples:
        //
        //      // First create a simple binding for IFactory<Foo>
        //      Container.BindIFactory<Foo>().ToFactory();
        //
        //      // Now create a binding for IFactory<IFoo> that uses the above binding
        //      Container.BindIFactory<IFoo>().ToIFactory<Foo>();
        //
        BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract;

        // --------------------------------------------------
        // [ ToCustomFactory ] - Create dynamic dependency using user created factory class
        // --------------------------------------------------
        //
        //  Results in a dependency of type IFactory<TContract> that will return an instance
        //  of type TConcrete using the given factory of type TFactory. It is assumed that
        //  TFactory is declared in another binding. TFactory must also derive from
        //  IFactory<TConcrete> for this to work.
        //
        //  Examples:
        //
        //      // Map IFoo to our custom factory Foo.Factory
        //      Container.BindIFactory<IFoo>().ToCustomFactory<Foo, MyCustomFooFactory>();
        //
        //      public class MyCustomFooFactory : IFactory<IFoo>
        //      {
        //          ...
        //      }
        //
        //BindingConditionSetter ToCustomFactory<TFactory>();

#if !ZEN_NOT_UNITY3D

        // --------------------------------------------------
        // [ ToPrefab  ] - Create dynamic MonoBehaviour using given prefab
        // --------------------------------------------------
        //
        // Results in a dependency of type IFactory<TContract> that can be used to create
        // instances of the given prefab. After instantiating the given prefab, the
        // factory will search it for a component of type TContract and then will return
        // that from the Create() method. In this case, the TContract class must either be
        // an interface or derive from Component / MonoBehaviour.
        //
        // Examples:
        //
        //      Container.BindIFactory<IFoo>().ToPrefab(prefab);
        //
        BindingConditionSetter ToPrefab(GameObject prefab);
#endif

        // --------------------------------------------------
        // [ ToFacadeFactoryMethod ] - Bind IFactory to a facade factory and initialize facade
        // subcontainer using a method
        // --------------------------------------------------
        //
        //  Results in a dependency of type IFactory<TContract> that will return an instance
        //  of type TConcrete where TConcrete is inside a subcontainer, that is initialized using
        //  the given method.  NOTE: It is assumed that TContract is bound to something within
        //  the given method.
        //
        //  Examples:
        //
        //      Container.BindIFactory<FooFacade>().ToFacadeFactoryMethod<FooFacade.Factory>(InstallFooFacade);
        //
        //      void InstallFooFacade(DiContainer subContainer)
        //      {
        //          subContainer.Bind<FooFacade>().ToSingle();
        //      }
        //
        //BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer> facadeInstaller)
            //where TFactory : IFactory<TContract>, IFacadeFactory;

        // --------------------------------------------------
        // [ ToFacadeFactoryInstaller ] - Bind IFactory to a facade factory and initialize facade
        // subcontainer using an installer
        // --------------------------------------------------
        //
        //  Results in a dependency of type IFactory<TContract> that will return an instance
        //  of type TConcrete where TConcrete is "inside" a subcontainer, with the subcontainer
        //  initialized using the given installer.
        //
        //  NOTE: It is assumed that TContract is bound to something within the given installer.
        //
        //  Examples:
        //
        //      Container.BindIFactory<FooFacade>().ToFacadeFactoryInstaller<FooFacade.Factory, FooInstaller>()
        //
        //      public class FooFacade
        //      {
        //          public class Factory : FacadeFactory<FooFacade>
        //          {
        //          }
        //      }
        //
        //      public class FooInstaller : Installer
        //      {
        //          public override void InstallBindings()
        //          {
        //              Container.Bind<FooFacade>().ToSingle();
        //              ...
        //          }
        //      }
        //
        //BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            //where TFactory : IFactory<TContract>, IFacadeFactory
            //where TInstaller : Installer;
    }
}





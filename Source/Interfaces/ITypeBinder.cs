using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface ITypeBinder
    {
        // --------------------------------------------------
        // [ ToSingle ] - Inject as singleton
        // --------------------------------------------------
        //
        //     Container.Bind<Foo>().ToSingle();
        //
        //  When a type is bound using ToSingle this will construct one and only
        //  one instance of Foo and use that everywhere that has Foo as a dependency
        //
        //  For example, given the following:
        //
        //     Container.Bind<IFoo>().ToSingle<Foo>();
        //     Container.Bind<IBar>().ToSingle<Foo>();
        //
        //  This will cause any dependencies of type IFoo or IBar to use the same instance of Foo.
        //  Of course, Foo must implement both IFoo and IBar for this to compile.
        //
        //  Note also that with only the above two lines the Foo singleton will not be accessible
        //  directly. You can achieve this by using another line to uses ToSingle directly:
        //
        //     Container.Bind<Foo>().ToSingle();
        //     Container.Bind<IFoo>().ToSingle<Foo>();
        //     Container.Bind<IBar>().ToSingle<Foo>();
        //
        //  Note again that the same instance will be used for all dependencies that
        //  take Foo, IFoo, or IBar.
        BindingConditionSetter ToSingle();
        BindingConditionSetter ToSingle(string concreteIdentifier);
        BindingConditionSetter ToSingle(Type concreteType);
        BindingConditionSetter ToSingle(Type concreteType, string concreteIdentifier);

        // --------------------------------------------------
        // [ ToInstance ] - Inject as instance
        // --------------------------------------------------
        //
        //  Examples:
        //
        //     Container.Bind<Foo>().ToInstance(new Foo());
        //     Container.Bind<string>().ToInstance("foo");
        //     Container.Bind<int>().ToInstance(42);
        //
        //  Or with shortcut:
        //     Container.BindInstance(new Bar());
        //
        //  In this case the given instance will be used for every dependency with the given type
        BindingConditionSetter ToInstance(Type concreteType, object instance);

        // --------------------------------------------------
        // [ ToTransient ] - Inject as newly created object
        // --------------------------------------------------
        //
        //    Container.Bind<Foo>().ToTransient();
        //
        //  In this case a new instance of Foo will be generated each time it is injected.
        //  Similar to ToSingle, you can bind via an interface as well:
        //
        //    Container.Bind<IFoo>().ToTransient<Foo>();
        //
        BindingConditionSetter ToTransient();
        BindingConditionSetter ToTransient(ContainerTypes containerType);

        BindingConditionSetter ToTransient(Type concreteType);
        BindingConditionSetter ToTransient(Type concreteType, ContainerTypes containerType);

#if !ZEN_NOT_UNITY3D
        // --------------------------------------------------
        // [ ToSinglePrefab ] - Inject by instantiating a single unity prefab
        // --------------------------------------------------
        //
        //   Container.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //
        //  This will instantiate a new instance of the given prefab, and then search the newly
        //  created game object for the given component (in this case FooMonoBehaviour).
        //
        //  Also, because it is ToSingle it will only instantiate the prefab once, and otherwise
        //  use the same instance of FooMonoBehaviour every time it's used
        //
        //  You can also bind multiple singletons to the same prefab. For example:
        //
        //    Container.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //    Container.Bind<BarMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //
        //  This will result in the prefab PrefabGameObject being instantiated once, and then
        //  searched for MonoBehaviour's FooMonoBehaviour and BarMonoBehaviour
        //
        BindingConditionSetter ToSinglePrefab(GameObject prefab);

        BindingConditionSetter ToSinglePrefab(string concreteIdentifier, GameObject prefab);

        BindingConditionSetter ToSinglePrefab(
            Type concreteType, string concreteIdentifier, GameObject prefab);

        // --------------------------------------------------
        // [ ToTransientPrefab ] - Inject by instantiating a unity prefab
        // --------------------------------------------------
        //
        //     Container.Bind<FooMonoBehaviour>().ToTransientPrefab(PrefabGameObject);
        //
        //  This works similar to ToSinglePrefab except it will instantiate a new instance of
        //  the given prefab every time the dependency is injected.
        //
        BindingConditionSetter ToTransientPrefab(GameObject prefab);
        BindingConditionSetter ToTransientPrefab(GameObject prefab, ContainerTypes containerType);

        BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab);
        BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab, ContainerTypes containerType);

        // --------------------------------------------------
        // [ ToSinglePrefabResource ] - Load singleton prefab via resources folder
        // --------------------------------------------------
        //
        //  Same as ToSinglePrefab except loads the prefab using a path in Resources folder
        //
        //  Container.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //
        //  In this example, I've placed my prefab at Assets/Resources/MyDirectory/MyPrefab.prefab.
        //  By doing this I don't have to pass in a GameObject and can refer to it by the path
        //  within the resources folder.
        //
        //  Note that you can re-use the same singleton instance for multiple monobehaviours that
        //  exist on the prefab.
        //
        //  Container.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //  Container.Bind<BarMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //
        //  In the above example, the prefab will only be instantiated once.
        //
        BindingConditionSetter ToSinglePrefabResource(
            Type concreteType, string concreteIdentifier, string resourcePath);

        BindingConditionSetter ToSinglePrefabResource(string resourcePath);

        BindingConditionSetter ToSinglePrefabResource(
            string concreteIdentifier, string resourcePath);

        // --------------------------------------------------
        // [ ToTransientPrefabResource ] - Load prefab via resources folder
        // --------------------------------------------------
        //
        // Same as ToSinglePrefabResource (see above) except instantiates a new prefab
        // each time it is used
        //
        BindingConditionSetter ToTransientPrefabResource(string resourcePath);
        BindingConditionSetter ToTransientPrefabResource(string resourcePath, ContainerTypes containerType);

        BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath);
        BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath, ContainerTypes containerType);

        // --------------------------------------------------
        // [ ToSingleGameObject ] - Inject by instantiating a new game object and
        // using that everywhere
        // --------------------------------------------------
        //
        //  Container.Bind<FooMonoBehaviour>().ToSingleGameObject();
        //
        //  This binding will create a new game object and attach the given FooMonoBehaviour.
        //
        //  Also note that since it is ToSingle that it will use the same instance everywhere
        //  that has FooMonoBehaviour as a dependency
        //
        //  Multiple bindings can be made to the same game object like so:
        //
        //    Container.Bind<FooMonoBehaviour>().ToSingleGameObject();
        //    Container.Bind<IFoo>().ToSingleGameObject<FooMonoBehaviour>();
        //
        //  In this case, dependencies of IFoo and FooMonoBehaviour will use the same instance
        BindingConditionSetter ToSingleGameObject();
        BindingConditionSetter ToSingleGameObject(string concreteIdentifier);

        BindingConditionSetter ToSingleGameObject(Type concreteType);
        BindingConditionSetter ToSingleGameObject(Type concreteType, string concreteIdentifier);

        // --------------------------------------------------
        // [ ToTransientGameObject ] - Inject by instantiating a new game object
        // --------------------------------------------------
        //
        // Same as ToSingleGameObject (see above) except instantiates a new prefab
        // each time it is used
        //
        BindingConditionSetter ToTransientGameObject();
        BindingConditionSetter ToTransientGameObject(ContainerTypes containerType);

        BindingConditionSetter ToTransientGameObject(Type concreteType);
        BindingConditionSetter ToTransientGameObject(Type concreteType, ContainerTypes containerType);

        // --------------------------------------------------
        // [ ToSingleMonoBehaviour ] - Inject by instantiating a new component on
        //  an existing GameObject
        // --------------------------------------------------
        //
        //   Container.Bind<FooMonoBehaviour>().ToSingleMonoBehaviour(MyGameObject);
        //
        //  This line will lazily add the FooMonoBehaviour MonoBehaviour to the given game
        //  object, and then re-use that same MonoBehaviour every time FooMonoBehaviour
        //  is asked for
        //
        BindingConditionSetter ToSingleMonoBehaviour(GameObject gameObject);
        BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, GameObject gameObject);

        BindingConditionSetter ToSingleMonoBehaviour(Type concreteType, GameObject gameObject);
        BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject);

        // --------------------------------------------------
        // [ ToResource ] - Inject using Resources.Load
        // --------------------------------------------------
        //
        //     Container.Bind<Texture>().ToResource("TestTexture")
        //
        //  This binding simply calls Resources.Load with the given resource path and type.  See
        //  unity docs for details on how to use Resources.Load(path, type)
        //
        //  Note that it calls Resources.Load each time it is used.  There is no single and
        //  transient vesions because in most cases Unity returns the same instance already
        //
        BindingConditionSetter ToResource(string resourcePath);
        BindingConditionSetter ToResource(Type concreteType, string resourcePath);
#endif

        // --------------------------------------------------
        // [ ToMethod ] - Inject by custom method
        // --------------------------------------------------
        //
        //  This binding allows you to customize creation logic yourself by defining a method.
        //  For example:
        //
        //  Container.Bind<IFoo>().ToMethod(SomeMethod);
        //
        //  public IFoo SomeMethod(InjectContext context)
        //  {
        //      ...
        //      return new Foo();
        //  }
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        // [ ToSingleMethod ] - Inject using a custom method but only call that method once
        // --------------------------------------------------
        //
        //  This binding works similar to ToMethod except that the given method will only be
        //  called once. The value returned from the method will then be used for every
        //  subsequent request for the given dependency.
        //
        //  Example:
        //
        //  Container.Bind<IFoo>().ToSingleMethod(SomeMethod);
        //
        //  public IFoo SomeMethod(InjectContext context)
        //  {
        //      ...
        //      return new Foo();
        //  }
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        // [ ToGetter ] - Inject by getter.
        // --------------------------------------------------
        //
        //  This method can be useful if you want to bind to a property of another object.
        //
        //  Example:
        //
        //  Container.Bind<IFoo>().ToSingle<Foo>()
        //  Container.Bind<Bar>().ToGetter<IFoo>(x => x.GetBar())
        //
        //  Note here that it gets IFoo by doing a recursive lookup on the container for whatever
        //  value is bound to <IFoo>
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        // [ ToResolve ] - Inject by recursive resolve
        // --------------------------------------------------
        //
        //  Examples
        //
        //    Container.Bind<IFoo>().ToResolve<IBar>()
        //    Container.Bind<IBar>().ToResolve<Foo>()
        //
        //  In some cases it is useful to be able to bind an interface to another interface.
        //  However, you cannot use ToSingle or ToTransient because they both require concrete
        //  types.
        //
        //  In the example code above we assume that Foo inherits from IBar, which inherits
        //  from IFoo. The result here will be that all dependencies for IFoo will be bound
        //  to whatever IBar is bound to (in this case, Foo).
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        // [ ToSingleInstance ] - Treat the given instance as a singleton.
        // --------------------------------------------------
        //
        //  This is the same as ToInstance except it will ensure that there is only ever one
        //  instance for the given type.
        //
        //  When using ToInstance you can do the following:
        //
        //  Container.Bind<Foo>().ToInstance(new Foo());
        //  Container.Bind<Foo>().ToInstance(new Foo());
        //  Container.Bind<Foo>().ToInstance(new Foo());
        //
        //  Or, equivalently:
        //
        //  Container.BindInstance(new Foo());
        //  Container.BindInstance(new Foo());
        //  Container.BindInstance(new Foo());
        //
        //  And then have a class that takes all of them as a list like this:
        //
        //  public class Bar
        //  {
        //      public Bar(List<Foo> foos)
        //      {
        //      }
        //  }
        //
        //  Whereas, if you use ToSingleInstance this would trigger an error.
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        // [ ToSingleFactory ] - Define a custom factory for a singleton
        // --------------------------------------------------
        //
        //  Example:
        //
        //  Container.Bind<IFoo>().ToSingleFactory<MyCustomFactory>();
        //
        //  class MyCustomFactory : IFactory<IFoo>
        //  {
        //      Bar _bar;
        //
        //      public MyCustomFactory(Bar bar)
        //      {
        //          _bar = bar;
        //      }
        //
        //      public IFoo Create()
        //      {
        //          ...
        //      }
        //  }
        //
        //  The ToSingleFactory binding can be useful when you want to define a singleton,
        //  but it has complex construction logic that you want to define yourself. You could
        //  use ToSingleMethod, but this can get ugly if your construction logic itself has
        //  its own dependencies that it needs. Using ToSingleFactory for this case it is nice
        //  because any dependencies that you require for construction can be simply added to
        //  the factory constructor
        //
        // (See derived classes for function signature)

        // --------------------------------------------------
        //  [ ToSingleFacadeMethod ] - Initialize a new sub container using the given method
        //  and use the given class as the facade for it
        // --------------------------------------------------
        //
        //  Example:
        //
        //     Container.Bind<FooFacade>().ToSingleFacadeMethod(InstallFooFacade)
        //
        //     void InstallFooFacade(DiContainer container)
        //     {
        //          container.Bind<FooFacade>().ToSingle();
        //          ...
        //     }
        //
        //  Note that the installer method MUST bind the given facade class to something
        //  In the example above, FooFacade is bound ToSingle
        //
        //  This binding can be useful if you have a sub-system with a related set of dependencies
        //  which don't need to be in the main container, and which can be interacted with
        //  entirely through the given facade class (google Facade Pattern)
        BindingConditionSetter ToSingleFacadeMethod(Action<DiContainer> installerFunc);

        BindingConditionSetter ToSingleFacadeMethod(
            string concreteIdentifier, Action<DiContainer> installerFunc);

        BindingConditionSetter ToSingleFacadeMethod(
            Type concreteType, string concreteIdentifier, Action<DiContainer> installerFunc);

        // --------------------------------------------------
        //  [ ToSingleFacadeInstaller ] - Initialize a new sub container using the given installer
        //  type and use the given class as the facade for it
        // --------------------------------------------------
        //
        //  Example:
        //
        //     Container.Bind<FooFacade>().ToSingleFacadeInstaller<FooInstaller>()
        //
        //     class FooInstaller : Installer
        //     {
        //         public override void InstallBindings()
        //         {
        //             Container.Bind<FooFacade>().ToSingle();
        //             ...
        //         }
        //     }
        //
        //  Note that the installer MUST bind the given facade class to something
        //  In the example above, FooFacade is bound ToSingle
        //
        //  This binding can be useful if you have a sub-system with a related set of dependencies
        //  which don't need to be in the main container, and which can be interacted with
        //  entirely through the given facade class (google Facade Pattern)
        BindingConditionSetter ToSingleFacadeInstaller<TInstaller>()
            where TInstaller : Installer;

        BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            string concreteIdentifier)
            where TInstaller : Installer;

        BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            Type concreteType, string concreteIdentifier)
            where TInstaller : Installer;

        BindingConditionSetter ToSingleFacadeInstaller(Type installerType);

        BindingConditionSetter ToSingleFacadeInstaller(
            string concreteIdentifier, Type installerType);

        BindingConditionSetter ToSingleFacadeInstaller(
            Type concreteType, string concreteIdentifier, Type installerType);
    }
}


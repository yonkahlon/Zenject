using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface ITypeBinder
    {
        // _____ ToSingle _____  - Inject as singleton
        //
        //     Binder.Bind<Foo>().ToSingle();
        //
        //  When a type is bound using ToSingle this will construct one and only
        //  one instance of Foo and use that everywhere that has Foo as a dependency
        //
        //  For example, given the following:
        //
        //     Binder.Bind<IFoo>().ToSingle<Foo>();
        //     Binder.Bind<IBar>().ToSingle<Foo>();
        //
        //  This will cause any dependencies of type IFoo or IBar to use the same instance of Foo.
        //  Of course, Foo must implement both IFoo and IBar for this to compile.
        //
        //  Note also that with only the above two lines the Foo singleton will not be accessible
        //  directly. You can achieve this by using another line to uses ToSingle directly:
        //
        //     Binder.Bind<Foo>().ToSingle();
        //     Binder.Bind<IFoo>().ToSingle<Foo>();
        //     Binder.Bind<IBar>().ToSingle<Foo>();
        //
        //  Note again that the same instance will be used for all dependencies that
        //  take Foo, IFoo, or IBar.
        BindingConditionSetter ToSingle();

        BindingConditionSetter ToSingle(string concreteIdentifier);

        BindingConditionSetter ToSingle(Type concreteType);

        BindingConditionSetter ToSingle(Type concreteType, string concreteIdentifier);

        // _____ ToInstance _____  - Inject as instance
        //
        //  Examples:
        //
        //     Binder.Bind<Foo>().ToInstance(new Foo());
        //     Binder.Bind<string>().ToInstance("foo");
        //     Binder.Bind<int>().ToInstance(42);
        //
        //  Or with shortcut:
        //     Binder.BindInstance(new Bar());
        //
        //  In this case the given instance will be used for every dependency with the given type
        BindingConditionSetter ToInstance(Type concreteType, object instance);

        // _____ ToTransient _____  - Inject as newly created object
        //
        //    Binder.Bind<Foo>().ToTransient();
        //
        //  In this case a new instance of Foo will be generated each time it is injected.
        //  Similar to ToSingle, you can bind via an interface as well:
        //
        //    Binder.Bind<IFoo>().ToTransient<Foo>();
        //
        BindingConditionSetter ToTransient();

        BindingConditionSetter ToTransient(Type concreteType);

#if !ZEN_NOT_UNITY3D
        // _____ ToSinglePrefab _____  - Inject by instantiating a single unity prefab
        //
        //   Binder.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //
        //  This will instantiate a new instance of the given prefab, and then search the newly
        //  created game object for the given component (in this case FooMonoBehaviour).
        //
        //  Also, because it is ToSingle it will only instantiate the prefab once, and otherwise
        //  use the same instance of FooMonoBehaviour every time it's used
        //
        //  You can also bind multiple singletons to the same prefab. For example:
        //
        //    Binder.Bind<FooMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //    Binder.Bind<BarMonoBehaviour>().ToSinglePrefab(PrefabGameObject);
        //
        //  This will result in the prefab PrefabGameObject being instantiated once, and then
        //  searched for MonoBehaviour's FooMonoBehaviour and BarMonoBehaviour
        //
        BindingConditionSetter ToSinglePrefab(GameObject prefab);

        BindingConditionSetter ToSinglePrefab(string concreteIdentifier, GameObject prefab);

        BindingConditionSetter ToSinglePrefab(
            Type concreteType, string concreteIdentifier, GameObject prefab);

        // _____ ToTransientPrefab  _____  - Inject by instantiating a unity prefab
        //
        //     Binder.Bind<FooMonoBehaviour>().ToTransientPrefab(PrefabGameObject);
        //
        //  This works similar to ToSinglePrefab except it will instantiate a new instance of
        //  the given prefab every time the dependency is injected.
        //
        BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab);

        BindingConditionSetter ToTransientPrefab(GameObject prefab);

        // _____ ToSinglePrefabResource  _____  - Load singleton prefab via resources folder
        //
        //  Same as ToSinglePrefab except loads the prefab using a path in Resources folder
        //
        //  Binder.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //
        //  In this example, I've placed my prefab at Assets/Resources/MyDirectory/MyPrefab.prefab.
        //  By doing this I don't have to pass in a GameObject and can refer to it by the path
        //  within the resources folder.
        //
        //  Note that you can re-use the same singleton instance for multiple monobehaviours that
        //  exist on the prefab.
        //
        //  Binder.Bind<FooMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //  Binder.Bind<BarMonoBehaviour>().ToSinglePrefabResource("MyDirectory/MyPrefab");
        //
        //  In the above example, the prefab will only be instantiated once.
        //
        BindingConditionSetter ToSinglePrefabResource(
            Type concreteType, string concreteIdentifier, string resourcePath);

        BindingConditionSetter ToSinglePrefabResource(string resourcePath);

        BindingConditionSetter ToSinglePrefabResource(
            string concreteIdentifier, string resourcePath);

        // _____ ToTransientPrefabResource  _____  - Load prefab via resources folder
        //
        // Same as ToSinglePrefabResource (see above) except instantiates a new prefab
        // each time it is used
        //
        BindingConditionSetter ToTransientPrefabResource(string resourcePath);
        BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath);

        // _____ ToSingleGameObject  _____  - Inject by instantiating a new game object and
        // using that everywhere
        //
        //  Binder.Bind<FooMonoBehaviour>().ToSingleGameObject();
        //
        //  This binding will create a new game object and attach the given FooMonoBehaviour.
        //
        //  Also note that since it is ToSingle that it will use the same instance everywhere
        //  that has FooMonoBehaviour as a dependency
        //
        //  Multiple bindings can be made to the same game object like so:
        //
        //    Binder.Bind<FooMonoBehaviour>().ToSingleGameObject();
        //    Binder.Bind<IFoo>().ToSingleGameObject<FooMonoBehaviour>();
        //
        //  In this case, dependencies of IFoo and FooMonoBehaviour will use the same instance
        BindingConditionSetter ToSingleGameObject();
        BindingConditionSetter ToSingleGameObject(string concreteIdentifier);

        BindingConditionSetter ToSingleGameObject(Type concreteType);
        BindingConditionSetter ToSingleGameObject(Type concreteType, string concreteIdentifier);

        // _____ ToTransientGameObject  _____  - Inject by instantiating a new game object
        //
        // Same as ToSingleGameObject (see above) except instantiates a new prefab
        // each time it is used
        //
        BindingConditionSetter ToTransientGameObject();

        BindingConditionSetter ToTransientGameObject(Type concreteType);

        // _____ ToSingleMonoBehaviour  _____  - Inject by instantiating a new component on
        //  an existing GameObject
        //
        //   Binder.Bind<FooMonoBehaviour>().ToSingleMonoBehaviour(MyGameObject);
        //
        //  This line will lazily add the FooMonoBehaviour MonoBehaviour to the given game
        //  object, and then re-use that same MonoBehaviour every time FooMonoBehaviour
        //  is asked for
        //
        BindingConditionSetter ToSingleMonoBehaviour(GameObject gameObject);

        BindingConditionSetter ToSingleMonoBehaviour(Type concreteType, GameObject gameObject);

        BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject);

        // _____ ToResource  _____  - Inject using Resources.Load
        //
        //     Binder.Bind<Texture>().ToResource("TestTexture")
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

        // _____ ToProvider  _____
        //
        //   This is mostly for internal use within Zenject.  It isn't recommended for use.
        //
        BindingConditionSetter ToProvider(ProviderBase provider);

        // _____ ToSingleFacadeMethod _____
        //
        BindingConditionSetter ToSingleFacadeMethod(Action<IBinder> installerFunc);

        BindingConditionSetter ToSingleFacadeMethod(
            string concreteIdentifier, Action<IBinder> installerFunc);

        BindingConditionSetter ToSingleFacadeMethod(
            Type concreteType, string concreteIdentifier, Action<IBinder> installerFunc);

        // _____ ToSingleFacadeInstaller _____
        //
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




using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IUntypedBinder : ITypeBinder
    {
        // _____ ToSingle _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSingle<TConcrete>();
        BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier);

        // _____ ToInstance _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToInstance<TConcrete>(TConcrete instance);

        // _____ ToTransient _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToTransient<TConcrete>();

#if !ZEN_NOT_UNITY3D
        // _____ ToSinglePrefab _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab);
        BindingConditionSetter ToSinglePrefab<TConcrete>(string identifier, GameObject prefab);

        // _____ ToTransientPrefab _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab);

        // _____ ToSinglePrefabResource _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath);
        BindingConditionSetter ToSinglePrefabResource<TConcrete>(
            string identifier, string resourcePath);

        // _____ ToTransientPrefabResource _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath);

        // _____ ToSingleGameObject _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSingleGameObject<TConcrete>()
            where TConcrete : Component;

        BindingConditionSetter ToSingleGameObject<TConcrete>(string concreteIdentifier)
            where TConcrete : Component;

        // _____ ToTransientGameObject _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToTransientGameObject<TConcrete>()
            where TConcrete : Component;

        // _____ ToSingleMonoBehaviour _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject);
        BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(string concreteIdentifier, GameObject gameObject);

        // _____ ToResource _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToResource<TConcrete>(string resourcePath);
#endif

        // _____ ToMethod _____  - Inject by custom method
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
        BindingConditionSetter ToMethod(Type returnType, Func<InjectContext, object> method);
        BindingConditionSetter ToMethod<TContract>(Func<InjectContext, TContract> method);

        // _____ ToResolve _____  - Inject by recursive resolve
        //
        //  Examples
        //
        //    Container.Bind<IFoo>().ToLookup<IBar>()
        //    Container.Bind<IBar>().ToLookup<Foo>()
        //
        //  In some cases it is useful to be able to bind an interface to another interface.
        //  However, you cannot use ToSingle or ToTransient because they both require concrete
        //  types.
        //
        //  In the example code above we assume that Foo inherits from IBar, which inherits
        //  from IFoo. The result here will be that all dependencies for IFoo will be bound
        //  to whatever IBar is bound to (in this case, Foo).
        //
        BindingConditionSetter ToResolve<TConcrete>();
        BindingConditionSetter ToResolve<TConcrete>(string identifier);

        // _____ ToGetter _____  - Inject by getter.
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
        BindingConditionSetter ToGetter<TObj, TContract>(Func<TObj, TContract> method);
        BindingConditionSetter ToGetter<TObj, TContract>(
            string identifier, Func<TObj, TContract> method);

        // _____ ToSingleInstance _____  - Treat the given instance as a singleton.
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
        BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance);
        BindingConditionSetter ToSingleInstance<TConcrete>(
            string concreteIdentifier, TConcrete instance);

        // _____ ToSingleMethod _____  - Inject using a custom method but only call that method once
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
        //
        BindingConditionSetter ToSingleMethod<TConcrete>(
            string concreteIdentifier, Func<InjectContext, TConcrete> method);

        BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method);

        // _____ ToSingleFacadeMethod _____
        //
        //  See description in ITypeBinder
        //
        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(Action<DiContainer> installerFunc);

        BindingConditionSetter ToSingleFacadeMethod<TConcrete>(
            string concreteIdentifier, Action<DiContainer> installerFunc);
    }
}




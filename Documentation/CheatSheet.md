
## <a id="cheatsheet"></a>Installers Cheat-Sheet

Below are a bunch of randomly assorted examples of bindings that you might include in one of your installers.

For more examples, you may also be interested in reading some of the Unit tests (see `Zenject/OptionalExtras/UnitTests` and `Zenject/OptionalExtras/IntegrationTests` directories)

```csharp

///////////// AsTransient

// Create a new instance of Foo for every class that asks for it
Container.Bind<Foo>().AsTransient();

// Create a new instance of Foo for every class that asks for an IFoo
Container.Bind<IFoo>().To<Foo>().AsTransient();

// Non generic versions
Container.Bind(typeof(IFoo)).AsTransient();
Container.Bind(typeof(IFoo)).To(typeof(Foo)).AsTransient();

///////////// ToSingle

// Create one definitive instance of Foo and re-use that for every class that asks for it
Container.Bind<Foo>().AsSingle();

// Create one definitive instance of Foo and re-use that for every class that asks for IFoo
Container.Bind<IFoo>().To<Foo>().AsSingle();

// In this example, the same instance of Foo will be used for all three cases
Container.Bind<Foo>().AsSingle();
Container.Bind<IFoo>().To<Foo>().AsSingle();
Container.Bind<IBar>().To<Foo>().AsSingle();

// Non generic versions
Container.Bind(typeof(Foo)).AsSingle();
Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

///////////// BindAllInterfaces

// Bind all interfaces that Foo implements to a new singleton of type Foo
Container.BindAllInterfaces<Foo>().To<Foo>().AsSingle();
// So for example if Foo implements ITickable and IInitializable then the above
// line is equivalent to this:
Container.Bind<ITickable>().To<Foo>().AsSingle();
Container.Bind<IInitializable>().To<Foo>().AsSingle();

///////////// ToInstance

// Use the given instance everywhere that Foo is used
Container.Bind<Foo>().ToInstance(new Foo());

// This is simply a shortcut for the above binding
// This can be a bit nicer since the type argument can be deduced from the parameter
Container.BindInstance(new Foo());

// Note that ToInstance is different from ToSingle because it does allow multiple bindings
// and you can't re-use the same instance in multiple bindings like you can with ToSingle
// For example, the following is allowed and will match any constructor parameters of type List<Foo>
// (and throw an exception for parameters that ask for a single Foo)
Container.Bind<Foo>().ToInstance(new Foo());
Container.Bind<Foo>().ToInstance(new Foo());

///////////// ToSingleInstance

// Use the given instance everywhere Foo is requested and ensure that it is the only Foo that is created
Container.Bind<Foo>().ToSingleInstance(new Foo());

// We assume here that Foo implements both IFoo and IBar
// This will result in the given instance of Foo used for all three cases
Container.Bind<IFoo>().ToSingleInstance(new Foo());
Container.Bind<IBar>().To<Foo>().AsSingle();
Container.Bind<Foo>().AsSingle();

///////////// Binding primitive types

// Use the number 10 every time an int is requested
// You'd never really want to do this, you should almost always use a When condition for primitive values (see conditions section below)
Container.Bind<int>().ToInstance(10);
Container.Bind<bool>().ToInstance(false);

// These are the same as above
// This can be a bit nicer though since the type argument can be deduced from the parameter
// Again though, be careful to use conditions to limit the scope of usage for values
// or consider using a Settings object as described above
Container.BindInstance(10);
Container.BindInstance(false);

///////////// ToMethod

// Create instance of Foo when requested, using the given method
// Note that for more complex construction scenarios, you might consider using a factory
// instead
Container.Bind<Foo>().ToMethod(GetFoo);

Foo GetFoo(InjectContext ctx)
{
    return new Foo();
}

// Randomly return one of several different implementations of IFoo
// We use Instantiate here instead of just new so that Foo1 gets its members injected
Container.Bind<IFoo>().ToMethod(GetFoo);

IFoo GetFoo(InjectContext ctx)
{
    switch (Random.Range(0, 3))
    {
        case 0:
            return ctx.Container.Instantiate<Foo1>();

        case 1:
            return ctx.Container.Instantiate<Foo2>();
    }

    return ctx.Container.Instantiate<Foo3>();
}

// Using lambda syntax
Container.Bind<Foo>().ToMethod((ctx) => new Foo());

// This is equivalent to AsTransient
Container.Bind<Foo>().ToMethod((ctx) => ctx.Container.Instantiate<Foo>());

///////////// ToGetter

// Bind to a property on another dependency
// This can be helpful to reduce coupling between classes
Container.Bind<Foo>().AsSingle();

Container.Bind<Bar>().ToGetter<Foo>(foo => foo.GetBar());

// Another example using values
Container.Bind<string>().ToGetter<Foo>(foo => foo.GetTitle());

///////////// ToSingleGameObject

// Create a new game object at the root of the scene, add the Foo MonoBehaviour to it, and name it "Foo"
Container.Bind<Foo>().ToSingleGameObject("Foo");

// Bind to an interface instead
Container.Bind<IFoo>().ToSingleGameObject<Foo>("Foo");

///////////// ToSinglePrefab

// Create a new game object at the root of the scene using the given prefab
// It is assumed that the Foo is a MonoBehaviour here and that Foo has been
// previously added to the prefab
// After zenject creates a new GameObject from the given prefab, it will
// search the prefab for a component of type 'Foo' and return that
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);

// Bind to interface instead
Container.Bind<IFoo>().ToSinglePrefab<Foo>(fooPrefab);

// Note that in this case only one prefab will be instantiated and re-used
// for all three bindings
// (Prefab singletons are uniquely identified by their prefab)
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IInitializable>().ToSinglePrefab<Foo>(fooPrefab);
Container.Bind<ITickable>().ToSinglePrefab<Foo>(fooPrefab);

///////////// ToTransientPrefab

// Instantiate a new copy of 'fooPrefab' every time an instance of Foo is
// requested by a constructor parameter, injected field, etc.
GameObject fooPrefab;
Container.Bind<Foo>().ToTransientPrefab(fooPrefab);

// Bind to interface instead
Container.Bind<IFoo>().ToTransientPrefab<Foo>(fooPrefab);

///////////// Identifiers

// By default this will use 'Qux' for every place that requires an instance of IFoo
// But also allow for classes to use FooA or FooB by using identifiers
Container.Bind<IFoo>().To<Qux>().AsSingle();
Container.Bind<IFoo>("FooA").To<Bar>().AsSingle();
Container.Bind<IFoo>("FooB").To<Baz>().AsSingle();

public class Norf
{
    // Uses Qux
    [Inject]
    IFoo _foo;

    // Uses Bar
    [Inject("FooA")]
    IFoo _foo;

    // Uses Baz if it exists, otherwise leaves it as null
    [InjectOptional("FooB")]
    IFoo _foo;
}

// Bind a globally accessible string with the name 'PlayerName'
// A better option might be to create a Settings object and bind that
// instead however
Container.Bind<string>("PlayerName").ToInstance("name of the player");

///////////// Conditions

// This will only allow dependencies on Foo by the Bar class
Container.Bind<Foo>().AsSingle().WhenInjectedInto<Bar>();

// Use different implementations of IFoo dependending on which
// class is being injected
Container.Bind<IFoo>().To<Foo1>().AsSingle().WhenInjectedInto<Bar>();
Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

// Use "Foo1" as the default implementation except when injecting into
// class Qux, in which case use Foo2
Container.Bind<IFoo>().To<Foo1>().AsSingle();
Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

// Allow depending on Foo in only a few select classes
Container.Bind<Foo>().AsSingle().WhenInjectedInto(typeof(Bar), typeof(Qux), typeof(Baz));

// Supply "my game" for any strings that are injected into the Gui class with the identifier "Title"
Container.BindInstance("Title", "my game").WhenInjectedInto<Gui>();

// Supply 5 for all ints that are injected into the Gui class
Container.BindInstance(5).WhenInjectedInto<Gui>();

// Supply 5 for all ints that are injected into a parameter or field
// inside type Gui that is named 'width'
// This is usually not a good idea since the name of a field can change
// easily and break the binding but shown here as an example  :)
Container.BindInstance(5.0f).When(ctx =>
    ctx.ObjectType == typeof(Gui) && ctx.MemberName == "width");

// Create a new 'Foo' for every class that is created as part of the
// construction of the 'Bar' class
// So if Bar has a constructor parameter of type Qux, and Qux has
// a constructor parameter of type IFoo, a new Foo will be created
// for that case
Container.Bind<IFoo>().To<Foo>().AsTransient().When(
    ctx => ctx.AllObjectTypes.Contains(typeof(Bar)));

///////////// Complex conditions example

var foo1 = new Foo();
var foo2 = new Foo();

Container.Bind<Bar>("Bar1").AsTransient();
Container.Bind<Bar>("Bar2").AsTransient();

// Here we use the 'ParentContexts' property of inject context to sync multiple corresponding identifiers
Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar1").Any());
Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && x.Identifier == "Bar2").Any());

// This results in:
// Container.Resolve<Bar>("Bar1").Foo == foo1
// Container.Resolve<Bar>("Bar2").Foo == foo2

///////////// ToResolve

// This will result in IBar, IFoo, and Foo, all being bound to the same instance of
// Foo which is assume to exist somewhere on the given prefab
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IBar>().ToResolve<Foo>()
Container.Bind<IFoo>().ToResolve<IBar>()

// This is result in the same as the above
GameObject fooPrefab;
Container.Bind<Foo>().ToSinglePrefab(fooPrefab);
Container.Bind<IBar>().ToSinglePrefab<Foo>(fooPrefab);
Container.Bind<IFoo>().ToSinglePrefab<Foo>(fooPrefab);

///////////// Rebind

// Rebind can be used to override previous bindings
// This will result in IFoo being bound to only Bar
// The binding to Foo will have been removed
// Normally the order that the bindings occur in doesn't
// matter at all, but Rebind does create an order-dependency
// so use with caution
Container.Bind<IFoo>().To<Foo>().AsSingle();
Container.Rebind<IFoo>().To<Bar>().AsSingle();

///////////// Installing Other Installers

// Immediately call InstallBindings() on FooInstaller
Container.Install<FooInstaller>();

// Before calling FooInstaller, configure a property of it
Container.BindInstance("foo").WhenInjectedInto<FooInstaller>();
Container.Install<FooInstaller>();

// After calling FooInstaller, override one of its bindings
// We assume here that FooInstaller binds IFoo to something
Container.Install<FooInstaller>();
Container.Rebind<IFoo>().To<Bar>().AsSingle();

///////////// Manual Use of Container

// This will fill in any parameters marked as [Inject] and also call any [Inject] methods
foo = new Foo();
Container.Inject(foo);

// Return an instance for IFoo, using the bindings that have been added previously
// Internally it is what is triggered when you fill in a constructor parameter of type IFoo
// Note: It will throw an exception if it cannot find a match
Container.Resolve<IFoo>();

// Same as the above except returns null when it can't find the given type
Container.TryResolve<IFoo>();

// Return a list of 2 instances of type Foo
Container.BindInstance(new Foo());
Container.BindInstance(new Foo());
var foos = Container.ResolveAll<IFoo>();

// Instantiate a new instance of Foo and inject on any of its members
Container.Instantiate<Foo>();

// Instantiate a new prefab and have any injectables filled in on the prefab
GameObject go = Container.InstantiatePrefab(prefab);

// Instantiate a new prefab and return a specific monobehaviour
Foo foo = Container.InstantiatePrefabForComponent<Foo>(prefab);

// Add a new component to an existing game object
Foo foo = Container.InstantiateComponent<Foo>(gameObject);

```


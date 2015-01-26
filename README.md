<img src="UnityProject/Assets/Zenject/Main/ZenjectLogo.png?raw=true" alt="Zenject" width="600px" height="134px"/>

# Dependency Injection Framework for Unity3D

## Table Of Contents

* <a href="#introduction">Introduction</a>
* <a href="#features">Features</a>
* <a href="#history">History</a>
* Dependency Injection
    * <a href="#theory">Theory</a>
    * <a href="#misconceptions">Misconceptions</a>
* Zenject API
    * <a href="#overview-of-the-zenject-api">Overview of the Zenject API</a>
        * <a href="#hello-world-example">Hello World Example</a>
        * <a href="#binding">Binding</a>
        * <a href="#list-bindings">List Bindings</a>
        * <a href="#optional-binding">Optional Binding</a>
        * <a href="#conditional-bindings">Conditional Bindings</a>
        * <a href="#itickable">ITickable</a>
        * <a href="#iinitializable-and-postinject">IInitializable and PostInject</a>
        * <a href="#implementing-idisposable">Implementing IDisposable</a>
        * <a href="#installers">Installers</a>
    * <a href="#zenject-order-of-operations">Zenject Order Of Operations</a>
    * <a href="#di-rules--guidelines--recommendations">Rules / Guidelines / Recommendations</a>
    * <a href="#gotchas">Gotchas / Miscellaneous Tips and Tricks</a>
    * Advanced Features
        * <a href="#update--initialization-order">Update Order And Initialization Order</a>
        * <a href="#creating-objects-dynamically">Creating Objects Dynamically</a>
        * <a href="#game-object-factories">Game Object Factories</a>
        * <a href="#custom-factories">Custom Factories</a>
        * <a href="#using-bindscope">Using BindScope</a>
        * <a href="#injecting-data-across-scenes">Injecting Data Across Scenes</a>
        * <a href="#using-the-unity-inspector-to-configure-settings">Using the Unity Inspector To Configure Settings</a>
        * <a href="#object-graph-validation">Object Graph Validation</a>
        * <a href="#global-bindings">Global Bindings</a>
        * <a href="#scenes-decorator">Scenes Decorators</a>
        * <a href="#auto-mocking-using-moq">Auto-Mocking Using Moq</a>
        * <a href="#nested-containers">Nested Containers / FallbackProvider</a>
        * <a href="#visualizing-object-graphs-automatically">Visualizing Object Graph Automatically</a>
    * <a href="#questions">Frequently Asked Questions</a>
        * <a href="#faq-performance">How is Performance?</a>
    * <a href="#further-help">Further Help</a>
    * <a href="#release-notes">Release Notes</a>
    * <a href="#license">License</a>

## NOTE

The following documentation is written to be packaged with Zenject as it appears in the Asset store (which you can find [here](http://u3d.as/content/modest-tree-media/zenject-dependency-injection/7ER))

## <a id="introduction"></a>Introduction

Zenject is a lightweight dependency injection framework built specifically to target Unity 3D.  It can be used to turn your Unity 3D application into a collection of loosely-coupled parts with highly segmented responsibilities.  Zenject can then glue the parts together in many different configurations to allow you to easily write, re-use, refactor and test your code in a scalable and extremely flexible way.

Tested in Unity 3D on the following platforms: PC/Mac/Linux, iOS, Android, WP8, and Webplayer

This project is open source.  You can find the official repository [here](https://github.com/modesttree/Zenject).

For general troubleshooting / support, please use the google group which you can find [here](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  You can also follow [@Zenject](https://twitter.com/Zenject) on twitter for updates.  Finally, you can also email me directly at svermeulen@modesttree.com

## <a id="features"></a>Features

* Injection into normal C# classes or MonoBehaviours
* Constructor injection (can tag constructor if there are multiple)
* Field injection
* Property injection
* Conditional Binding Including Named injections (string, enum, etc.)
* Optional Dependencies
* Support For Building Dynamic Object Graphs At Runtime Using Factories
* Auto-Mocking using the Moq library
* Injection across different Unity scenes
* Ability to print entire object graph as a UML image automatically
* Ability to validate object graphs at editor time including dynamic object graphs
* Nested Containers

## <a id="history"></a>History

Unity is a fantastic game engine, however the approach that new developers are encouraged to take does not lend itself well to writing large, flexible, or scalable code bases.  In particular, the default way that Unity manages dependencies between different game components can often be awkward and error prone.

Having worked on non-unity projects that use dependency management frameworks (such as Ninject, which Zenject takes a lot of inspiration from), the problem irked me enough that I decided a custom framework was in order.  Upon googling for solutions, I found a series of great articles by Sebastiano Mandalà outlining the problem, which I strongly recommend that everyone read before firing up Zenject:

* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/)
* [http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/](http://blog.sebaslab.com/ioc-container-for-unity3d-part-2/)

Sebastiano even wrote a proof of concept and open sourced it, which became the basis for this library.

What follows in the next section is a general overview of Dependency Injection from my perspective.  I highly recommend seeking other resources for more information on the subject, as there are many (often more intelligent) people that have written on the subject.  In particular, I highly recommend anything written by Mark Seeman on the subject - in particular his book 'Dependency Injection in .NET'.

Finally, I will just say that if you don't have experience with DI frameworks, and are writing object oriented code, then trust me, you will thank me later!  Once you learn how to write properly loosely coupled code using DI, there is simply no going back.

## <a id="theory"></a>Theory

When writing an individual class to achieve some functionality, it will likely need to interact with other classes in the system to achieve its goals.  One way to do this is to have the class itself create its dependencies, by calling concrete constructors:

    public class Foo
    {
        ISomeService _service;

        public Foo()
        {
            _service = new SomeService();
        }

        public void DoSomething()
        {
            _service.PerformTask();
            ...
        }
    }

This works fine for small projects, but as your project grows it starts to get unwieldy.  The class Foo is tightly coupled to class 'SomeService'.  If we decide later that we want to use a different concrete implementation then we have to go back into the Foo class to change it.

After thinking about this, often you come to the realization that ultimately, Foo shouldn't bother itself with the details of choosing the specific implementation of the service.  All Foo should care about is fulfilling its own specific responsibilities.  As long as the service fulfills the abstract interface required by Foo, Foo is happy.  Our class then becomes:

    public class Foo
    {
        ISomeService _service;

        public Foo(ISomeService service)
        {
            _service = service;
        }

        public void DoSomething()
        {
            _service.PerformTask();
            ...
        }
    }

This is better, but now whatever class is creating Foo (let's call it Bar) has the problem of filling in Foo's extra dependencies:

    public class Bar
    {
        public void DoSomething()
        {
            var foo = new Foo(new SomeService());
            foo.DoSomething();
            ...
        }
    }

And class Bar probably also doesn't really care about what specific implementation of SomeService Foo uses.  Therefore we push the dependency up again:

    public class Bar
    {
        ISomeService _service;

        public Bar(ISomeService service)
        {
            _service = service;
        }

        public void DoSomething()
        {
            var foo = new Foo(_service);
            foo.DoSomething();
            ...
        }
    }

So we find that it is useful to push the responsibility of deciding which specific implementations of which classes to use further and further up in the 'object graph' of the application.  Taking this to an extreme, we arrive at the entry point of the application, at which point all dependencies must be satisfied before things start.  The dependency injection lingo for this part of the application is called the 'composition root'.

## <a id="misconceptions"></a>Misconceptions

There are many misconceptions about DI, due to the fact that it can be tricky to fully wrap your head around at first.  It will take time and experience before it fully 'clicks'.

As shown in the above example, DI can be used to easily swap different implementations of a given interface (in the example this was ISomeService).  However, this is only one of many benefits that DI offers.

More important than that is the fact that using a dependency injection framework like Zenject allows you to more easily follow the '[Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle)'.  By letting Zenject worry about wiring up the classes, the classes themselves can just focus on fulfilling their specific responsibilities.

Another common mistake that people new to DI make is that they extract interfaces from every class, and use those interfaces everywhere instead of using the class directly.  The goal is to make code more loosely coupled, so it's reasonable to think that being bound to an interface is better than being bound to a concrete class.  However, in most cases the various responsibilities of an application have single, specific classes implementing them, so using an interfaces in these cases just adds unnecessary maintenance overhead.  Also, concrete classes already have an interface defined by their public members.  A good rule of thumb instead is to only create interfaces when the class has more than one implementation.  This is known, by the way, as the [Reused Abstraction Principle](http://codemanship.co.uk/parlezuml/blog/?postid=934))

Other benefits include:

* Testability - Writing automated unit tests or user-driven tests becomes very easy, because it is just a matter of writing a different 'composition root' which wires up the dependencies in a different way.  Want to only test one subsystem?  Simply create a new composition root.  Zenject also has some support for avoiding code duplication in the composition root itself (described below). In cases where you can't easily separate out a specific sub-system to test, you can also creates 'mocks' for the sub-systems that you don't care about. (more detail <a href="#auto-mocking-using-moq">below</a>)
* Refactorability - When code is loosely coupled, as is the case when using DI properly, the entire code base is much more resilient to changes.  You can completely change parts of the code base without having those changes wreak havoc on other parts.
* Encourages modular code - When using a DI framework you will naturally follow better design practices, because it forces you to think about the interfaces between classes.

## <a id="overview-of-the-zenject-api"></a>Overview Of The Zenject API

What follows is a general overview of how DI patterns are applied using Zenject.  For further documentation I highly recommend the sample project itself (a kind of asteroids clone, which you can find by opening "Extras/SampleGame/Asteroids.unity").  I would recommend using that for reference after reading over these concepts.

The unit tests may also be helpful to show usage for each specific feature (which you can find by extracting Extras/ZenjectUnitTests.zip)

## <a id="hello-world-example"></a>Hello World Example

    using Zenject;
    using UnityEngine;
    using System.Collections;

    public class TestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITickable>().ToSingle<TestRunner>();
            Container.Bind<IInitializable>().ToSingle<TestRunner>();
        }
    }

    public class TestRunner : ITickable, IInitializable
    {
        public void Initialize()
        {
            Debug.Log("Hello World");
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Exiting!");
                Application.Quit();
            }
        }
    }

You can run this example by doing the following:

* Copy and paste the above code into a file named 'TestInstaller'
* Create a new scene in Unity
* Add a new GameObject and name it "CompositionRoot" (though the name does not really matter)
* Attach the CompositionRoot MonoBehaviour out of Zenject to your new GameObject
* Add your TestInstaller script to the scene as well (as its own GameObject or on the same GameObject as the CompositionRoot, it doesn't matter)
* Add a reference to your TestInstaller to the properties of the CompositionRoot by adding a new row in the inspector of the "Installers" property and then dragging the TestInstaller GameObject to it
* Validate your scene by either selecting Edit -> Zenject -> Validate Current Scene or hitting CTRL+SHIFT+V.  (note that this step isn't necessary but good practice to get into)
* Run
* Observe unity console for output

The CompositionRoot MonoBehaviour is the entry point of the application, where Zenject sets up all the various dependencies before kicking off your scene.  To add content to your Zenject scene, you need to write what is referred to in Zenject as an 'Installer', which declares all the dependencies used in your scene and their relationships with each other.  If the above doesn't make sense to you yet, keep reading!

## <a id="binding"></a>Binding

Every dependency injection framework is ultimately just a framework to bind types to instances.

In Zenject, dependency mapping is done by adding bindings to something called a container.  The container should then 'know' how to create all the object instances in our application, by recursively resolving all dependencies for a given object.

When the container is asked to construct an instance of a given type, it uses C# reflection to find the list of constructor arguments, and all fields/properties that are marked with an [Inject] attribute.  It then attempts to resolve each of these required dependencies, which it uses to call the constructor and create the new instance.

Each Zenject application therefore must tell the container how to resolve each of these dependencies, which is done via Bind commands.  The format for the bind command can be any of the following:

1. **ToSingle** - Inject as singleton

        Container.Bind<Foo>().ToSingle();

    When a type is bound using ToSingle this will construct one and only one instance of Foo and use that everywhere that has Foo as a dependency

    You may also bind the singleton instance to one or more interfaces:

        Container.Bind<IFoo>().ToSingle<Foo>();
        Container.Bind<IBar>().ToSingle<Foo>();

    This will cause any dependencies of type IFoo or IBar to use the same instance of Foo.  Of course, Foo must implement both IFoo and IBar for this to compile.  However, with only the above two lines the Foo singleton will not be accessible directly.  You can achieve this by using another line to uses ToSingle directly:

        Container.Bind<Foo>().ToSingle();
        Container.Bind<IFoo>().ToSingle<Foo>();
        Container.Bind<IBar>().ToSingle<Foo>();

    Note again that the same instance will be used for all dependencies that take Foo, IFoo, or IBar.

1. **ToTransient** - Inject as newly created object

        Container.Bind<Foo>().ToTransient();

    In this case a new instance of Foo will be generated each time it is injected. Similar to ToSingle, you can bind via an interface as well:

        Container.Bind<IFoo>().ToTransient<Foo>();

1. **ToSingleFromPrefab** - Inject from unity prefab as singleton

        Container.Bind<FooMonoBehaviour>().ToSingleFromPrefab<FooMonoBehaviour>(PrefabGameObject);

    This will instantiate a new instance of the given prefab, and then search the newly created game object for the given component (in this case FooMonoBehaviour).  Note in this case specifying `FooMonoBehaviour` twice is redundant but necessary.

    Also, because it is ToSingle it will only instantiate the prefab once, and otherwise use the same instance of FooMonoBehaviour

1. **ToTransientFromPrefab** - Inject from unity prefab as newly created object

        Container.Bind<FooMonoBehaviour>().ToTransientFromPrefab<FooMonoBehaviour>(PrefabGameObject);

    This works similar to ToSingleFromPrefab except it will instantiate a new instance of the given prefab every time the dependency is injected.

1.  **ToSingleGameObject** - Inject MonoBehaviour.

        Container.Bind<FooMonoBehaviour>().ToSingleGameObject();

    This binding will create a new game object and attach the given FooMonoBehaviour.  Also note that since it is ToSingle that it will use the same instance everywhere that has FooMonoBehaviour as a dependency

1.  **ToMethod** - Inject using a custom method

    This binding allows you to customize creation logic yourself by defining a method:

        Container.Bind<IFoo>().ToMethod(SomeMethod);

        ...

        public IFoo SomeMethod(DiContainer container)
        {
            ...
            return new Foo();
        }

1.  **ToGetter** - Inject by getter.

    This method can be useful if you want to bind to a property of another object.

        Container.Bind<IFoo>().ToSingle<Foo>()
        Container.Bind<Bar>().ToGetter<IFoo>(x => x.GetBar())

1.  **ToLookup** - Inject by recursive resolve.

        Container.Bind<IFoo>().ToLookup<IBar>()
        Container.Bind<IBar>().ToLookup<Foo>()

    In some cases it is useful to be able to bind an interface to another interface.  However, you cannot use ToSingle or ToTransient because they both require concrete types.

    In the example code above we assume that Foo inherits from IBar, which inherits from IFoo.  The result here will be that all dependencies for IFoo will be bound to whatever IBar is bound to (in this case, Foo).

1. **BindValue** - Inject primitive values

        Container.BindValue<float>().To(1.5f);
        Container.BindValue<int>().To(42);

    Primitive types such as int, float, struct, etc. are treated specially in Zenject.  Note that when binding to primitives you will almost certaintly want to specify the type that the binding is for using `WhenInjectedInto` (described <a href="#conditional-bindings">below</a>).  I'll also add that while it can be useful to inject primitives for configuration settings it is often better to inject a "settings" object instead.  There are other advantages to this approach as well as described <a href="#using-the-unity-inspector-to-configure-settings">here</a>.

1. **Rebind** - Override existing binding

        Container.Rebind<IFoo>().To<Foo>();

    The Rebind function can be used to override any existing bindings that were added previously.  It will first clear all previous bindings and then add the new binding.  This method is especially useful for tests, where you often want to use almost all the same bindings used in production, except override a few specific bindings.

1. **Untyped Bindings**

        Container.Bind(typeof(IFoo)).ToSingle(typeof(Foo));

    In some cases it is not possible to use the generic versions of the Bind<> functions.  In these cases a non-generic version is provided, which works by taking in a Type value as a parameter.

## <a id="list-bindings"></a>List Bindings

When Zenject finds multiple bindings for the same type, it interprets that to be a list.  So, in the example code below, Bar would get a list containing a new instance of Foo1, Foo2, and Foo3:

    ...

    // In an installer somewhere
    Container.Bind<IFoo>().ToSingle<Foo1>();
    Container.Bind<IFoo>().ToSingle<Foo2>();
    Container.Bind<IFoo>().ToSingle<Foo3>();

    ...

    public class Bar
    {
        public Bar(List<IFoo> foos)
        {
        }
    }

Also worth noting is that if you try and declare a single dependency of IFoo (like Bar below) and there are multiple bindings for it, then Zenject will throw an exception, since Zenject doesn't know which instance of IFoo to use.  Also, if the empty list is valid, then you should mark your List constructor parameter (or [Inject] field) as optional (see <a href="#optional-binding">here</a> for details).

    public class Bar
    {
        public Bar(IFoo foo)
        {
        }
    }

## <a id="optional-binding"></a>Optional Binding

You can declare some dependencies as optional as follows:

    public class Bar
    {
        public Bar(
            [InjectOptional]
            IFoo foo)
        {
            ...
        }
    }

In this case, if IFoo is not bound in any installers, then it will be passed as null.

Note that when declaring dependencies with primitive types as optional, they will be given their default value (eg. 0 for ints).  However, if you need to distinguish between being given a default value and the primitive dependency not being specified, you can do this as well by declaring it as nullable:

    public class Bar
    {
        int _foo;

        public Bar(
            [InjectOptional]
            int? foo)
        {
            if (foo == null)
            {
                // Use 5 if unspecified
                _foo = 5;
            }
            else
            {
                _foo = foo.Value;
            }
        }
    }

    ...

    // Can leave this commented or not and it will still work
    // Container.BindValue<int>().To(1);

## <a id="conditional-bindings"></a>Conditional Bindings

In many cases you will want to restrict where a given dependency is injected.  You can do this using the following syntax:

Use different implementations of IFoo in different cases:

    Container.Bind<IFoo>().ToSingle<Foo1>().WhenInjectedInto<Bar1>();
    Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar2>();

Inject by name:

    Container.Bind<IFoo>().ToSingle<Foo>().As("foo");

    ...

    public class Bar
    {
        [Inject("foo")]
        Foo _foo;
    }

You can also inject by name and also restrict to only Bar class:

    Container.Bind<IFoo>().ToSingle<Foo>().WhenInjectedInto<Bar2>().As("foo");

Note also that you can name dependencies with any type (and not just string) and that it applies to constructor arguments as well, for example:

    enum Foos
    {
        A,
    }

    public class Bar
    {
        Foo _foo;

        public Bar(
            [Inject(Foos.A)] Foo foo)
        {
        }
    }

Note that `WhenInjectedInto` is simple shorthand for the following.  You can use the more general `When()` method for more complex conditionals.

    Container.Bind<IFoo>().ToSingle<Foo>().When(context => context.Target == typeof(Bar)).As("foo");

The InjectContext class (which is passed as the `context` parameter above) contains the following information that you can use in your conditional:

* `Type EnclosingType` - The type of the newly instantiated object, which we are injecting dependencies into.
* `object EnclosingInstance` - The newly instantiated instance that is having its dependencies filled.  Note that this is only available when injecting fields and null for constructor parameters
* `string SourceName` - The name of the field or parameter that we are injecting into.  This can be used, for example, in the case where you have multiple constructor parameters that are strings.  However, using the parameter or field name can be error prone since other programmers may refactor it to use a different name.  In many cases it's better to use an explicit identifier
* `object Identifier` - This will be null in most cases and set to whatever is given as a parameter to the [Inject] attribute.  For example, `[Inject("foo")] _foo` will result in `Identifier` being equal to the string "foo".
* `List<Type> ParentTypes` - This contains the entire object graph that precedes the current class being created.  For example, dependency A might be created, which requires an instance of B, which requires an instance of C.  In this case, the InjectContext given for any fields when creating C will contains the list `{typeof(A), typeof(B)}`.
* `bool Optional` - True if the [InjectOptional] parameter is declared on the field being injected

## <a id="itickable"></a>ITickable

I prefer to avoid MonoBehaviours when possible in favour of just normal C# classes.  Zenject allows you to do this much more easily by providing interfaces that mirror functionality that you would normally need to use a MonoBehaviour for.

For example, if you have code that needs to run per frame, then you can implement the ITickable interface:

    public class Ship : ITickable
    {
        public void Tick()
        {
            // Perform per frame tasks
        }
    }

Then it's just a matter of including the following in one of your installers (as long as you are using DependencyRootStandard or a subclass)

    Container.Bind<ITickable>().ToSingle<Ship>();

Note that the order that Tick() is called on all ITickables is also configurable, as outlined <a href="#update--initialization-order">here</a>.

## <a id="iinitializable-and-postinject"></a>IInitializable and PostInject

If you have some initialization that needs to occur on a given object, you can include this code in the constructor.  However, this means that the initialization logic would occur in the middle of the object graph being constructed, so it may not be ideal.

One alternative is to implement IInitializable, and then perform initialization logic in an Initialize() method.  This method would be called immediately after the entire object graph is constructed.  This is also nice because the initialization order is customizable in a similar way to ITickable, as explained <a href="#update--initialization-order">here</a>.

IInitializable works well for start-up initialization, but what about for objects that are created dynamically via factories?  (see <a href="#dynamic-object-graph-validation">this section</a> for what I'm referring to here).

In these cases you can mark any methods that you want to be called after injection occurs with a [PostInject] attribute:

    public class Foo
    {
        [Inject]
        IBar _bar;

        [PostInject]
        public void Initialize()
        {
            ...
            _bar.DoStuff();
            ...
        }
    }

This still has the drawback that it is called in the middle of object graph construction, but can be useful in many cases.  In particular, if you are using property injection (which isn't generally recommended but necessary in some cases) then you will not have your dependencies in the constructor, and therefore you will need to define a [PostInject] method in this case.  You will also need to use [PostInject] for MonoBehaviours that you are creating dynamically, since MonoBehaviours cannot have constructors.

In the case where there are multiple [PostInject] methods on a given object, they are called in the order of Base class to Derived class.

## <a id="implementing-idisposable"></a>Implementing IDisposable

If you have external resources that you want to clean up when the app closes, the scene changes, or for whatever reason the composition root object is destroyed, you can do the following:

    public class Logger : IInitializable, IDisposable
    {
        FileStream _outStream;

        public void Initialize()
        {
            _outStream = File.Open("log.txt", FileMode.Open);
        }

        public void Log(string msg)
        {
            _outStream.WriteLine(msg);
        }

        public void Dispose()
        {
            _outStream.Close();
        }
    }

Then in your installer you can include:

    Container.Bind<Logger>().ToSingle();
    Container.Bind<IInitializable>().ToSingle<Logger>();
    Container.Bind<IDisposable>().ToSingle<Logger>();

This works because when the scene changes or your unity application is closed, the unity event OnDestroy() is called on all MonoBehaviours, including the CompositionRoot class, which then triggers all objects that are bound to IDisposable

Note that this example may or may not be a good idea (for example, the file will be left open if your app crashes), but illustrates the point  :)

## <a id="installers"></a>Installers

Often, there is some collection of related bindings for each sub-system and so it makes sense to group those bindings into a re-usable object.  In Zenject this re-usable object is called an Installer.  You can define a new installer as follows:

    public class FooInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITickable>().ToSingle<Foo>();
            Container.Bind<IInitializable>().ToSingle<Foo>();
        }
    }

You add bindings by overriding the InstallBindings method, which is called by the CompositionRoot when your scene starts up.  MonoInstaller is a MonoBehaviour so you can add FooInstaller by attaching it to a GameObject.  Since it is a GameObject you can also add public members to it to configure your installer from the Unity inspector.  However, note that in order for your installer to be used it must be attached to the Installers property of the CompositionRoot object.

In many cases you want to have your installer derive from MonoInstaller.  There is also another base class called Installer which you can use in cases where you do not need it to be a MonoBehaviour.

It can also be nice to use Installer since this allows you to "include" it from another installer. For example:

    public class BarInstaller : Installer
    {
        public override void InstallBindings()
        {
            ...
        }
    }

    public class FooInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInstaller>().ToSingle<BarInstaller>();
        }
    }

This way you don't need to have an instance of BarInstaller in your scene in order to use it.  After the CompositionRoot calls InstallBindings it will then instantiate and call any extra installers that have bound to IInstaller.

One of the main reasons we use installers as opposed to just having all our bindings declared all at once for each scene, is to make them re-usable.  So how then do we use the same installer in multiple scenes?

The recommended way of doing this is to use unity prefabs.  After attaching your MonoInstaller to a gameobject in your scene, you can then create a prefab out of it.  This is nice because it allows you to share any configuration that you've done in the inspector on the MonoInstaller across scenes (and also have per-scene overrides if you want)

Installers that simply implement Installer instead of MonoInstaller can be simply bound as described above, to re-use in different scenes.

## <a id="zenject-order-of-operations"></a>Zenject Order Of Operations

A Zenject driven application is executed by the following steps:

* Composition Root is started (via Unity Awake() method)
* Composition Root creates a new DiContainer object to be used to contain all instances used in the scene
* Composition Root iterates through all the Installers that have been added to it via the Unity Inspector, and updates them to point to the new DiContainer.  It then calls InstallBindings() on each installer.
* Each Installer then registers different sets of dependencies directly on to the given DiContainer by calling one of the Bind<> methods.  Note that the order that this binding occurs should not generally matter. Each installer may also include other installers by binding to the IInstaller interface.  Each installer can also add bindings to configure other installers, however note that in this case order might actually matter, since you will have to make sure that code configuring other installers is executed before the installers that you are configuring! You can control the order by simply re-ordering the Installers property of the CompositionRoot
* The Composition Root then traverses the entire scene hierarchy and injects all MonoBehaviours with their dependencies. Since MonoBehaviours are instantiated by Unity we cannot use constructor injection in this case and therefore field or property injection must be used (which is done by adding a [Inject] attribute to any member).  Any methods on these MonoBehaviour's marked with [PostInject] are called at this point as well.
* After filling in the scene dependencies the Composition Root then retrieves the instance of IDependencyRoot, which contains the objects that handle the ITickable/IInitializable/IDisposable interfaces.
* If any required dependencies cannot be resolved, a ZenjectResolveException is thrown
* Initialize() is called on all IInitializable objects in the order specified in the installers
* Unity Start() is called on all built-in MonoBehaviours
* Unity Update() is called, which results in Tick() being called for all ITickable objects (in the order specified in the installers)
* App is exited
* Dispose() is called on all objects mapped to IDisposable (see <a href="#implementing-idisposable">here</a> for details)

## <a id="di-rules--guidelines--recommendations"></a>DI Rules / Guidelines / Recommendations

* The container should *only* be referenced in the composition root "layer".  Note that factories are part of this layer and the container can be referenced there (which is necessary to create objects at runtime).  For example, see ShipStateFactory in the sample project.  See <a href="#dynamic-object-graph-validation">here</a> for more details on this.
* Prefer constructor injection to field or property injection.
    * Constructor injection forces the dependency to only be resolved once, at class creation, which is usually what you want.  In many cases you don't want to expose a public property with your internal dependencies
    * Constructor injection guarantees no circular dependencies between classes, which is generally a bad thing to do
    * Constructor injection is more portable for cases where you decide to re-use the code without a DI framework such as Zenject.  You can do the same with public properties but it's more error prone (it's easier to forget to initialize one field and leave the object in an invalid state)
    * Finally, Constructor injection makes it clear what all the dependencies of a class are when another programmer is reading the code.  They can simply look at the parameter list of the constructor.

## <a id="gotchas"></a>Gotchas / Miscellaneous Tips and Tricks

* **Do not use GameObject.Instantiate if you want your objects to have their dependencies injected**
    * If you want to create a prefab yourself, you can use the provided zenject class GameObjectInstantiator, which will automatically fill in any fields that are marked with the [Inject] attribute.
    * You can also use GameObjectFactory as suggested <a href="#game-object-factories">in this section</a>

* **Do not use IInitializable, ITickable and IDisposable for dynamically created objects**
    * Objects that are of type IInitializable are only initialized once, at startup.  If you create an object through a factory, and it derives from IInitializable, the Initialize() method will not be called.  You should use [PostInject] in this case.
    * The same applies to ITickable and IDisposable.  Deriving from these will do nothing unless they are part of the original object graph created at startup
    * If you have dynamically created objects that have an Update() method, it is usually best to call Update() on those manually, and often there is a higher level manager-like class in which it makes to do this from.  If however you prefer to use ITickable for dynamically objects you can declare a dependency to TickableManager and add/remove it explicitly as well.

* **Using multiple constructors**
    * Zenject does not support injecting into multiple constructors currently.  You can have multiple constructors however you must mark one of them with the [Inject] attribute so Zenject knows which one to use.

* **Injecting into MonoBehaviours**
    * One issue that often arises when using Zenject is that a game object is instantiated dynamically, and then one of the monobehaviours on that game object attempts to use one of its injected field dependencies in its Start() or Awake() methods.  Often in these cases the dependency will still be null, as if it was never injected.  The issue here is that Zenject cannot fill in the dependencies until after the call to GameObject.Instantiate completes, and in most cases GameObject.Instantiate will call the Start() and Awake() methods.  The solution is to use neither Start() or Awake() and instead define a new method and mark it with a [PostInject] attribute.  This will guarantee that all dependencies have been resolved before executing the method.

Please feel free to submit any other sources of confusion to svermeulen@modesttree.com and I will add it here.

## <a id="update--initialization-order"></a>Update / Initialization Order

In many cases, especially for small projects, the order that classes update or initialize in does not matter.  However, in larger projects update or initialization order can become an issue.  This can especially be an issue in Unity, since it is often difficult to predict in what order the Start(), Awake(), or Update() methods will be called in.  Unfortunately, Unity does not have an easy way to control this (besides in Edit -> Project Settings -> Script Execution Order, though that is pretty awkward to use)

In Zenject, by default, ITickables and IInitializables are updated in the order that they are added, however for cases where the update or initialization order matters, there is a much better way:  By specifying their priorities explicitly in the installer.  For example, in the sample project you can find this code in the scene installer:

    public class AsteroidsInstaller : MonoInstaller
    {
        ...

        void InitPriorities()
        {
            Container.Bind<IInstaller>().ToSingle<InitializablePrioritiesInstaller>();
            Container.Bind<List<Type>>().To(InitializablesOrder)
                .WhenInjectedInto<InitializablePrioritiesInstaller>();

            Container.Bind<IInstaller>().ToSingle<TickablePrioritiesInstaller>();
            Container.Bind<List<Type>>().To(Tickables)
                .WhenInjectedInto<TickablePrioritiesInstaller>();
        }

        static List<Type> InitializablesOrder = new List<Type>()
        {
            // Re-arrange this list to control init order
            typeof(GameController),
        };

        static List<Type> TickablesOrder = new List<Type>()
        {
            // Re-arrange this list to control update order
            typeof(AsteroidManager),
            typeof(GameController),
        };
    }

This way, you won't hit a wall at the end of the project due to some unforeseen order-dependency.

Note also that any ITickables or IInitializables that aren't given an explicit order are updated last.

## <a id="creating-objects-dynamically"></a>Creating Objects Dynamically

One of the things that often confuses people new to dependency injection is the question of how to create new objects dynamically, after the app/game has fully started up and all the IInitializable objects have had their Initialize() method called.  For example, if you are writing a game in which you are spawning new enemies throughout the game, then you will want to construct a new object graph for the 'enemy' class.  How to do this?  The answer: Factories.

Remember that an important part of dependency injection is to reserve use of the container to strictly the "Composition Root Layer".  The container class (DiContainer) is included as a dependency in itself automatically so there is nothing stopping you from ignoring this rule and injecting the container into any classes that you want.  For example, the following code will work:

    public class Enemy
    {
        DiContainer Container;

        public Enemy(DiContainer container)
        {
            Container = container;
        }

        public void Update()
        {
            ...
            var player = Container.Resolve<Player>();
            WalkTowards(player.Position);
            ...
            etc.
        }
    }

HOWEVER, the above code is an example of an anti-pattern.  This will work, and you can use the container to get access to all other classes in your app, however if you do this you will not really be taking advantage of the power of dependency injection.  This is known, by the way, as [Service Locator Pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

Of course, the dependency injection way of doing this would be the following:

    public class Enemy
    {
        Player _player;

        public Enemy(Player player)
        {
            _player = player;
        }

        public void Update()
        {
            ...
            WalkTowards(_player.Position);
            ...
        }
    }

The only exception to this rule is within factories and installers.  Again, factories and installers make up what we refer to as the "composition root layer".

For example, if you have a class responsible for spawning new enemies, before DI you might do something like this:

    public class EnemySpawner
    {
        List<Enemy> _enemies = new List<Enemy>();

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var enemy = new Enemy();
                _enemies.Add(enemy);
            }
        }
    }

This will not work however, since in our case the Enemy class requires a reference to the Player class in its constructor.  We could add a dependency to the Player class to the EnemySpawner class, but then we have the problem described <a href="#theory">above</a>.  The EnemySpawner class doesn't care about filling in the dependencies for the Enemy class.  All the EnemySpawner class cares about is getting a new Enemy instance.

The recommended way to do this in Zenject is the following:

    public class Enemy
    {
        Player _player;

        public Enemy(Player player)
        {
            _player = player;
        }

        ...

        public class Factory : Factory<Enemy>
        {
        }
    }

    public class EnemySpawner
    {
        Enemy.Factory _enemyFactory;
        List<Enemy> _enemies = new List<Enemy>();

        public EnemySpawner(Enemy.Factory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var enemy = _enemyFactory.Create();
                _enemies.Add(enemy);
            }
        }
    }

Then in your installer, you would include:

    Container.Bind<Enemy.Factory>().ToSingle();

By using Enemy.Factory above, all the dependencies for the Enemy class (such as the Player) will be automatically filled in.

There is no requirement that the Enemy.Factory class be a subclass of Enemy, however we have found this to be a very useful convention.  Enemy.Factory is empty and simply derives from the built-in Zenject Factory<> class, which handles the work of using the DiContainer to construct a new instance of Enemy.

Also note that by using the built-in Zenject Factory<> class, the Enemy class will be automatically validated as well.  So if the constructor of the Enemy class includes a type that is missing a binding, this error can be caught before running your app, by simply running validation.  Validation can be especially useful for dynamically created objects, because otherwise you may not catch the error until the factory is invoked at some point during runtime.  See <a href="#dynamic-object-graph-validation">this section</a> for more details on Validation.

However, in more complex examples, the EnemySpawner class may wish to pass in custom constructor arguments as well. For example, let's say we want to randomize the speed of each Enemy to add some interesting variation to our game.  Our enemy class becomes:

    public class Enemy
    {
        Player _player;
        float _runSpeed;

        public Enemy(Player player, float runSpeed)
        {
            _player = player;
            _runSpeed = runSpeed;
        }

        public class Factory : Factory<float, Enemy>
        {
        }
    }

    public class EnemySpawner
    {
        Enemy.Factory _enemyFactory;
        List<Enemy> _enemies = new List<Enemy>();

        public EnemySpawner(Enemy.Factory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public void Update()
        {
            if (ShouldSpawnNewEnemy())
            {
                var newSpeed = Random.Range(MIN_ENEMY_SPEED, MAX_ENEMY_SPEED);
                var enemy = _enemyFactory.Create(newSpeed);
                _enemies.Add(enemy);
            }
        }
    }

The dynamic parameters that are provided to the Enemy constructor are declared by using generic arguments to the Factory<> base class of Enemy.Factory.  This will add a method to Enemy.Factory that takes the parameters with the given types.

## <a id="game-object-factories"></a>Game Object Factories

You can also use the same approach as described <a href="#creating-objects-dynamically">above</a> to create factories that construct game objects.  For example:

    public class FooMonoBehaviour : MonoBehaviour
    {
        ...

        public class Factory : GameObjectFactory<FooMonoBehaviour>
        {
        }
    }

The only difference here is that this factory requires a prefab to be installed on it.  There is a convenience method that you can use to handle both installing the prefab and also declaring FooMonoBehaviour.Factory as a singleton:

    public override void InstallBindings()
    {
        ...

        Container.BindGameObjectFactory<FooMonoBehaviour.Factory>(_prefab);

        ...
    }

Now classes can simply declare a constructor parameter of type FooMonoBehaviour.Factory and by calling the Create() method, construct new instances of a given prefab.

## <a id="custom-factories"></a>Custom Factories

You may also use your own custom factory.  This is often necessary when you want to construct an instance of an interface at runtime.  In this case you do not want to refer to a concrete factory class (eg. Enemy.Factory) as described above.

    public enum Difficulties
    {
        Easy,
        Hard,
    }

    public interface IEnemy
    {
        ...
    }

    public class Demon : IEnemy
    {
        ...
    }

    public class Dog : IEnemy
    {
        ...
    }

    public class EnemyFactory
    {
        DiContainer _container;
        Difficulties _difficulty;

        public EnemyFactory(DiContainer container, Difficulties difficulty)
        {
            _container = container;
            _difficulty = difficulty;
        }

        public IEnemy Create(float speed)
        {
            if (_difficulty == Difficulties.Hard)
            {
                return _container.Instantiate<Demon>(speed);
            }

            return _container.Instantiate<Dog>(speed);
        }
    }

And then in our installer we would include:

    Container.Bind<EnemyFactory>().ToSingle();
    Container.BindValue<Difficulties>().To(Difficulties.Easy);

This way we can change the type of enemy we spawn by simply changing the difficulty bound in the installer.

One issue with the above implementation is that it will not be validated properly.  Any constructor parameters added to the Dog or Demon classes that cannot be resolved will not be detected until runtime.  If you wish to address this you can implement it the following way:

    public class EnemyFactory : IValidatable
    {
        DiContainer _container;
        Difficulties _difficulty;

        public EnemyFactory(DiContainer container, Difficulties difficulty)
        {
            _container = container;
            _difficulty = difficulty;
        }

        public IEnemy Create(float speed)
        {
            if (_difficulty == Difficulties.Hard)
            {
                return _container.Instantiate<Demon>(speed);
            }

            return _container.Instantiate<Dog>(speed);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<Dog>(typeof(float))
                .Concat(_container.ValidateObjectGraph<Demon>(typeof(float)));
        }
    }

This is optional but can be nice if you are fan of validation.  The parameters provided to the ValidateObjectGraph method indicate the dependencies that can be skipped for validation.  This should include any runtime parameters.

Note that we are injecting the DiContainer directly into the EnemyFactory class, which is generally a bad thing to do but ok in this case because it is a factory (and therefore part of the "composition root layer")

## <a id="using-bindscope"></a>Using BindScope

In the real world there can sometimes be complex construction that needs to occur in your custom factory classes.  In some of these cases it can be useful to use a feature called BindScope.

For example, suppose one day we decide to add further runtime constructor arguments to the Enemy class:

    public class Enemy
    {
        public Enemy(EnemyWeapon weapon)
        {
            ...
        }
    }

    public class EnemyWeapon
    {
        public EnemyWeapon(float damage)
        {
            ...
        }
    }

And let's say we want the damage of the EnemyWeapon class to be specified by the EnemySpawner class.  How do we pass that argument down to EnemyWeapon?  In this case it might be easiest to create the EnemyWeapon class first and then pass it to the factory.  However, for the sake of this example let's pretend we want to create the EnemyClass in one call to Instantiate

    public class EnemyFactory
    {
        DiContainer _container;

        public EnemyFactory(DiContainer container)
        {
            _container = container;
        }

        public Enemy Create(float weaponDamage)
        {
            using (BindScope scope = Container.CreateScope())
            {
                scope.Bind<float>().ToSingle(weaponDamage).WhenInjectedInto<EnemyWeapon>();
                return _container.Instantiate<Enemy>();
            }
        }
    }

BindScope can be used in factories to temporarily configure the container in a similar way that's done in installers.  This can be useful when creating complex object graphs at runtime.  After the function returns, whatever bindings you added in the using{} block are automatically removed.  BindScope can also be used to specify injection identifiers as well.

## <a id="injecting-data-across-scenes"></a>Injecting data across scenes

In some cases it's useful to pass arguments from one scene to another.  The way Unity allows us to do this by default is fairly awkward.  Your options are to create a persistent GameObject and call DontDestroyOnLoad() to keep it alive when changing scenes, or use global static classes to temporarily store the data.

Let's pretend you want to specify a 'level' string to the next scene.  You have the following class that requires the input:

    public class LevelHandler : IInitializable
    {
        readonly string _startLevel;

        public LevelHandler(
            [InjectOptional]
            [Inject("StartLevelName")]
            string startLevel)
        {
            if (startLevel == null)
            {
                _startLevel = "default_level";
            }
            else
            {
                _startLevel = startLevel;
            }
        }

        public void Initialize()
        {
            ...
            [Load level]
            ...
        }
    }

You can load the scene containing `LessonStandaloneStart` and specify a particular level by using the following syntax:

    ZenUtil.LoadScene("NameOfSceneToLoad",
        delegate (DiContainer container)
        {
            container.Bind<string>().To("custom_level").WhenInjectedInto<LevelHandler>().As("StartLevelName");
        });

Note that you can still run the scene directly, in which case it will default to using "level01".  This is possible because we are using the InjectOptional flag.

An alternative and arguably cleaner way to do this would be to customize the installer itself rather than the LevelHandler class.  In this case we can write our LevelHandler class like this (without the [InjectOptional] flag)

    public class LevelHandler : IInitializable
    {
        readonly string _startLevel;

        public LevelHandler(string startLevel)
        {
            _startLevel = startLevel;
        }

        public void Initialize()
        {
            ...
            [Load level]
            ...
        }
    }

Then, in the installer for our scene we can include the following:

    public class GameInstaller : Installer
    {
        [InjectOptional]
        public string LevelName = "default_level";

        ...

        public override void InstallBindings()
        {
            ...
            Container.Bind<string>().To(LevelName).WhenInjectedInto<LevelHandler>();
            ...
        }
    }

Then, instead of injecting directly into the LevelHandler we can inject into the installer instead.

    ZenUtil.LoadScene("NameOfSceneToLoad",
        delegate (DiContainer container)
        {
            container.Bind<string>().To("level02").WhenInjectedInto<GameInstaller>();
        });

Note that in this case I didn't need to use the "LevelName" identifier since there is only one string injected into the GameInstaller class.

Some people have also found it useful to separate out content into different scenes and then load each scene additively using the Unity method `Application.LoadLevelAdditive`.  In some cases it's useful to have the dependencies in the new scene resolved using the container of the original scene.  To achieve this, you can call `ZenUtil.LoadSceneAdditiveWithContainer` and pass in your scene's container.  Note however that it is assumed in this case that the new scene does not have its own container + Composition Root.

## <a id="using-the-unity-inspector-to-configure-settings"></a>Using the Unity Inspector To Configure Settings

One implication of writing most of your code as normal C# classes instead of MonoBehaviour's is that you lose the ability to configure data on them using the inspector.  You can however still take advantage of this in Zenject by using the following pattern, as seen in the sample project:

    public class AsteroidsInstaller : MonoInstaller
    {
        public Settings SceneSettings;

        public override void InstallBindings()
        {
            ...
            Container.Bind<ShipStateMoving.Settings>().ToSingle(SceneSettings.StateMoving);
            ...
        }

        [Serializable]
        public class Settings
        {
            ...
            public ShipStateMoving.Settings StateMoving;
            ...
        }
    }

Note that if you follow this method, you will have to make sure to always include the [Serializable] attribute on your settings wrappers, otherwise they won't show up in the Unity inspector.

To see this in action, start the asteroids scene and try adjusting `Ship -> State Moving -> Move Speed` setting and watch live as your ship changes speed.

## <a id="object-graph-validation"></a>Object Graph Validation

The usual workflow when setting up bindings using a DI framework is something like this:

* Add some number of bindings in code
* Execute your app
* Observe a bunch of DI related exceptions
* Modify your bindings to address problem
* Repeat

This works ok for small projects, but as the complexity of your project grows it is often a tedious process.  The problem gets worse if the startup time of your application is particularly bad, or when the resolve errors only occur from factories at various points at runtime.  What would be great is some tool to analyze your object graph and tell you exactly where all the missing bindings are, without requiring the cost of firing up your whole app.

You can do this in Zenject out-of-the-box by executing the menu item `Edit -> Zenject -> Validate Current Scene` or simply hitting CTRL+SHIFT+V with the scene open that you want to validate.  This will execute all installers for the current scene and construct a fully bound container.   It will then iterate through the object graphs and verify that all bindings can be found (without actually instantiating any of them).

Also, if you happen to be a fan of automated testing (as I am) then you can include calls to this menu item as part of your testing suite.

## <a id="global-bindings"></a>Global Bindings

This all works great for each individual scene, but what if have dependencies that you wish to persist permanently across all scenes?  In Zenject you can do this by adding installers to the global container.

This works by first add a global composition root and then adding installers to it.  You can create an empty global composition root by selecting Edit -> Zenject -> Create Global Composition Root.  After selecting this menu item you should see a new asset in the root level Resources folder called 'ZenjectGlobalCompositionRoot'.

If you click on this it will display a property for the list of Installers in the same way that it does for the composition root object that is placed in each scene.  The only difference in this case is that the installers you add here must exist in the project as prefabs and cannot exist in any specific scene.  You can then directly reference those prefabs by dragging them into the Installers property of the global composition root.

Then, when you start any scene, the CompositionRoot for the scene will call the global composition root to install the global bindings, before installing any scene specific bindings.  If you load another scene from the first scene, the global composition root will not be called again and the bindings that it added previously will persist into the new scene.  You can declare ITickable / IInitializable / IDisposable objects in your global installers in the same way you do for your scene installers with the result being IInitializable.Initialize is called only once across each play session and IDisposable.Dispose is only called once the application is fully stopped.

## <a id="scenes-decorator"></a>Scene Decorators

Scene Decorators can be used to add behaviour to another scene without actually changing the installers in that scene.  The usual way to achieve this is to use flags on MonoInstallers to conditionally add different bindings within the scene itself.  However the scene decorator approach can be cleaner sometimes because it doesn't involve changing the main scene.

For example, let's say we want to add some special keyboard shortcuts to our main production scene for testing purposes.  In order to do this using decorators, you can do the following:

* Create a new scene
* Add an empty GameObject and name it 'CompositionRoot'
* Add a 'SceneDecoratorCompositionRoot' MonoBehaviour to it
* Type in the scene you want to 'decorate' in the 'Scene Name' field of SceneDecoratorCompositionRoot
* Create a new C# script with the following contents, then add your the MonoBehaviour to your scene and drag it to the Installers property of SceneDecoratorCompositionRoot

        public class ExampleDecoratorInstaller : DecoratorInstaller
        {
            public override void PostInstallBindings()
            {
                // Add bindings here that you want added AFTER installing the main scene

                Container.Bind<ITickable>().ToSingle<TestHotKeysAdder>();
            }

            public override void PreInstallBindings()
            {
                // Add bindings here that you want added BEFORE installing the main scene
            }
        }

        public class TestHotKeysAdder : ITickable
        {
            public void Tick()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Hotkey triggered!");
                }
            }
        }

If you run your scene it should now behave exactly like the scene you entered in 'Scene Name' except with the added functionality in your decorator installer.

The PostInstallBindings method is useful when you want to override a binding in the main scene using 'Rebind'.  And PreInstallBindings is necessary if you want to inject data into the installers in the main scene. For a better example see the asteroids project that comes with Zenject (open 'AsteroidsDecoratorExample' scene).

Note also that Zenject validate (using CTRL+SHIFT+V or the menu item via Edit->Zenject->Validate Current Scene) also works with decorator scenes.

## <a id="auto-mocking-using-moq"></a>Auto-Mocking using Moq

One of the really cool features of DI is the fact that it makes testing code much, much easier.  This is because you can easily substitute one dependency for another by using a different Composition Root.  For example, if you only want to test a particular class (let's call it Foo) and don't care about testing its dependencies, you might write 'mocks' for them so that you can isolate Foo specifically.

    public class Foo
    {
        IWebServer _webServer;

        public Foo(IWebServer webServer)
        {
            _webServer = webServer;
        }

        public void Initialize()
        {
            ...
            var x = _webServer.GetSomething();
            ...
        }
    }

In this example, we have a class Foo that interacts with a web server to retrieve content.  This would normally be very difficult to test for the following reasons:

* You would have to set up an environment where it can properly connect to a web server (configuring ports, urls, etc.)
* Running the test could be slower and limit how much testing you can do
* The web server itself could contain bugs so you couldn't with certainty isolate Foo as the problematic part of the test
* You can't easily configure the values returned from the web server to test sending various inputs to the Foo class

However, if we create a mock class for IWebServer then we can address all these problems:

    public class MockWebServer : IWebServer
    {
        ...
    }

Then hook it up in our installer:

    Container.Bind<IWebServer>().ToSingle<MockWebServer>();

Then you can implement the fields of the IWebServer interface and configure them based on what you want to test on Foo. Hopefully You can see how this can make life when writing tests much easier.

Zenject also allows you to even avoid having to write the MockWebServer class in favour of using a library called "Moq" which does all the work for you.

Note that by default, Auto-mocking is not enabled in Zenject.  If you wish to use the auto-mocking feature then you need to go to your Zenject install directory and extract the contents of "Extras/ZenjectAutoMocking.zip".  Note also that AutoMocking is incompatible with webplayer builds, and you will also need to change your "Api Compatibility Level" from ".NET 2.0 Subset" to ".NET 2.0" (you can find this in PC build settings)

After extracting the auto mocking package it is just a matter of using the following syntax to mock out various parts of your project:

    Container.Bind<IFoo>().ToMock();

However, this approach will not allow you to take advantage of the advanced features of Moq.  For more advanced usages, see the documentation for Moq

## <a id="nested-containers"></a>Nested Containers / FallbackProvider

Every DiContainer exposes a FallbackProvider property, which by default is null.  In cases where the container is unable to resolve a dependency, the container will first try using the FallbackProvider before throwing a ZenjectResolveException.

This allows for the ability to define nested sub-containers by executing the following:

    Container.FallbackProvider = new DiContainerProvider(_nestedContainer);

Nested sub-containers can be useful in some rare cases.  For example, if you are creating a word processor it may be useful to have a sub-container for each tab that represents a separate document.  Nested sub-containers is also the way that the Global Composition Root works under the hood.  In the future we plan to add more support for this kind of thing.

There are other uses for FallbackProvider as well.

For example, if you are writing test code and want to automatically auto-mock missing dependencies, you can do the following:

    Container.FallbackProvider = new TransientMockProvider(Container);

Or, perhaps you wish to write custom logic to handle cases of missing dependencies.  You can do that as well, by writing a custom "Provider" class and setting it to be used as the FallbackProvider

## <a id="visualizing-object-graphs-automatically"></a>Visualizing Object Graphs Automatically

Zenject allows users to generate UML-style images of the object graphs for their applications.  You can do this simply by running your Zenject-driven app, then selecting from the menu `Assets -> Zenject -> Output Object Graph For Current Scene`.  You will be prompted for a location to save the generated image file.

Note that you will need to have graphviz installed for this to work (which you can find [here](http://www.graphviz.org/)).  You will be prompted to choose the location the first time.

The result is two files (Foo.dot and Foo.png).  The dot file is included in case you want to add custom graphviz commands.  As an example, this is the graph that is generated when run on the sample project:

However, admittedly, I personally haven't gotten a lot of mileage out of this feature.  When I have found it useful it's when I first encounter a lot of unfamiliar code.  Reading a visual diagram can be easier than reading the code in some cases.

<img src="UnityProject/Assets/Zenject/Main/ExampleObjectGraph.png?raw=true" alt="Example Object Graph" width="600px" height="127px"/>

## <a id="questions"></a>Frequently Asked Questions

* **<a id="faq-performance"></a>How is performance?**

    DI can affect start-up time when it builds the initial object graph. However it can also affect performance any time you instantiate new objects at run time.

    Zenject uses C# reflection which is typically slow, but in Zenject this work is cached so any performance hits only occur once for each class type.  In other words, Zenject avoids costly reflection operations by making a trade-off between performance and memory to ensure good performance.

## <a id="further-help"></a>Further Help

For general troubleshooting / support, please use the google group which you can find [here](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  Finally, you can also email me directly at svermeulen@modesttree.com

## <a id="release-notes"></a>Release Notes
1.18
* Added minor optimizations to reduce per-frame allocation to zero
* Fixed unit tests to be compatible with unity test tools
* Minor bug fix with scene decorators, GameObjectInstantiator.

1.17
* Bug fix.  Was not forwarding parameters correctly when instantiating objects from prefabs

1.16
* Removed the word 'ModestTree' from namespaces since Zenject is open source and not proprietary to the company ModestTree.

1.15
* Fixed bug with ToSingleFromPrefab which was causing it to create multiple instances when used in different bindings.

1.14
* Added flag to CompositionRoot for whether to inject into inactive game objects or ignore them completely
* Added BindAllInterfacesToSingle method to DiContainer
* Changed to call PostInject[] on children first when instantiating from prefab
* Added ILateTickable interface, which works just like ITickable or IFixedTickable for unity's LateUpdate event
* Added support for 'decorators', which can be used to add dependencies to another scene

1.13
Minor bug fix to global composition root.  Also fixed a few compiler warnings.

1.12
* Added Rebind<> method
* Changed Factories to use strongly typed parameters by default.  Also added ability to pass in null values as arguments as well as multiple instances of the same type
* Renamed _container to Container in the installers
* Added support for Global Composition Root to allow project-wide installers/bindings
* Added DiContainer.ToSingleMonoBehaviour method
* Changed to always include the StandardUnityInstaller in the CompositionRoot class.
* Changed TickableManager to not be a monobehaviour and receive its update from the UnityDependencyRoot instead
* Added IFixedTickable class to support unity FixedUpdate method

1.11
* Removed Fasterflect library to keep Zenject nice and lightweight (it was also causing issues on WP8)
* Fixed bug related to singletons + object graph validation. Changed the way IDisposables are handled to be closer to the way IInitializable and ITickable are handled. Added method to BinderUntyped.

1.10
* Added custom editor for the Installers property of CompositionRoot to make re-ordering easier

1.09

* Added support for nested containers
* Added ability to execute bind commands using Type objects rather than a generic type
* Changed the way IDisposable bindings work to be similar to how ITickable and IInitializable work
* Bug fixes

1.08

* Order of magnitude speed improvement by using more caching
* Minor change to API to use the As() method to specify identifiers
* Bug fixes

1.07

* Simplified API by removing the concept of modules in favour of just having installers instead (and add support for installers installing other installers)
* Bug fixes

1.06

* Introduced concept of scene installer, renamed installers 'modules'
* Bug fixes

## <a id="license"></a>License

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.


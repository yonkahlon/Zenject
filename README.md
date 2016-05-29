
<img src="UnityProject/Assets/Zenject/Documentation/ReadMe_files/ZenjectLogo.png?raw=true" alt="Zenject" width="600px" height="134px"/>

## Dependency Injection Framework for Unity3D

#### ----- NEW ----- If you like Zenject, you may also be interested in [Projeny](https://github.com/modesttree/projeny) (our other open source project)

## <a id="introduction"></a>Introduction

Zenject is a lightweight dependency injection framework built specifically to target Unity 3D.  It can be used to turn your Unity 3D application into a collection of loosely-coupled parts with highly segmented responsibilities.  Zenject can then glue the parts together in many different configurations to allow you to easily write, re-use, refactor and test your code in a scalable and extremely flexible way.

Tested in Unity 3D on the following platforms: 
* PC/Mac/Linux
* iOS
* Android
* Webplayer
* WebGL
* Windows Store (including 8.1, Phone 8.1, Universal 8.1 and Universal 10 - both .NET and IL2CPP backend)

IL2CPP is supported, however there are some gotchas - see <a href="#aot-support">here</a> for details

This project is open source.  You can find the official repository [here](https://github.com/modesttree/Zenject).

For general troubleshooting / support, please use the [zenject subreddit](http://www.reddit.com/r/zenject) or the [zenject google group](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  You can also follow [@Zenject](https://twitter.com/Zenject) on twitter for updates.  Finally, you can also email me directly at sfvermeulen@gmail.com

__Quick Start__:  If you are already familiar with dependency injection and are more interested in the syntax than anything else, you might want to start by reading the [Hello World Example](#hello-world-example) and then looking over the <a href="#cheatsheet">cheatsheet</a> at the bottom of this page, which shows a bunch of typical example cases of usage.  You may also be interested in reading some of the tests (see `Zenject/OptionalExtras/UnitTests` and `Zenject/OptionalExtras/IntegrationTests` directories)

## Table Of Contents

* <a href="#introduction">Introduction</a>
* <a href="#features">Features</a>
* <a href="#installation">Installation</a>
* <a href="#history">History</a>
* Dependency Injection
    * <a href="#theory">Theory</a>
    * <a href="#misconceptions">Misconceptions</a>
* Zenject API
    * <a href="#overview-of-the-zenject-api">Overview of the Zenject API</a>
        * <a href="#hello-world-example">Hello World Example</a>
        * <a href="#inject-methods">Inject Methods</a>
        * <a href="#binding">Binding</a>
        * <a href="#construction-methods">Construction Methods</a>
        * <a href="#list-bindings">List Bindings</a>
        * <a href="#list-bindings">List Bindings</a>
        * <a href="#optional-binding">Optional Binding</a>
        * <a href="#identifiers">Identifiers</a>
        * <a href="#singleton-identifiers">Singleton Identifiers</a>
        * <a href="#conditional-bindings">Conditional Bindings</a>
        * <a href="#itickable">ITickable</a>
        * <a href="#iinitializable-and-postinject">IInitializable and PostInject</a>
        * <a href="#implementing-idisposable">Implementing IDisposable</a>
        * <a href="#installers">Installers</a>
    * <a href="#di-guidelines--recommendations">Guidelines / Recommendations / Gotchas / Miscellaneous Tips and Tricks</a>
    * Advanced Features
        * <a href="#global-bindings">Global Bindings</a>
        * <a href="#update--initialization-order">Update Order And Initialization Order</a>
        * <a href="#object-graph-validation">Object Graph Validation</a>
        * <a href="#creating-objects-dynamically">Creating Objects Dynamically Using Factories</a>
        * <a href="#using-the-unity-inspector-to-configure-settings">Using the Unity Inspector To Configure Settings</a>
        * <a href="#game-object-factories">Game Object Factories</a>
        * <a href="#abstract-factories">Abstract Factories</a>
        * <a href="#custom-factories">Custom Factories</a>
        * <a href="#injecting-data-across-scenes">Injecting Data Across Scenes</a>
        * <a href="#scenes-decorator">Scenes Decorators</a>
        * <a href="#commands-and-signals">Commands And Signals</a>
        * <a href="#scene-bindings">Scene Bindings</a>
        * Using SubContainers / Facades
            * <a href="#advanced-factory-construction-using-subcontainers">Advanced Factory Construction Using SubContainers</a>
            * <a href="#sub-containers-and-facades">Sub-Containers and Facades</a>
            * <a href="#dynamic-facades">Creating Facade's Dynamically</a>
            * <a href="#non-monoBehaviour-facades-method">Non-MonoBehaviour Facades (By Method)</a>
            * <a href="#non-monoBehaviour-facades-installer">Non-MonoBehaviour Facades (By Installer)</a>
            * <a href="#non-monoBehaviour-facade-interfaces">Using ITickable / IInitializable / IDisposable with Non-MonoBehaviour Facades</a>
        * <a href="#zenject-order-of-operations">Zenject Order Of Operations</a>
        * <a href="#auto-mocking-using-moq">Auto-Mocking Using Moq</a>
* <a href="#questions">Frequently Asked Questions</a>
    * <a href="#isthisoverkill">Isn't this overkill?  I mean, is using statically accessible singletons really that bad?</a>
    * <a href="#aot-support">Does this work on AOT platforms such as iOS and WebGL?</a>
    * <a href="#faq-performance">How is Performance?</a>
    * <a href="#net-framework">Can I use .NET framework 4.0 and above?</a>
    * <a href="#howtousecoroutines">How do I use Unity style Coroutines in normal C# classes?</a>
    * <a href="#memorypools">How do I use Zenject with pools to minimize memory allocations?</a>
* <a href="#cheatsheet">Cheat Sheet</a>
* <a href="#further-help">Further Help</a>
* <a href="#release-notes">Release Notes</a>
* <a href="#license">License</a>

## <a id="features"></a>Features

* Injection into normal C# classes or MonoBehaviours
* Constructor injection (can tag constructor if there are multiple)
* Field injection
* Property injection
* Method injection
* Conditional binding (eg. by type, by name, etc.)
* Optional dependencies
* Support for building dynamic object graphs at runtime using factories
* Injection across different Unity scenes
* Convention based binding, based on class name, namespace, or any other criteria
* Support for global, project-wide bindings to add dependencies for all scenes
* "Scene Decorators" which allow adding functionality to a different scene without changing it directly
* Ability to validate object graphs at editor time including dynamic object graphs created via factories
* Nested Containers aka Sub-Containers
* Support for Commands and Signals
* Ability to automatically add bindings within your scene by dropping `ZenjectBinding` components on to game objects
* Auto-Mocking using the Moq library

## <a id="installation"></a>Installation

You can install Zenject using any of the following methods

1. From [Releases Page](https://github.com/modesttree/Zenject/releases). Here you can choose between the following:

    * **Zenject-WithAsteroidsDemo.vX.X.unitypackage** - This is equivalent to what you find in the Asset Store and contains the sample game "Asteroids" as part of the package.  All the source code for Zenject is included here.
    * **Zenject.vX.X.unitypackage** - Same as above except without the Sample project.
    * **Zenject-BinariesOnly.vX.X.unitypackage** - If you don't care about the source you can install Zenject from this package, which just contains a few DLL files.
    * **Zenject-NonUnity.vX.X.zip** - Use this if you want to use Zenject outside of Unity (eg. just as a normal C# project)

1. From the [Asset Store Page](http://u3d.as/content/modest-tree-media/zenject-dependency-injection/7ER)

    * Normally this should be the same as what you find in the [Releases section](https://github.com/modesttree/Zenject/releases), but may also be slightly out of date since Asset Store can take a week or so to review submissions sometimes.

1. From Source

    * You can also just clone this repo and copy the `UnityProject/Assets/Zenject` directory to your own Unity3D project.  In this case, make note of the folders underneath "OptionalExtras" and choose only the ones you want.

## <a id="history"></a>History

Unity is a fantastic game engine, however the approach that new developers are encouraged to take does not lend itself well to writing large, flexible, or scalable code bases.  In particular, the default way that Unity manages dependencies between different game components can often be awkward and error prone.

Having worked on non-unity projects that use dependency management frameworks (such as Ninject, which Zenject takes a lot of inspiration from), and without any alternative in Unity, I decided a custom framework was necessary.

Upon googling for solutions, I found a <a href="http://blog.sebaslab.com/ioc-container-for-unity3d-part-1/">series of great articles</a> by Sebastiano Mandalà outlining the problem.  Sebastiano even wrote a proof of concept and open sourced it, which became the basis for this library.

What follows in the next section is a general overview of Dependency Injection from my perspective.  I highly recommend seeking other resources for more information on the subject, as there are many (often more intelligent) people that have written on the subject.  In particular, I highly recommend anything written by Mark Seeman on the subject - in particular his book 'Dependency Injection in .NET'.

Finally, I will just say that if you don't have experience with DI frameworks, and are writing object oriented code, then trust me, you will thank me later!  Once you learn how to write properly loosely coupled code using DI, there is simply no going back.

## <a id="theory"></a>Theory

See <a href="https://www.youtube.com/watch?v=8ZCkEXv3QsQ">here</a> for a video that also serves as a nice introduction to the theory.

When writing an individual class to achieve some functionality, it will likely need to interact with other classes in the system to achieve its goals.  One way to do this is to have the class itself create its dependencies, by calling concrete constructors:

```csharp
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
```

This works fine for small projects, but as your project grows it starts to get unwieldy.  The class Foo is tightly coupled to class 'SomeService'.  If we decide later that we want to use a different concrete implementation then we have to go back into the Foo class to change it.

After thinking about this, often you come to the realization that ultimately, Foo shouldn't bother itself with the details of choosing the specific implementation of the service.  All Foo should care about is fulfilling its own specific responsibilities.  As long as the service fulfills the abstract interface required by Foo, Foo is happy.  Our class then becomes:

```csharp
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
```

This is better, but now whatever class is creating Foo (let's call it Bar) has the problem of filling in Foo's extra dependencies:

```csharp
public class Bar
{
    public void DoSomething()
    {
        var foo = new Foo(new SomeService());
        foo.DoSomething();
        ...
    }
}
```

And class Bar probably also doesn't really care about what specific implementation of SomeService Foo uses.  Therefore we push the dependency up again:

```csharp
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
```

So we find that it is useful to push the responsibility of deciding which specific implementations of which classes to use further and further up in the 'object graph' of the application.  Taking this to an extreme, we arrive at the entry point of the application, at which point all dependencies must be satisfied before things start.  The dependency injection lingo for this part of the application is called the 'composition root'.  It would normally look like this:

```csharp
var service = new SomeService();
var foo = new Foo(service);
var bar = new Bar(foo);

.. etc.
```

DI frameworks such as Zenject simply help automate this process of creating and handing out all these concrete dependencies

## <a id="misconceptions"></a>Misconceptions

There are many misconceptions about DI, due to the fact that it can be tricky to fully wrap your head around at first.  It will take time and experience before it fully 'clicks'.

As shown in the above example, DI can be used to easily swap different implementations of a given interface (in the example this was ISomeService).  However, this is only one of many benefits that DI offers.

More important than that is the fact that using a dependency injection framework like Zenject allows you to more easily follow the '[Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle)'.  By letting Zenject worry about wiring up the classes, the classes themselves can just focus on fulfilling their specific responsibilities.

<a id="overusinginterfaces"></a>Another common mistake that people new to DI make is that they extract interfaces from every class, and use those interfaces everywhere instead of using the class directly.  The goal is to make code more loosely coupled, so it's reasonable to think that being bound to an interface is better than being bound to a concrete class.  However, in most cases the various responsibilities of an application have single, specific classes implementing them, so using interfaces in these cases just adds unnecessary maintenance overhead.  Also, concrete classes already have an interface defined by their public members.  A good rule of thumb instead is to only create interfaces when the class has more than one implementation.  This is known, by the way, as the [Reused Abstraction Principle](http://codemanship.co.uk/parlezuml/blog/?postid=934))

Other benefits include:

* Testability - Writing automated unit tests or user-driven tests becomes very easy, because it is just a matter of writing a different 'composition root' which wires up the dependencies in a different way.  Want to only test one subsystem?  Simply create a new composition root.  Zenject also has some support for avoiding code duplication in the composition root itself (using Installers - described below).
* Refactorability - When code is loosely coupled, as is the case when using DI properly, the entire code base is much more resilient to changes.  You can completely change parts of the code base without having those changes wreak havoc on other parts.
* Encourages modular code - When using a DI framework you will naturally follow better design practices, because it forces you to think about the interfaces between classes.

Also see <a href="#isthisoverkill">here</a> for more justification for using a DI framework.

## <a id="overview-of-the-zenject-api"></a>Overview Of The Zenject API

What follows is a general overview of how DI patterns are applied using Zenject.  For further documentation I highly recommend the sample projects (which you can find by opening "Zenject/OptionalExtras/SampleGame1" or "Zenject/OptionalExtras/SampleGame2").  I would recommend reading those to understand how this gets applied in practice.

You may also find the <a href="#cheatsheet">cheatsheet</a> at the bottom of this page helpful in understanding some typical usage scenarios.

The tests may also be helpful to show usage for each specific feature (which you can find at `Zenject/OptionalExtras/UnitTests` and `Zenject/OptionalExtras/IntegrationTests`)

## <a id="hello-world-example"></a>Hello World Example

```csharp
using Zenject;
using UnityEngine;
using System.Collections;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<string>().FromInstance("Hello World!");
        Container.Bind<TestRunner>().NonLazy();
    }
}

public class TestRunner
{
    public TestRunner(string message)
    {
        Debug.Log(message);
    }
}
```

You can run this example by doing the following:

* Create a new scene in Unity
* Right Click inside the Hierarchy tab and select `Zenject -> Scene Context`
* Right Click in a folder within the Scene Heirarchy and Choose `Create -> Zenject -> MonoInstaller`.  Name it TestInstaller.cs.  (Note that you can also just directly create this file too without using this template).
* Add your TestInstaller script to the scene (as its own GameObject or on the same GameObject as the SceneContext, it doesn't matter)
* Add a reference to your TestInstaller to the properties of the SceneContext by adding a new row in the inspector of the "Installers" property (Increase "Size" to 1) and then dragging the TestInstaller GameObject to it
* Open up TestInstaller and paste the above code into it
* Validate your scene by either selecting Edit -> Zenject -> Validate Current Scene or hitting CTRL+SHIFT+V.  (note that this step isn't necessary but good practice to get into)
* Run
* Observe unity console for output

The SceneContext MonoBehaviour is the entry point of the application, where Zenject sets up all the various dependencies before kicking off your scene.  To add content to your Zenject scene, you need to write what is referred to in Zenject as an 'Installer', which declares all the dependencies used in your scene and their relationships with each other.  All dependencies that are marked as "NonLazy" are automatically created at this point, as well as any dependencies that implement the standard Zenject interfaces such as `IInitializable`, `ITickable`, etc.  If the above doesn't make sense to you yet, keep reading!

## <a id="inject-methods"></a>Inject Methods

There are many different ways of binding types on the container, which are documented in the <a href="#binding">next section</a>.  There are also several ways of having these dependencies injected into your classes. These are:

1 - **Constructor Injection**

```csharp
public class Foo
{
    IBar _bar;

    public Foo(IBar bar)
    {
        _bar = bar;
    }
}
```

2 - **Field Injection**

```csharp
public class Foo
{
    [Inject]
    IBar _bar;
}
```

Field injection occurs immediately after the constructor is called.  All fields that are marked with the [Inject] attribute are looked up in the container and given a value.  Note that these fields can be private or public and injection will still occur.

3 - **Property Injection**

```csharp
public class Foo
{
    [Inject]
    public IBar Bar
    {
        get;
        private set;
    }
}
```

Property injection works the same as field injection except is applied to C# properties.  Just like fields, the setter can be private or public in this case.

4 - **Method Injection**

```csharp
public class Foo
{
    IBar _bar;
    Qux _qux;

    [Inject]
    public Init(IBar bar, Qux qux)
    {
        _bar = bar;
        _qux = qux;
    }
}
```

Method Inject injection works very similar to constructor injection.  The methods marked with [Inject] are called after all other dependencies have been resolved. Because they are the last injection type triggered, they can be used to execute initialization logic.  You can also leave the parameter list empty if you just want to do some initialization logic only.

Note that there can be any number of inject methods.  In this case, they are called in the order of Base class to Derived class.  This can be useful to avoid the need to forward many dependencies from derived classes to the base class via constructor parameters, while also guaranteeing that the base class inject methods complete first, just like how constructors work.

Note that the dependencies that you receive via inject methods should themselves have already been injected.  This can be important if you use inject methods to perform some basic initialization, since you may need the given dependencies to themselves be initialized.

Using [Inject] methods to inject dependencies is the recommended approach for MonoBehaviours, since MonoBehaviours cannot have constructors.

**Recommendations**

* Best practice is to prefer constructor injection or method injection to field or property injection.
    * Constructor injection forces the dependency to only be resolved once, at class creation, which is usually what you want.  In most cases you don't want to expose a public property for your initial dependencies because this suggests that it's open to changing.
    * Constructor injection guarantees no circular dependencies between classes, which is generally a bad thing to do.  You can do this however using method injection or field injection if necessary.
    * Constructor/Method injection is more portable for cases where you decide to re-use the code without a DI framework such as Zenject.  You can do the same with public properties but it's more error prone (it's easier to forget to initialize one field and leave the object in an invalid state)
    * Finally, Constructor/Method injection makes it clear what all the dependencies of a class are when another programmer is reading the code.  They can simply look at the parameter list of the method.

## <a id="binding"></a>Binding

Every dependency injection framework is ultimately just a framework to bind types to instances.

In Zenject, dependency mapping is done by adding bindings to something called a container.  The container should then 'know' how to create all the object instances in your application, by recursively resolving all dependencies for a given object.

When the container is asked to construct an instance of a given type, it uses C# reflection to find the list of constructor arguments, and all fields/properties that are marked with an [Inject] attribute.  It then attempts to resolve each of these required dependencies, which it uses to call the constructor and create the new instance.

Each Zenject application therefore must tell the container how to resolve each of these dependencies, which is done via Bind commands.  For example, given the following class:

```csharp
public class Foo
{
    IBar _bar;

    public Foo(IBar bar)
    {
        _bar = bar;
    }
}
```

You can wire up the dependencies for this class with the following bindings:

```csharp
Container.Bind<Foo>().AsSingle();
Container.Bind<IBar>().To<Bar>().AsSingle();
```

This tells Zenject that every class that requires a dependency of type Foo should use the same instance, which it will automatically create when needed.  And similarly, any class that requires the IBar interface (like Foo) will be given the same instance of type Bar.

The full format for the bind command is the following.  Note that in most cases you will not use all of these methods and that they all have logical defaults when unspecified

<pre>
Container.Bind&lt;<b>ContractType</b>&gt;()
    .To&lt;<b>ResultType</b>&gt;()
    .From<b>ConstructionMethod</b>()
    .As<b>Scope</b>()
    .WithArguments(<b>Arguments</b>)
    .When(<b>Condition</b>)
    .InheritInSubContainers()
    .NonLazy();
</pre>

Where:

* **ContractType** = The type that you are creating a binding for.

    * This value will correspond to the type of the field/parameter that is being injected.

* **ResultType** = The type to bind to.

    * Default: **ContractType**
    * This type must either to equal to **ContractType** or derive from **ContractType**.  If unspecified, it assumes ToSelf(), which means that the **ResultType** will be the same as the **ContractType**.  This value will be used by whatever is given as the **ConstructionMethod** to retrieve an instance of this type

* **ConstructionMethod** = The method by which an instance of **ResultType** is created/retrieved.  See <a href="#binding">this section</a> for more details on the various construction methods.

    * Default: FromNew()
    * Examples: eg. FromGetter, FromMethod, FromPrefab, FromResolve, FromSubContainerResolve, FromInstance, etc.

* **Scope** = This value determines how often (or if at all) the generated instance is re-used across multiple injections.

    * Default: AsTransient
    * It can be one of the following:
        1. AsTransient - Will not re-use the instance at all.  Every time **ContractType** is requested, the DiContainer will return a brand new instance of type **ResultType**
        2. AsCached - Will re-use the same instance of **ResultType** every time **ContractType** is requested, which it will lazily generate upon first use
        3. AsSingle - Will re-use the same instance of **ResultType** across the entire DiContainer, which it will lazily generate upon first use.  It can be thought of as a stronger version of AsCached, because it allows you to bind to the same instance across multiple bind commands.  It will also ensure that there is only ever exactly one instance of **ResultType** in the DiContainer (ie. it will enforce **ResultType** to be a 'Singleton' hence the name).

    * In most cases, you will likely want to just use AsSingle, however AsTransient and AsCached have their uses too.
    * To illustrate the difference between the different scope types, see the following example:
        ```csharp
        public interface IBar
        {
        }

        public class Bar : IBar
        {
        }

        public class Foo()
        {
            public Foo(Bar bar)
            {
            }
        }
        ```

        ```csharp
        // This will cause every instance of Foo to be given a brand new instance of Bar
        Container.Bind<Bar>().AsTransient();
        ```

        ```csharp
        // This will cause every instance of Foo to be given the same instance of Bar
        Container.Bind<Bar>().AsCached();
        ```

        ```csharp
        public class Qux()
        {
            public Qux(IBar bar)
            {
            }
        }
        ```

        ```csharp
        // This will cause both Foo and Qux to get different instances of type Bar
        // However, every instance of Foo will be given the the same instance of type Bar
        // and similarly for Qux
        Container.Bind<Bar>().AsCached();
        Container.Bind<IBar>().To<Bar>().AsCached();
        ```

        ```csharp
        // This will cause both Foo and Qux to get the same instance of type Bar
        Container.Bind<Bar>().AsSingle();
        Container.Bind<IBar>().To<Bar>().AsSingle();
        ```

## <a id="construction-methods"></a>Construction Methods

1. **FromNew** - Create via the C# new operator. This is the default if no construction method is specified.

    ```csharp
    // These are both the same
    Container.Bind<Foo>();
    Container.Bind<Foo>().FromNew();
    ```

1. **FromInstance** - Use a given instance

    ```csharp
    Container.Bind<Foo>().FromInstance(new Foo());

    // You can also use this short hand which just takes ContractType from the parameter type
    Container.BindInstance(new Foo());

    // This is also what you would typically use for primitive types
    Container.BindInstance(5.13f);
    Container.BindInstance("foo");
    ```

1. **FromMethod** - Create via a custom method

    ```csharp
    Container.Bind<Foo>().FromMethod(SomeMethod);

    Foo SomeMethod(InjectContext context)
    {
        ...
        return new Foo();
    }
    ```

1. **FromComponent** - Create as a new component on an existing game object.  **ResultType** must derive from UnityEngine.MonoBehaviour / UnityEngine.Component in this case

    ```csharp
    Container.Bind<Foo>().FromComponent(someGameObject);
    ```

1. **FromGameObject** - Create as a new component on new game object.  **ResultType** must derive from UnityEngine.MonoBehaviour / UnityEngine.Component in this case

    ```csharp
    Container.Bind<Foo>().FromGameObject();
    ```

1. **FromPrefab** - Create by instantiating the given prefab and then searching it for type **ResultType**

    ```csharp
    Container.Bind<Foo>().FromPrefab(somePrefab);
    ```

1. **FromPrefabResource** - Create by instantiating the prefab at the given resource path and then searching it for type **ResultType**

    ```csharp
    Container.Bind<Foo>().FromPrefabResource("Some/Path/Foo");
    ```

1. **FromResource** - Create by calling the Unity3d function `Resources.Load` for **ResultType**.  This can be used to load any type that `Resources.Load` can load, such as textures, sounds, prefabs, custom classes deriving from ScriptableObject, etc.

    ```csharp
    public class Foo : ScriptableObject
    {
    }

    Container.Bind<Foo>().FromResource("Some/Path/Foo");
    ```

1. **FromResolve** - Get instance by doing another lookup on the container (in other words, calling `DiContainer.Resolve<ResultType>()`).  Note that for this to work, **ResultType** must be bound in a separate bind statement.  This construction method can be especially useful when you want to bind an interface to another interface, as shown in the below example

    ```csharp
    public interface IFoo
    {
    }

    public interface IBar : IFoo
    {
    }

    public class Foo : IBar
    {
    }

    Container.Bind<IFoo>().To<IBar>().FromResolve();
    Container.Bind<IBar>().To<Foo>();
    ```

1. **FromFactory** - Create instance using a custom factory class.  This construction method is similar to `FromMethod` except can be cleaner in cases where the logic is more complicated or requires dependencies (the factory itself can have dependencies injected)

    ```csharp
    class FooFactory : IFactory<Foo>
    {
        public Foo Create()
        {
            // ...
            return new Foo();
        }
    }

    Container.Bind<Foo>().FromFactory<FooFactory>()
    ```

1. **FromResolveGetter<ObjectType>** - Get instance from the property of another dependency which is obtained by doing another lookup on the container (in other words, calling `DiContainer.Resolve<ObjectType>()` and then accessing a value on the returned instance of type **ResultType**).  Note that for this to work, **ObjectType** must be bound in a separate bind statement.

    ```csharp
    public class Bar
    {
    }

    public class Foo
    {
        public Bar GetBar()
        {
            return new Bar();
        }
    }

    Container.Bind<Foo>();
    Container.Bind<Bar>().FromResolveGetter<Foo>(x => x.GetBar());
    ```

1. **FromSubContainerResolve** - Get **ResultType** by doing a lookup on a subcontainer.  Note that for this to work, the sub-container must have a binding for **ResultType**.  This approach can be very powerful, because it allows you to group related dependencies together inside a mini-container, and then expose only certain classes (aka <a href="https://en.wikipedia.org/wiki/Facade_pattern">"Facades"</a>) to operate on this group of dependencies at a higher level.  For more details on using sub-containers, see <a href="">this section</a>.  There are 4 different ways to define the subcontainer:

    1. **ByMethod** - Initialize the subcontainer by using a method.

        ```csharp
        Container.Bind<Foo>().FromSubContainerResolve().ByMethod(InstallFooFacade);

        void InstallFooFacade(DiContainer subContainer)
        {
            subContainer.Bind<Foo>();
        }
        ```

    1. **ByInstaller** - Initialize the subcontainer by using a class derived from `Installer`.  This can be a cleaner alternative than using `ByMethod`, especially if you need to inject data into the installer itself.

        ```csharp
        Container.Bind<Foo>().FromSubContainerResolve().ByInstaller<FooFacadeInstaller>();

        class FooFacadeInstaller : Installer
        {
            public override void InstallBindings()
            {
                Container.Bind<Foo>();
            }
        }
        ```

    1. **ByPrefab** - Initialize subcontainer using a prefab.  Note that the prefab must contain a `GameObjectContext` component attached to the root game object.

        ```csharp
        Container.Bind<Foo>().FromSubContainerResolve().ByPrefab(MyPrefab);

        // Assuming here that this installer is added to the GameObjectContext at the root
        // of the prefab
        class FooFacadeInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                Container.Bind<Foo>();
            }
        }
        ```

    1. **ByPrefabResource** - Initialize subcontainer using a prefab obtained via `Resources.Load`.  Note that the prefab must contain a `GameObjectContext` component attached to the root game object.

        ```csharp
        Container.Bind<Foo>().FromSubContainerResolve().ByPrefabResource("Path/To/MyPrefab");
        ```

## <a id="list-bindings"></a>List Bindings

When Zenject finds multiple bindings for the same type, it interprets that to be a list.  So, in the example code below, Bar would get a list containing a new instance of Foo1, Foo2, and Foo3:

```csharp
// In an installer somewhere
Container.Bind<IFoo>().To<Foo1>().AsSingle();
Container.Bind<IFoo>().To<Foo2>().AsSingle();
Container.Bind<IFoo>().To<Foo3>().AsSingle();

...

public class Bar
{
    public Bar(List<IFoo> foos)
    {
    }
}
```

Also worth noting is that if you try and declare a single dependency of IFoo (like Bar below) and there are multiple bindings for it, then Zenject will throw an exception, since Zenject doesn't know which instance of IFoo to use.

```csharp
public class Bar
{
    public Bar(IFoo foo)
    {
    }
}
```

Also, if the empty list is valid, then you should mark your List constructor parameter (or [Inject] field) as optional (see <a href="#optional-binding">here</a> for details).

## <a id="optional-binding"></a>Optional Binding

You can declare some dependencies as optional as follows:

```csharp
public class Bar
{
    public Bar(
        [InjectOptional]
        IFoo foo)
    {
        ...
    }
}
...

// You can comment this out and it will still work
Container.Bind<IFoo>().AsSingle();
```

If an optional dependency is not bound in any installers, then it will be injected as null.

If the dependency is a primitive type (eg. int, float, struct) then it will be injected with its default value (eg. 0 for ints).

You may also assign an explicit default using the standard C# way such as:

```csharp
public class Bar
{
    public Bar(int foo = 5)
    {
        ...
    }
}
...

// Can comment this out and 5 will be used instead
Container.BindInstance(1);
```

Note also that the `[InjectOptional]` is not necessary in this case, since it's already implied by the default value.

Alternatively, you can define the primitive parameter as nullable, and perform logic depending on whether it is supplied or not, such as:

```csharp
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

// Can comment this out and it will use 5 instead
Container.BindInstance(1);
```

## <a id="identifiers"></a>Identifiers

You can also give an ID to your binding if you need to have distinct bindings for the same type, and you don't want it to just result in a `List<>`.  For example:

```csharp
Container.Bind<IFoo>().WithId("foo").To<Foo1>().AsSingle();
Container.Bind<IFoo>().To<Foo2>().AsSingle();

...

public class Bar1
{
    [Inject(Id = "foo")]
    IFoo _foo;
}

public class Bar2
{
    [Inject]
    IFoo _foo;
}
```

In this example, the `Bar1` class will be given an instance of `Foo1`, and the `Bar2` class will use the default version of `IFoo` which is bound to `Foo2`.

Note also that you can do the same thing for constructor/inject-method arguments as well:

```csharp
public class Bar
{
    Foo _foo;

    public Bar(
        [Inject(Id = "foo")] 
        Foo foo)
    {
    }
}
```

In many cases, the ID is created as a string, however you can actually use any type you like for this.  For example, it's sometimes useful to use an enum instead:

```csharp
enum Cameras
{
    Main,
    Player,
}

Container.Bind<Camera>().WithId(Cameras.Main).FromInstance(MyMainCamera);
Container.Bind<Camera>().WithId(Cameras.Player).FromInstance(MyPlayerCamera);
```

You can also use custom types, as long as they implement the `Equals` operator.

## <a id="non-generic-bindings"></a>Non Generic bindings

In some cases you may not know the exact type you want to bind at compile time.  In these cases you can use the overload of the `Bind` method which takes a `System.Type` value instead of a generic parameter.

```csharp
// These two lines will result in the same behaviour
Container.Bind(typeof(Foo));
Container.Bind<Foo>();
```

Note also that when using non generic bindings, you can pass multiple arguments:

```csharp
Container.Bind(typeof(Foo), typeof(Bar), typeof(Qux)).AsSingle();

// The above line is equivalent to these three:
Container.Bind<Foo>().AsSingle();
Container.Bind<Bar>().AsSingle();
Container.Bind<Qux>().AsSingle();
```

The same goes for the To method:

```csharp
Container.Bind<IFoo>().To(typeof(Foo), typeof(Bar)).AsSingle();

// The above line is equivalent to these two:
Container.Bind<IFoo>().To<Foo>().AsSingle();
Container.Bind<IFoo>().To<Bar>().AsSingle();
```

You can also do both:

```csharp
Container.Bind(typeof(IFoo), typeof(IBar)).To(typeof(Foo1), typeof(Foo2)).AsSingle();

// The above line is equivalent to these:
Container.Bind<IFoo>().To<Foo>().AsSingle();
Container.Bind<IFoo>().To<Bar>().AsSingle();
Container.Bind<IBar>().To<Foo>().AsSingle();
Container.Bind<IBar>().To<Bar>().AsSingle();
```

This can be especially useful when you have a class that implements multiple interfaces:

```csharp
Container.Bind(typeof(ITickable), typeof(IInitializable), typeof(IDisposable)).To<Foo>().AsSingle();
```

There is also a built-in shortcut method for this to make this even easier:

```csharp
Container.BindAllInterfaces<Foo>().To<Foo>().AsSingle();
```

## <a id="convention-based-bindings"></a>Convention Based Binding

Convention based binding can come in handy in any of the following scenarios:

- You want to define a naming convention that determines how classes are bound to the container (eg. using a prefix, suffix, or regex)
- You want to use custom attributes to determine how classes are bound to the container
- You want to automatically bind all classes that implement a given interface within a given namespace or assembly

Using "convention over configuration" can allow you to define a framework that other programmers can use to quickly and easily get things done, instead of having to explicitly add every binding within installers.  This is the philosophy that is followed by frameworks like Ruby on Rails, ASP.NET MVC, Django, etc.

They are specified in a similar way to <a href="#non-generic-bindings">Non Generic bindings</a>, except instead of giving a list of types to the `Bind()` and `To()` methods, you describe the convention using a Fluent API.  For example, to bind `IFoo` to every class that implements it in the entire codebase:

```csharp
Container.Bind<IFoo>().To(x => x.AllTypes().DerivingFrom<IFoo>());
```

Note that you can use the same Fluent API in the `Bind()` method as well, and you can also use it in both `Bind()` and `To()` at the same time.

For more examples see the <a href="#convention-binding-examples">examples</a> section below.  The full format is as follows:

<pre>
x.<b>InitialList</b>().<b>AssemblySources</b>().<b>Conditional</b>()
</pre>

###Where:

* **InitialList** = The initial list of types to use for our binding.  This list will be filtered by the given **Conditional**s.  It can be one of the following (fairly self explanatory) methods:

    1. **AllTypes**
    1. **AllNonAbstractClasses**
    1. **AllAbstractClasses**
    1. **AllInterfaces**
    1. **AllClasses**

* **AssemblySources** = The list of assemblies to search for types when populating **InitialList**.  It can be one of the following:

    1. **FromAllAssemblies** - Look up types in all loaded assemblies.  This is the default when unspecified.
    1. **FromAssemblyContaining**<T> - Look up types in whatever assembly the type `T` is in
    1. **FromAssembliesContaining**(type1, type2, ..) - Look up types in all assemblies that contains any of the given types
    1. **FromThisAssembly** - Look up types only in the assembly in which you are calling this method
    1. **FromAssembly**(assembly) - Look up types only in the given assembly
    1. **FromAssemblies**(assembly1, assembly2, ...) - Look up types only in the given assemblies
    1. **FromAssembliesWhere**(predicate) - Look up types in all assemblies that match the given predicate

* **Conditional** = The filter to apply to the list of types given by **InitialList**.  Note that you can chain as many of these together as you want, and they will all be applied to the initial list in sequence.  It can be one of the following:

    1. **DerivingFrom**<T> - Only match types deriving from `T`
    1. **DerivingFromOrEqual**<T> - Only match types deriving from or equal to `T`
    1. **WithPrefix**(value) - Only match types with names that start with `value`
    1. **WithSuffix**(value) - Only match types with names that end with `value`
    1. **WithAttribute**<T> - Only match types that have the attribute `[T]` above their class declaration
    1. **WithoutAttribute**<T> - Only match types that do not have the attribute `[T]` above their class declaration
    1. **WithAttributeWhere**<T>(predicate) - Only match types that have the attribute `[T]` above their class declaration AND in which the given predicate returns true when passed the attribute.  This is useful so you can use data given to the attribute to create bindings
    1. **InNamespace**(value) - Only match types that are in the given namespace
    1. **InNamespaces**(value1, value2, etc.) - Only match types that are in any of the given namespaces
    1. **MatchingRegex**(pattern) - Only match types that match the given regular expression
    1. **Where**(predicate) - Finally, you can also add any kind of conditional logic you want by passing in a predicate that takes a `Type` parameter

###<a id="convention-binding-examples"></a>Examples:

Note that you can chain together any combination of the below conditionals in the same binding.  Also note that since we aren't specifying an assembly here, Zenject will search within all loaded assemblies.

1. Bind `IFoo` to every class that implements it in the entire codebase:

    ```csharp
    Container.Bind<IFoo>().To(x => x.AllTypes().DerivingFrom<IFoo>());
    ```

    Note that this will also have the same result:

    ```csharp
    Container.Bind<IFoo>().To(x => x.AllNonAbstractTypes());
    ```

    This is because Zenject will skip any bindings in which the concrete type does not actually derive from the base type.  Also note that in this case we have to make sure we use `AllNonAbstractTypes` instead of just `AllTypes`, to ensure that we don't bind `IFoo` to itself

1. Bind an interface to all classes implementing it within a given namespace

    ```csharp
    Container.Bind<IFoo>().To(x => x.AllTypes().DerivingFrom<IFoo>().InNamespace("MyGame.Foos"));
    ```

1. Auto-bind `IController` every class that has the suffix "Controller" (as is done in ASP.NET MVC):

    ```csharp
    Container.Bind<IController>().To(x => x.AllNonAbstractTypes().WithSuffix("Controller"));
    ```

    You could also do this using `MatchingRegex`:

    ```csharp
    Container.Bind<IController>().To(x => x.AllNonAbstractTypes().MatchingRegex("Controller$"));
    ```

1. Bind all types with the prefix "Widget" and inject into Foo

    ```csharp
    Container.Bind<object>().To(x => x.AllNonAbstractTypes().WithPrefix("Widget")).WhenInjectedInto<Foo>();
    ```

1. Auto-bind the interfaces that are used by every type in a given namespace

    ```csharp
    Container.Bind(x => x.AllInterfaces())
        .To(x => x.AllNonAbstractClasses().InNamespace("MyGame.Things"));
    ```

    This is equivalent to calling `Container.BindAllInterfaces<T>().To<T>()` for every type in the namespace "MyGame.Things".  This works because, as touched on above, Zenject will skip any bindings in which the concrete type does not actually derive from the base type.  So even though we are using `AllInterfaces` which matches every single interface in every single loaded assembly, this is ok because it will not try and bind an interface to a type that doesn't implement this interface.

## <a id="unbind-rebind"></a>Unbind / Rebind

1. Unbind - Remove binding from container.

    ```csharp
    Container.Bind<IFoo>().To<Foo>();

    // This will nullify the above statement
    Container.Unbind<IFoo>();
    ```

1. Rebind - Override an existing binding with a new one.  This is equivalent to calling unbind with the given type and then immediately calling bind afterwards.

    ```csharp
    Container.Bind<IFoo>().To<Foo>();

    // 
    Container.Rebind<IFoo>().To<Bar>();
    ```

## <a id="singleton-identifiers"></a>Singleton Identifiers

In addition to <a href="#identifiers">normal identifiers</a>, you can also assign an identifer to a given singleton.

This allows you to force Zenject to create multiple singletons instead of just one, since otherwise the singleton is uniquely identified based on the type given as generic argument to the `To<>` method.  So for example:

```csharp
Container.Bind<IFoo>().To<Foo>().AsSingle();
Container.Bind<IBar>().To<Foo>().AsSingle();
Container.Bind<IQux>().To<Qux>().AsSingle();
```

In the above code, both `IFoo` and `IBar` will be bound to the same instance.  Only one instance of Foo will be created.

```csharp
Container.Bind<IFoo>().To<Foo>().AsSingle("foo1");
Container.Bind<IBar>().To<Foo>().AsSingle("foo2");
```

In this case however, two instances will be created.

Another use case for this is to allow creating multiple singletons from the same prefab.  For example, Given the following:

```csharp
Container.Bind<Foo>().FromPrefab(MyPrefab).AsSingle();
Container.Bind<Bar>().FromPrefab(MyPrefab).AsSingle();
```

It will only instantiate the prefab MyPrefab once, since the singleton is identified solely by the prefab when using `FromPrefab`.  The concrete type given can be interpreted as "Search the instantiated prefab for this component".  But, if instead you want Zenject to instantiate a new instance of the prefab for each `FromPrefab` binding, then you can do that as well by supplying an identifier to the `AsSingle` function like this:

```csharp
Container.Bind<Foo>().FromPrefab(MyPrefab).AsSingle("foo");
Container.Bind<Bar>().FromPrefab(MyPrefab).AsSingle("bar");
```

Now two instances of the prefab will be created.

## <a id="conditional-bindings"></a>Conditional Bindings

In many cases you will want to restrict where a given dependency is injected.  You can do this using the following syntax:

```csharp
Container.Bind<IFoo>().To<Foo1>().AsSingle().WhenInjectedInto<Bar1>();
Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Bar2>();
```

Note that `WhenInjectedInto` is simple shorthand for the following, which uses the more general `When()` method:

```csharp
Container.Bind<IFoo>().To<Foo>().AsSingle().When(context => context.ObjectType == typeof(Bar));
```

The InjectContext class (which is passed as the `context` parameter above) contains the following information that you can use in your conditional:

* `Type ObjectType` - The type of the newly instantiated object, which we are injecting dependencies into.  Note that this is null for root calls to `Resolve<>` or `Instantiate<>`
* `object ObjectInstance` - The newly instantiated instance that is having its dependencies filled.  Note that this is only available when injecting fields or into `[Inject]` methods and null for constructor parameters
* `string Identifier` - This will be null in most cases and set to whatever is given as a parameter to the `[Inject]` attribute.  For example, `[Inject(Id = "foo")] _foo` will result in `Identifier` being equal to the string "foo".
* `string ConcreteIdentifier` - This will be null in most cases and set to whatever is given as the string identifier to the `AsSingle` method.
* `string MemberName` - The name of the field or parameter that we are injecting into.  This can be used, for example, in the case where you have multiple constructor parameters that are strings.  However, using the parameter or field name can be error prone since other programmers may refactor it to use a different name.  In many cases it's better to use an explicit identifier
* `Type MemberType` - The type of the field or parameter that we are injecting into.
* `InjectContext ParentContext` - This contains information on the entire object graph that precedes the current class being created.  For example, dependency A might be created, which requires an instance of B, which requires an instance of C.  You could use this field to inject different values into C, based on some condition about A.  This can be used to create very complex conditions using any combination of parent context information.  Note also that `ParentContext.MemberType` is not necessarily the same as ObjectType, since the ObjectType could be a derived type from `ParentContext.MemberType`
* `bool Optional` - True if the `[InjectOptional]` parameter is declared on the field being injected

## <a id="itickable"></a>ITickable

In many cases it is preferable to avoid the extra weight of MonoBehaviours in favour of just normal C# classes.  Zenject allows you to do this much more easily by providing interfaces that mirror functionality that you would normally need to use a MonoBehaviour for.

For example, if you have code that needs to run per frame, then you can implement the `ITickable` interface:

```csharp
public class Ship : ITickable
{
    public void Tick()
    {
        // Perform per frame tasks
    }
}
```

Then it's just a matter of including the following in one of your installers:

```csharp
Container.Bind<ITickable>().To<Ship>().AsSingle();
```

Or if you don't want to have to remember which interfaces the Ship implements:

```csharp
Container.BindAllInterfaces<Ship>().To<Ship>().AsSingle();
```

Note that the order that Tick() is called on all ITickables is also configurable, as outlined <a href="#update--initialization-order">here</a>.

Also note that there is also interfaces `ILateTickable` and `IFixedTickable` which work similarly for the other unity update methods.

## <a id="iinitializable-and-postinject"></a>IInitializable and Inject Methods

If you have some initialization that needs to occur on a given object, you could include this code in the constructor.  However, this means that the initialization logic would occur in the middle of the object graph being constructed, so it may not be ideal.

One alternative is to implement `IInitializable`, and then perform initialization logic in an `Initialize()` method.  This Initialize method would then be called after the entire object graph is constructed and all constructors have been called.

Note that the constructors for the initial object graph are called during Unity's Awake event, and that the `IInitializable.Initialize` methods are called immediately on Unity's Start event.  Using `IInitializable` as opposed to a constructor is therefore more in line with Unity's own recommendations, which suggest that the Awake phase be used to set up object references, and the Start phase should be used for more involved initialization logic.

This can also be better than using constructors or `[Inject]` methods because the initialization order is customizable in a similar way to `ITickable`, as explained <a href="#update--initialization-order">here</a>.

```csharp
public class Ship : IInitializable
{
    public void Initialize()
    {
        // Initialize your object here
    }
}
```

`IInitializable` works well for start-up initialization, but what about for objects that are created dynamically via factories?  (see <a href="#creating-objects-dynamically">this section</a> for what I'm referring to here).  For these cases you will most likely want to use an `[Inject]` method:

```csharp
public class Foo
{
    [Inject]
    IBar _bar;

    [Inject]
    public void Initialize()
    {
        ...
        _bar.DoStuff();
        ...
    }
}
```

## <a id="implementing-idisposable"></a>Implementing IDisposable

If you have external resources that you want to clean up when the app closes, the scene changes, or for whatever reason the context object is destroyed, you can simply declare your class as `IDisposable` like below:

```csharp
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
```

Then in your installer you can include:

```csharp
Container.Bind<Logger>().AsSingle();
Container.Bind<IInitializable>().To<Logger>().AsSingle();
Container.Bind<IDisposable>().To<Logger>().AsSingle();
```

Or you can use the following shortcut:

```csharp
Container.BindAllInterfacesAndSelf<Logger>().To<Logger>().AsSingle();
```

This works because when the scene changes or your unity application is closed, the unity event OnDestroy() is called on all MonoBehaviours, including the SceneContext class, which then triggers Dispose() on all objects that are bound to `IDisposable`

Note that this example may or may not be a good idea (for example, the file will be left open if your app crashes), but illustrates the point  :)

## <a id="installers"></a>Installers

Often, there is some collections of related bindings for each sub-system and so it makes sense to group those bindings into a re-usable object.  In Zenject this re-usable object is called an Installer.  You can define a new installer as follows:

```csharp
public class FooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<Foo>().To<Foo>().AsSingle();
    }
}
```

You add bindings by overriding the InstallBindings method, which is called by the `SceneContext` when your scene starts up.  MonoInstaller is a MonoBehaviour so you can add FooInstaller by attaching it to a GameObject.  Since it is a GameObject you can also add public members to it to configure your installer from the Unity inspector.  This allows you to add references within the scene, references to assets, or simply tuning data (see [here](https://github.com/modesttree/Zenject#using-the-unity-inspector-to-configure-settings) for more information on tuning data).

Note that in order for your installer to be triggered it must be attached to the Installers property of the `SceneContext` object.  This is necessary to be able to control the order that installers are called in (which you can do by dragging rows around in the Installers property).  The order should not usually matter (since nothing should be instantiated during the install process) however it can matter in some cases, such as when you configure an Installer from an existing installer (eg: `Container.BindInstance("mysetting").WhenInjectedInto<MyOtherInstaller>()`).

In many cases you want to have your installer derive from MonoInstaller, so that you can have inspector settings.  There is also another base class called simply `Installer` which you can use in cases where you do not need it to be a MonoBehaviour.

You can also call other installers from an existing installer.  For example:

```csharp
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
        Container.Install<BarInstaller>();
    }
}
```

Note that in this case BarInstaller is of type Installer and not MonoInstaller, which is why we can simply call `Container.Install<BarInstaller>`.  By using Installer for BarInstaller instead of MonoInstaller, we don't need an instance of BarInstaller in our scene to use it.  Any calls to Container.Install will immediately create the given installer type and then call InstallBindings on it.  This will repeat for any installers that this installer installs.

One of the main reasons we use installers as opposed to just having all our bindings declared all at once for each scene, is to make them re-usable.  This is not a problem for installers of type `Installer` because you can simply call `Container.Install` as described above for every scene you wish to use it in, but then how would we re-use a MonoInstaller in multiple scenes?

There are two ways to do this.

1. **Prefabs within the scene**.  After attaching your MonoInstaller to a gameobject in your scene, you can then create a prefab out of it.  This is nice because it allows you to share any configuration that you've done in the inspector on the MonoInstaller across scenes (and also have per-scene overrides if you want).  After adding it in your scene you can then drag and drop it on to the SceneContext property in the Unity inspector

2. **Prefabs within Resources folder**.  You can also call `Container.InstallPrefabResource`, which will load a prefab that is assumed to container a MonoInstaller on it from the resources folder.  If you do not supply a resource path, it will be assumed to exist at "Resources/Installers/NameOfMonoInstallerType".  For example:

```csharp
// Note that this is a MonoInstaller and has inspector settings
public class QuxInstaller : MonoInstaller
{
    public string MyConfigurationSetting;

    public override void InstallBindings()
    {
        ...
    }
}

public class FooInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // When this is called, Zenject will look for a prefab at `Resources/Installers/QuxInstaller.prefab` and load that
        Container.InstallPrefabResource<QuxInstaller>();

        // You can also explicitly give a custom resource path
        Container.InstallPrefabResource<QuxInstaller>("Stuff/Qux");
    }
}
```

Using InstallPrefabResource is sometimes a useful alternative to adding installer prefabs to every scene because it allows you to keep the objects in your scenes extremely light.

## <a id="di-guidelines--recommendations"></a>General Guidelines / Recommendations / Gotchas / Tips and Tricks

* **Do not use GameObject.Instantiate if you want your objects to have their dependencies injected**
    * If you want to create a prefab yourself, we recommend creating a <a href="#game-object-factories">MonoBehaviourFactory or a MonoBehaviourFactory</a>.  You can also instantaite a prefab by directly using the DiContainer by calling any of the `InstantiatePrefab` methods.  Using these ways as opposed to GameObject.Instantiate will ensure any fields that are marked with the [Inject] attribute have their dependencies injected, and all [Inject] methods within the prefab are called appropriately.

* **Best practice with DI is to *only* reference the container in the composition root "layer"**
    * Note that factories are part of this layer and the container can be referenced there (which is necessary to create objects at runtime).  For example, see ShipStateFactory in the sample project.  See <a href="#creating-objects-dynamically">here</a> for more details on this.

* **Do not use IInitializable, ITickable and IDisposable for dynamically created objects**
    * Objects that are of type `IInitializable` are only initialized once - at startup.  If you create an object through a factory, and it derives from `IInitializable`, the Initialize() method will not be called.  You should use [Inject] methods in this case.
    * The same applies to `ITickable` and `IDisposable`.  Deriving from these will do nothing unless they are part of the original object graph created at startup.
    * If you have dynamically created objects that have an Update() method, it is usually best to call Update() on those manually, and often there is a higher level manager-like class in which it makes sense to do this from.  If however you prefer to use `ITickable` for dynamically objects you can declare a dependency to TickableManager and add/remove it explicitly as well.
    * Note that if you are using GameObjectContext for your dynamically created object, this suggestion does not apply

* **Using multiple constructors**
    * Zenject does not support injecting into multiple constructors currently.  You can have multiple constructors however you must mark one of them with the [Inject] attribute so Zenject knows which one to use.

* **Using dependencies within Start/Awake methods for dynamically created MonoBehaviours**
    * One issue that often arises when using Zenject is that a game object is instantiated dynamically, and then one of the MonoBehaviours on that game object attempts to use one of its injected field dependencies in its Start() or Awake() methods.  Often in these cases the dependency will still be null, as if it was never injected.  The issue here is that Zenject cannot fill in the dependencies until after the call to GameObject.Instantiate completes, and in most cases GameObject.Instantiate will call the Start() and Awake() methods.  The solution is to use neither Start() or Awake() and instead define a new method and mark it with a [Inject] attribute.  This will guarantee that all dependencies have been resolved before executing the method.

* **Using Zenject outside of Unity**
    * Zenject is primarily designed to work within Unity3D.  However, it can also be used as a general purpose DI framework outside of Unity3D.  In order to do this, you can use the DLL provided in the Releases section of the GitHub page, or use the solution at `AssemblyBuild/Zenject.sln` to build it with the build configurations "Not Unity Debug" and "Not Unity Release".  Note also that if multi-threading support is needed (for eg. if used with ASP.NET MVC) then you will also have to define ZEN_MULTITHREADING

* **Lazily instantiated objects and the object graph**
    * Zenject will only instantiate any objects that are referenced in the object graph that is generated based on the bindings that you've invoked in your installer.  Internally, how it works is that Zenject has one single class that represents the root of the entire object graph (aka IDependencyRoot).  For unity projects this is typically the 'UnityDependencyRoot' class.  This class has a dependency on all `ITickable`, `IInitializable`, and `IDisposable` objects.  This is important to understand because it means that any class that you bind to `ITickable`, `IInitializable`, or `IDisposable` will always be created as part of the initial object graph of your application.  And only otherwise will your class be lazily instantiated when referenced by another class.

* **The order that things occur in is wrong, like injection is occurring too late, or Initialize() event is not called at the right time, etc.**
    * It may be because the 'script execution order' of the Zenject classes 'SceneContext', 'SceneFacade', or 'GlobalFacade' are incorrect.  These classes should always have the earliest or near earliest execution order.  This should already be set by default (since this setting is included in the `cs.meta` files for these classes).  However if you are compiling Zenject yourself or have a unique configuration you may want to make sure, which you can do by going to "Edit -> Project Settings -> Script Execution Order" and confirming that these classes are at the top, before the default time.

Please feel free to submit any other sources of confusion to sfvermeulen@gmail.com and I will add it here.

## <a id="global-bindings"></a>Global Bindings

This all works great for each individual scene, but what if you have dependencies that you wish to persist permanently across all scenes?  In Zenject you can do this by adding installers to a ProjectContext object.

To do this, first you need to create a prefab for the ProjectContext, and then you can add installers to it.  You can do this most easily by selecting the menu item `Edit -> Zenject -> Create Project Context`. You should then see a new asset in the folder `Assets/Resources` called 'ProjectContext'.

If you click on this it will appear nearly identically to the inspector for `SceneContext`.  The easiest way to configure this prefab is to temporarily add it to your scene, add Installers to it, then click "Apply" to save it back to the prefab before deleting it from your scene.  In addition to installers, you can also add your own custom MonoBehaviour classes to the ProjectContext object directly.

Then, when you start any scene that contains a `SceneContext`, your `ProjectContext` object will always be initialized first.  All the installers you add here will be executed and the bindings that you add within them will be available for use in all scenes within your project.

Note also that this only occurs once.  If you load another scene from the first scene, your ProjectContext will not be called again and the bindings that it added previously will persist into the new scene.  You can declare `ITickable` / `IInitializable` / `IDisposable` objects in your global installers in the same way you do for your scene installers with the result being that `IInitializable.Initialize` is called only once across each play session and `IDisposable.Dispose` is only called once the application is fully stopped.

This works because the container defined for each scene is nested inside the global container that your global installers bind into.  For more information on nested containers see <a href="#sub-containers-and-facades">here</a>.

## <a id="update--initialization-order"></a>Update / Initialization Order

In many cases, especially for small projects, the order that classes update or initialize in does not matter.  However, in larger projects update or initialization order can become an issue.  This can especially be an issue in Unity, since it is often difficult to predict in what order the Start(), Awake(), or Update() methods will be called in.  Unfortunately, Unity does not have an easy way to control this (besides in `Edit -> Project Settings -> Script Execution Order`, though that is pretty awkward to use)

In Zenject, by default, ITickables and IInitializables are updated in the order that they are added, however for cases where the update or initialization order does matter, there is a much better way:  By specifying their priorities explicitly in the installer.  For example, in the sample project you can find this code in the scene installer:

```csharp
public class AsteroidsInstaller : MonoInstaller
{
    ...

    // We don't need to include these bindings but often its nice to have
    // control over initialization-order and update-order
    void InitExecutionOrder()
    {
        Container.Install<ExecutionOrderInstaller>(
            new List<Type>()
            {
                // Re-arrange this list to control update order
                // These classes will be initialized and updated in this order and disposed of in reverse order
                typeof(AsteroidManager),
                typeof(GameController),
            });
    }

    ...

    public override void InstallBindings()
    {
        ...
        InitExecutionOrder();
        ...
    }

}
```

This way, you won't hit a wall at the end of the project due to some unforeseen order-dependency.

You can also assign priorities one class at a time using the following helper method:

```csharp
ExecutionOrderInstaller.BindPriority<Foo>(Container, -100);

Container.Bind<IInitializable>().To<Foo>().AsSingle();
Container.Bind<ITickable>().To<Foo>().AsSingle();
Container.Bind<IInitializable>().To<Bar>().AsSingle();
```

You can also define specific priorities for each interface with other helper methods:

```csharp
ExecutionOrderInstaller.BindTickablePriority<Foo>(Container, -100);
ExecutionOrderInstaller.BindInitializablePriority<Foo>(Container, -50);

Container.Bind<IInitializable>().To<Foo>().AsSingle();
Container.Bind<ITickable>().To<Foo>().AsSingle();
Container.Bind<IInitializable>().To<Bar>().AsSingle();
```

Note that you can use these helper method without actually calling `Container.Install<ExecutionOrderInstaller>` as well.

Note that any ITickables, IInitializables, or `IDisposable`'s that are not assigned a priority are automatically given the priority of zero.  This allows you to have classes with explicit priorities executed either before or after the unspecified classes.  For example, the above code would result in 'Foo.Initialize' being called before 'Bar.Initialize'.  If you instead gave it 100 instead of -100, it would be executed afterwards.

Note also that classes are disposed of in the opposite order.  This is similar to how Unity handles explicit script execution order.

## <a id="object-graph-validation"></a>Object Graph Validation

The usual workflow when setting up bindings using a DI framework is something like this:

* Add some number of bindings in code
* Execute your app
* Observe a bunch of DI related exceptions
* Modify your bindings to address problem
* Repeat

This works ok for small projects, but as the complexity of your project grows it is often a tedious process.  The problem gets worse if the startup time of your application is particularly bad, or when the resolve errors only occur from factories at various points at runtime.  What would be great is some tool to analyze your object graph and tell you exactly where all the missing bindings are, without requiring the cost of firing up your whole app.

You can do this in Zenject out-of-the-box by executing the menu item `Edit -> Zenject -> Validate Current Scene` or simply hitting CTRL+SHIFT+V with the scene open that you want to validate.  This will execute all installers for the current scene without actually running your game, with the result being a fully bound container (with null values for all instances).   It will then iterate through the object graphs and verify that all bindings can be found (without actually instantiating any of them).

Note that if you want to use this feature (which I recommend as its very useful) then you should avoid instantiating new objects in your installers and executing other code that has similar side effects.

Also, if you happen to be a fan of automated testing (as I am) then you can include calls to this menu item in unity batch mode as part of your testing suite.  For example:

    "[PATH TO UNITY]" -projectPath "[PATH TO SAMPLE PROJECT]" -executeMethod Zenject.ZenEditorUtil.ValidateAllScenesFromScript -batchmode -nographics

You can also validate specific scenes from the command line by calling ValidateScenesFromScript and also passing a list of scenes:

    "[PATH TO UNITY]" -projectPath "[PATH TO SAMPLE PROJECT]" -executeMethod Zenject.ZenEditorUtil.ValidateScenesFromScript -batchmode -nographics "-CustomArg:scenes=Asteroids,AsteroidsDecoratorExample"

This will both return an error code if validation fails.  If it succeeded, it will return 0 and the log for the unity editor will contain the line "Successfully validated all 2 scenes".

## <a id="using-the-unity-inspector-to-configure-settings"></a>Using the Unity Inspector To Configure Settings

One implication of writing most of your code as normal C# classes instead of MonoBehaviour's is that you lose the ability to configure data on them using the inspector.  You can however still take advantage of this in Zenject by using the following pattern, as seen in the sample project:

```csharp
public class AsteroidsInstaller : MonoInstaller
{
    public Settings SceneSettings;

    public override void InstallBindings()
    {
        ...
        Container.BindInstance(SceneSettings.StateMoving);
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
```

Note that if you follow this method, you will have to make sure to always include the [Serializable] attribute on your settings wrappers, otherwise they won't show up in the Unity inspector.

To see this in action, start the asteroids scene and try adjusting `Ship -> State Moving -> Move Speed` setting and watch live as your ship changes speed.

## <a id="creating-objects-dynamically"></a>Creating Objects Dynamically Using Factories

One of the things that often confuses people new to dependency injection is the question of how to create new objects dynamically, after the app/game has fully started up and all the `IInitializable` objects have had their Initialize() method called.  For example, if you are writing a game in which you are spawning new enemies throughout the game, then you will want to construct a new object for the 'enemy' class, and you will want to ensure that this object gets injected with dependencies just like all the objects that are part of the initial object graph.  How to do this?  The answer: Factories.

Remember that an important part of dependency injection is to reserve use of the container to strictly the "Composition Root Layer".  The container class (DiContainer) is included as a dependency in itself automatically so there is nothing stopping you from ignoring this rule and injecting the container into any classes that you want.  For example, the following code will work:

```csharp
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
```

However, the above code is an example of an anti-pattern.  This will work, and you can use the container to get access to all other classes in your app, however if you do this you will not really be taking advantage of the power of dependency injection.  This is known, by the way, as [Service Locator Pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/).

Of course, the dependency injection way of doing this would be the following:

```csharp
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
```

The only exception to this rule is within factories and installers.  Again, factories and installers make up what we refer to as the "composition root layer".

For example, if you have a class responsible for spawning new enemies, before DI you might do something like this:

```csharp
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
```

This will not work however, since in our case the Enemy class requires a reference to the Player class in its constructor.  We could add a dependency to the Player class to the EnemySpawner class, but then we have the problem described <a href="#theory">above</a>.  The EnemySpawner class doesn't care about filling in the dependencies for the Enemy class.  All the EnemySpawner class cares about is getting a new Enemy instance.

The recommended way to do this in Zenject is the following:

```csharp
public class Player
{
}

public class Enemy
{
    readonly Player _player;

    public Enemy(Player player)
    {
        _player = player;
    }

    public class Factory : Factory<Enemy>
    {
    }
}

public class EnemySpawner : ITickable
{
    readonly Enemy.Factory _enemyFactory;

    public EnemySpawner(Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Tick()
    {
        if (ShouldSpawnNewEnemy())
        {
            var enemy = _enemyFactory.Create();
            // ...
        }
    }
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<EnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.BindFactory<Enemy, Enemy.Factory>();
    }
}
```

By using `Enemy.Factory` above instead of `new Enemy`, all the dependencies for the Enemy class (such as the Player) will be automatically filled in.  We can also add runtime parameters to our factory.  For example, let's say we want to randomize the speed of each Enemy to add some interesting variation to our game.  Our enemy class becomes:

```csharp
public class Enemy
{
    readonly Player _player;
    readonly float _speed;

    public Enemy(float speed, Player player)
    {
        _player = player;
        _speed = speed;
    }

    public class Factory : Factory<float, Enemy>
    {
    }
}

public class EnemySpawner : ITickable
{
    readonly Enemy.Factory _enemyFactory;

    public EnemySpawner(Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Tick()
    {
        if (ShouldSpawnNewEnemy())
        {
            var newSpeed = Random.Range(MIN_ENEMY_SPEED, MAX_ENEMY_SPEED);
            var enemy = _enemyFactory.Create(newSpeed);
            // ...
        }
    }
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<EnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.BindFactory<float, Enemy, Enemy.Factory>();
    }
}
```

The dynamic parameters that are provided to the Enemy constructor are declared by using generic arguments to the Factory<> base class of Enemy.Factory.  This will add a method to Enemy.Factory that takes the parameters with the given types, which is called by EnemySpawner.

There is no requirement that the `Enemy.Factory` class be a nested class within Enemy, however we have found this to be a very useful convention.  `Enemy.Factory` is always intentionally left empty and simply derives from the built-in Zenject `Factory<>` class, which handles the work of using the DiContainer to construct a new instance of Enemy.

The reason that we don't use the `Factory<Enemy>` class directly is because this can become error prone when adding/removing parameters and also can get verbose since the parameter types must be declared whenever you refer to this class.  This is error prone because if for example you add a parameter to enemy and change the factory from `Factory<Enemy>` to `Factory<float, Enemy>`, any references to `Factory<Enemy>` will not be resolved.  And this will not be caught at compile time and instead will only be seen during validation or runtime.  So we recommend always using an empty class that derives from `Factory<>` instead.

Also note that by using the built-in Zenject `Factory<>` class, the Enemy class will be automatically validated as well.  So if the constructor of the Enemy class includes a type that is not bound to the container, this error can be caught before running your app, by simply running validation.  Validation can be especially useful for dynamically created objects, because otherwise you may not catch the error until the factory is invoked at some point during runtime.  See <a href="#object-graph-validation">this section</a> for more details on Validation.

This is all well and good for simple cases like this, but what if the Enemy class derives from MonoBehaviour and is instantiated via a prefab?  Or what if I want to create Enemy using a custom method or a custom factory class?

These cases are handled very similar to how they are handled when not using a factory.  When you bind the factory to the container using `BindFactory`, you have access to the same bind methods that are given by the Bind method and described above.  For example, if we wanted to instantiate the `Enemy` class from a prefab, our code would become:

```csharp
public class Enemy : MonoBehaviour
{
    Player _player;

    // Note that we can't use a constructor anymore since we are a MonoBehaviour now
    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    public class Factory : Factory<float, Enemy>
    {
    }
}

public class TestInstaller : MonoInstaller
{
    public GameObject EnemyPrefab;

    public override void InstallBindings()
    {
        Container.BindAllInterfaces<EnemySpawner>().To<EnemySpawner>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.BindFactory<Enemy, Enemy.Factory>().FromPrefab(EnemyPrefab);
    }
}

```

And similarly for FromInstance, FromMethod, FromSubContainerResolve, or any of the other <a href="#construction-methods">construction methods</a>.

Using FromSubContainerResolve can be particularly useful if your dynamically created object has a lot of its own dependencies.  You can have it behave like a Facade.  See <a href="#sub-containers-and-facades">here</a> for details on nested containers / facades.

## <a id="abstract-factories"></a>Abstract Factories

The above description of factories is great for most cases, however, there are times you do not want to depend directly on a concrete class and instead want your factory to return an interface instead.  This kind of factory is called an Abstract Factory.

Let's create an example scenario, where we have multiple different implementations of a given interface:

```csharp

public interface IPathFindingStrategy
{
    ...
}

public class AStarPathFindingStrategy : IPathFindingStrategy
{
    ...
}

public class RandomPathFindingStrategy : IPathFindingStrategy
{
    ...
}
```

For the sake of this example, let's also assume that we have to create the instance of IPathFindingStrategy at runtime.  Otherwise it would be as simple as executing `Container.Bind<IPathFindingStrategy>().To<TheImplementationWeWant>().AsSingle();` in one of our installers.

This is done in a very similar way that non-Abstract factories work.  One difference is that we can't include the factory as a nested class inside the interface (not allowed in C#) but otherwise it's no different:

```csharp
public class PathFindingStrategyFactory : Factory<IPathFindingStrategy>
{
}

public class GameController : IInitializable
{
    PathFindingStrategyFactory _strategyFactory;
    IPathFindingStrategy _strategy;

    public GameController(PathFindingStrategyFactory strategyFactory)
    {
        _strategyFactory = strategyFactory;
    }

    public void Initialize()
    {
        _strategy = _strategyFactory.Create();
        // ...
    }
}

public class GameInstaller : MonoInstaller
{
    public bool UseAStar;

    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameController>().To<GameController>().AsSingle();

        if (UseAStar)
        {
            Container.BindFactory<IPathFindingStrategy, PathFindingStrategyFactory>().To<AStarPathFindingStrategy>();
        }
        else
        {
            Container.BindFactory<IPathFindingStrategy, PathFindingStrategyFactory>().To<RandomPathFindingStrategy>();
        }
    }
}
```

## <a id="custom-factories"></a>Custom Factories

*Ok, but what if I don't know what type I want to create until after the application has started?  Or what if I have special requirements for constructing instances of the Enemy class that are not covered by any of the construction methods?* 

In these cases you can create a custom factory, and directly call `new Enemy` or directly use the `DiContainer` class to create your object.  For example, continuing the previous factory example, let's say that you wanted to be able to change a runtime value (difficulty) that determines what kinds of enemies get created.

```csharp
public enum Difficulties
{
    Easy,
    Hard,
}

public interface IEnemy
{
}

public class EnemyFactory : Factory<IEnemy>
{
}

public class Demon : IEnemy
{
}

public class Dog : IEnemy
{
}

public class DifficultyManager
{
    public Difficulties Difficulty
    {
        get;
        set;
    }
}

public class CustomEnemyFactory : IFactory<IEnemy>
{
    DiContainer _container;
    DifficultyManager _difficultyManager;

    public CustomEnemyFactory(DiContainer container, DifficultyManager difficultyManager)
    {
        _container = container;
        _difficultyManager = difficultyManager;
    }

    public IEnemy Create()
    {
        if (_difficultyManager.Difficulty == Difficulties.Hard)
        {
            return _container.Instantiate<Demon>();
        }

        return _container.Instantiate<Dog>();
    }
}

public class GameController : IInitializable
{
    readonly EnemyFactory _enemyFactory;

    public GameController(EnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Initialize()
    {
        var enemy = _enemyFactory.Create();
        // ...
    }
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameController>().To<GameController>().AsSingle();
        Container.Bind<DifficultyManager>().AsSingle();
        Container.BindFactory<IEnemy, EnemyFactory>().FromFactory<CustomEnemyFactory>();
    }
}
```

You could also directly call `new Dog()` and `new Demon()` here instead of using the DiContainer (though in that case `Dog` and `Demon` would not have their members injected).

One important issue to be aware of with using custom factories, is that the dynamically created classes will not be validated properly.  So in this example, if the `Demon` or `Dog` classes have a constructor parameter that is not bound to the DiContainer, that will not become obvious until runtime.  Whereas, if using a normal factory, that would be caught during validation.

If you want to properly validate your custom factories, you can do that by just making a small modification to it:

```csharp
public class CustomEnemyFactory : IFactory<IEnemy>, IValidatable
{
    DiContainer _container;
    DifficultyManager _difficultyManager;

    public CustomEnemyFactory(DiContainer container, DifficultyManager difficultyManager)
    {
        _container = container;
        _difficultyManager = difficultyManager;
    }

    public IEnemy Create()
    {
        if (_difficultyManager.Difficulty == Difficulties.Hard)
        {
            return _container.Instantiate<Demon>();
        }

        return _container.Instantiate<Dog>();
    }

    public void Validate()
    {
        _container.Instantiate<Dog>();
        _container.Instantiate<Demon>();
    }
}
```

This is done by implementing the interface `IValidatable` and then adding a `Validate()` method.  Then, to manually validate objects, you simply instantiate them.  Note that this will not actually instantiate these objects (these calls actually return null here).  The point is to do a "dry run" without actually instantiating anything, to prove out the full object graph.  For more details on validation see <a href="#object-graph-validation">here</a>.

## <a id="injecting-data-across-scenes"></a>Injecting data across scenes

In some cases it's useful to pass arguments from one scene to another.  The way Unity allows us to do this by default is fairly awkward.  Your options are to create a persistent GameObject and call DontDestroyOnLoad() to keep it alive when changing scenes, or use global static classes to temporarily store the data.

Let's pretend you want to specify a 'level' string to the next scene.  You have the following class that requires the input:

```csharp
public class LevelHandler : IInitializable
{
    readonly string _startLevel;

    public LevelHandler(
        [InjectOptional]
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
```

You can load the scene containing `LevelHandler` and specify a particular level by using the following syntax:

```csharp
ZenUtil.LoadScene("NameOfSceneToLoad", (container) => 
    {
        container.Bind<string>().ToInstance("custom_level").WhenInjectedInto<LevelHandler>();
    });
```

The bindings that we add here inside the lambda will be added to the container as if they were inside an installer in the new scene.

Note that you can still run the scene directly, in which case it will default to using "default_level".  This is possible because we are using the `InjectOptional` flag.

An alternative and arguably cleaner way to do this would be to customize the installer itself rather than the `LevelHandler` class.  In this case we can write our `LevelHandler` class like this (without the `[InjectOptional]` flag).

```csharp
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
```

Then, in the installer for our scene we can include the following:

```csharp
public class GameInstaller : Installer
{
    [InjectOptional]
    public string LevelName = "default_level";

    ...

    public override void InstallBindings()
    {
        ...
        Container.Bind<string>().ToInstance(LevelName).WhenInjectedInto<LevelHandler>();
        ...
    }
}
```

Then, instead of injecting directly into the `LevelHandler` we can inject into the installer instead.

```csharp
ZenUtil.LoadScene("NameOfSceneToLoad", (container) =>
    {
        container.Bind<string>().ToInstance("custom_level").WhenInjectedInto<GameInstaller>();
    });
```

Some people have also found it useful to separate out content into different scenes and then load each scene additively using the Unity method `Application.LoadLevelAdditive`.  In some cases it's useful to have the dependencies in the new scene resolved using the container of the original scene.  To achieve this, you can call `ZenUtil.LoadSceneAdditiveWithContainer` and pass in your scene's container.  Note however that it is assumed in this case that the new scene does not have its own container + Context.

## <a id="scenes-decorator"></a>Scene Decorators

Scene Decorators can be thought of a more advanced way of the process described <a href="#injecting-data-across-scenes">above</a>.  That is, they can be used to add behaviour to another scene without actually changing the installers in that scene.

Usually, when you want to customize different behaviour for a given scene depending on some conditions, you would use flags on MonoInstallers, which would then add different bindings depending on the given values.  However the scene decorator approach can be cleaner sometimes because it doesn't involve changing the main scene.

For example, let's say we want to add some special keyboard shortcuts to our main production scene for testing purposes.  In order to do this using decorators, you can do the following:

* Create a new scene
* Right Click inside the Hierarchy tab and select `Zenject -> Decorator Context`
* Type in the scene you want to 'decorate' in the 'Scene Name' field of SceneDecoratorContext.  Note that as a shortcut, you can click the Open button next to this name to jump to the decorated scene.
* Create a new C# script with the following contents, then add this MonoBehaviour to your scene as a gameObject, then drag it to the `PreInstallers` property of `SceneDecoratorContext`

```csharp
public class ExampleDecoratorInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ITickable>().To<TestHotKeysAdder>().AsSingle();
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
```

If you run your scene it should now behave exactly like the scene you entered in 'Scene Name' except with the added functionality in your decorator installer.

NOTE: If the scene fails to load, it might be because the scene that you're decoratoring has not been added to the list of levels in build settings.

Normally it is not that important whether you put your installers in `PreInstallers` or `PostInstallers`.  The one case where this matters is if you are configuring bindings for an installer within the new scene.  In this case you will want to make sure to put your installers in `PreInstallers`.

For a full example see the asteroids project that comes with Zenject (open 'AsteroidsDecoratorExample' scene).  NOTE:  If installing from asset store version, you need to add the 'Asteroids' scene to your build settings so that the scene decorator can find it.

Note also that Zenject validate (using CTRL+SHIFT+V or the menu item via Edit->Zenject->Validate Current Scene) also works with decorator scenes.

Note also that you can add a decorator scene for another decorator scene, and this should work fine.

## <a id="commands-and-signals"></a>Commands And Signals

Zenject also includes an optional extension that allows you to define "Commands" and "Signals".

A signal can be thought of as a single event, that when triggered will notify a number of listeners.  A command is an object with a single method named `Execute`, that will forward the request to a specific handler.

The advantage of using Signals and Commands is that the result will often be more loosely coupled code.  Given two classes A and B that need to communicate, your options are usually:

1. Directly call a method on B from A.  In this case, A is strongly coupled with B.
2. Inverse the dependency by having B observe an event on A.  In this case, B is strongly coupled with A.

Both cases result in the classes being coupled in some way.  Now if instead you create a command object, which is called by A and which invokes a method on B, then the result is less coupling.  Granted, A is still coupled to the command class, but in some cases that is better than being directly coupled to B.  Using signals works similarly, in that you can remove the coupling by having A trigger a signal, which is observed by B.

Signals are defined like this:

```csharp
public class GameLoadedSignal : Signal
{
    public class Trigger : TriggerBase { }
}

public class GameLoadedSignalWithParameter : Signal<string>
{
    public class Trigger : TriggerBase { }
}
```

The trigger class is used to invoke the signal event.  We make the trigger a separate class so that we can control which classes can trigger the signal and which classes can listen on the signal separately.

Note that the Signal base class is defined within the Zenject.Commands namespace.

Signals are declared in an installer like this:

```csharp

public override void InstallBindings()
{
    ...
    Container.BindSignal<GameLoadedSignal>();
    ...
    Container.BindSignal<GameLoadedSignalWithParameter>().WhenInjectedInto<Foo>();
}

```

These statements will do the following:
* Bind the class `GameLoadedSignal` using `AsSingle` without a condition.  This means that any class can declare `GameLoadedSignal` as a dependency.
* Bind the class `GameLoadedSignalWithParameter` using `AsSingle` as well, except it will limit its usage strictly to class `Foo`.

Once you have added the signal to your container by binding it within an installer, you can use it like this:

```csharp

public class Foo : IInitializable, IDisposable
{
    readonly GameLoadedSignal _signal;

    public Foo(GameLoadedSignal signal)
    {
        _signal = signal;
    }

    public void Initialize()
    {
        _signal.Event += OnGameLoaded;
    }

    public void Dispose()
    {
        _signal.Event -= OnGameLoaded;
    }

    void OnGameLoaded()
    {
        ...
    }
}
```

Here we use the convention of prefixing event handlers with On, but of course you don't have to follow this convention.

After binding the signal, you will almost always want to also bind a trigger, so that you can actually invoke the signal.  Signals and Triggers are bound as separate statements so that you can optionally add conditional binding on both the trigger and the signal separately.

Triggers are declared in an installer like this:

```csharp

public override void InstallBindings()
{
    ...
    Container.BindTrigger<GameLoadedSignal.Trigger>();
    ...
    Container.BindTrigger<GameLoadedSignalWithParameter.Trigger>().WhenInjectedInto<Foo>();
}

```

Once you have added the trigger to your container by binding it within an installer, you can use it like this:

```csharp
public class Foo
{
    readonly GameLoadedSignal.Trigger _trigger;

    public Foo(GameLoadedSignal.Trigger trigger)
    {
        _trigger = trigger;
    }

    public void DoSomething()
    {
        _trigger.Fire();
    }
}
```

Commands are defined like this

```csharp
public class ResetSceneCommand : Command { }

public class ResetSceneCommandWithParameter : Command<string> { }
```

Note again that the Command base class is defined within the Zenject.Commands namespace here.

Unlike with signals, there are several different ways of declaring a command in an installer.  Perhaps the simplest way would be the following:

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().To<ResetSceneHandler>().AsSingle();
    ...
    Container.BindCommand<ResetSceneCommandWithParameter, string>().To<ResetSceneHandler>().AsSingle();
    ...
}

public class ResetSceneHandler : ICommandHandler
{
    public void Execute()
    {
        ... [reset scene] ...
    }
}
```

This bind statement will result in an object of type `ResetSceneCommand` being added to the container.  Any time a class calls Execute on `ResetSceneCommand`, it will trigger the Execute method on the `ResetSceneHandler` class as well.  For example:

```csharp
public class Foo : ITickable
{
    readonly ResetSceneCommand _command;

    public Foo(ResetSceneCommand command)
    {
        _command = command;
    }

    public void Tick()
    {
        ...
        _command.Execute();
        ...
    }
}
```

We might also want to restrict usage of our command to the Foo class only, which we could do with the following

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().To<ResetSceneHandler>().AsSingle().WhenInjectedInto<Foo>();
    ...
}
```

Note that in this case we are using `AsSingle` - this means that the same instance of `ResetSceneHandler` will be used every time the command is executed.  Alternatively, you could declare it using `ToTransient<>` which would instantiate a new instance of `ResetSceneHandler` every time Execute() is called.  For example:

```csharp
Container.BindCommand<ResetSceneCommand>().ToTransient<ResetSceneHandler>();
```

This might be useful if the `ResetSceneCommand` class involves some long-running operations that require unique sets of member variables/dependencies.

You can also bind commands directly to methods instead of classes by doing the following:

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().ToSingle<MyOtherHandler>(x => x.ResetScene);
    ...
}

public class ResetSceneHandler
{
    public void ResetScene()
    {
        ... [reset scene] ...
    }
}
```

This approach does not require that you derive from `ICommandHandler` at all.  There is also a `ToTransient` version of this which works similarly (instantiates a new instance of MyOtherHandler).

```csharp
Container.BindCommand<ResetSceneCommand>().ToTransient<MyOtherHandler>(x => x.ResetScene);
```

## <a id="scene-bindings"></a>Scene Bindings

In many cases, you have a number of MonoBehaviour's that have been added to the scene within the Unity editor (ie. at editor time not runtime) and you want to also have these MonoBehaviour's added to the Zenject Container so that they can be injected into other classes.

The usual way this is done is to add public references to these objects within your installer like this:

    public class Foo : MonoBehaviour
    {
    }

    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        Foo _foo;

        public override void InstallBindings()
        {
            Container.BindInstance(_foo);
            Container.Bind<IInitializable>().To<GameRunner>().AsSingle();
        }
    }

    public class GameRunner : IInitializable
    {
        readonly Foo _foo;

        public GameRunner(Foo foo)
        {
            _foo = foo;
        }

        public void Initialize()
        {
            ...
        }
    }

(Note that you could also just make `Foo` public here - my personal convention is just to always use `SerializeField` instead to avoid breaking encapsulation)

This works fine however in some cases this can get cumbersome.  For example, if you want to allow an artist to add any number of `Enemy` objects to the scene, and you also want all those `Enemy` objects added to the Zenject Container.  In this case, you would have to manually drag each one to the inspector of one of your installers.  This is very error prone since its easy to forget one, or to delete the `Enemy` game object but forget to delete the null reference in the inspector for your installer, etc.

So another way to do this is to use the `ZenjectBinding` component.  You can do this by adding a `ZenjectBinding` MonoBehaviour to the same game object that you want to be automatically added to the Zenject container.

For example, if I have a MonoBehaviour of type `Foo` in my scene, I can just add `ZenjectBinding` alongside it, and then drag the Foo component into the Component property of the ZenjectBinding component.

<img src="UnityProject/Assets/Zenject/Documentation/ReadMe_files/AutoBind1.png?raw=true" alt="ZenjectBinding"/>

Then our installer becomes:

    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInitializable>().To<GameRunner>().AsSingle();
        }
    }

When using `ZenjectBinding` this way, by default it will bind `Foo` using the `Self` method, so it is equivalent to the first example where we did this:

    Container.Bind<Foo>().ToInstance(_foo);

Which is also the same as this:

    Container.BindInstance(_foo);

So if we duplicate this game object to have multiple game objects with `Foo` on them (and its `ZenjectBinding`), they will all be bound to the Container this way.  So after doing this, we would have to change `GameRunner` above to take a `List<Foo>` otherwise we would get Zenject exceptions.

Also note that the `ZenjectBinding` component contains a `Bind Type` property in its inspector.  By default this simply binds the instance as shown above but it can also be set to the following:

1 - `AllInterfaces`

This bind type is equivalent to the following:

    Container.BindAllInterfaces(_foo.GetType()).ToInstance(_foo);

Note however, in this case, that `GameRunner` must ask for type `IFoo` in its constructor.  If we left `GameRunner` asking for type `Foo` then Zenject would throw exceptions, since the `BindAllInterfaces` method only binds the interfaces, not the concrete type.  If you want the concrete type as well then you can use:

2 - `AllInterfacesAndSelf`

This bind type is equivalent to the following:

    Container.BindAllInterfacesAndSelf(_foo.GetType()).ToInstance(_foo);

This is the same as AllInterfaces except we can directly access Foo using type Foo instead of needing an interface.

The final property you will notice on the ZenjectBinding component is the "Container Type".

In most cases you can leave this as its default "Local".  However, if you are using GameObjectContext in places, then the other value might be useful here.

"Container Type" will determine what container the component gets added to.  If set to 'Scene', it will be as if the given component was bound inside an installer on the SceneContext.  If set to 'Local', it will be as if it is bound inside an installer on whatever Context it is in.  In most cases that will be the SceneContext, but if it's inside a GameObjectContext it will be bound into that instead.  Typically you would only need to use the 'Scene' value when you want to bind something to the SceneContext that is inside a GameObjectContext (eg. typically this would be the MonoFacade derived class)

## <a id="sub-containers-and-facades"></a>Sub-Containers And Facades

In some cases it can be very useful to use multiple containers in the same application.  For example, if you are creating a word processor it may be useful to have a sub-container for each tab that represents a separate document.  This way, you could bind a bunch of classes `AsSingle()` within the sub-container and they could all easily reference each other as if they were all singletons.  Then you could instantiate multiple sub-containers to be used for each document, with each sub-container having unique instances of all the classes that handle each specific document.

Another example might be if you are designing an open-world space ship game, you might want each space ship to have it's own container that contains all the class instances responsible for running that specific spaceship.

This is actually how global bindings work.  There is one global container for the entire application, and when a unity scene starts up, it creates a new sub-container "underneath" the global container.  All the bindings that you add in your scene MonoInstaller are bound to your sub-container.  This allows the dependencies in your scene to automatically get injected with global bindings, because sub-containers automatically inherit all the bindings in its parent (and grandparent, etc.).

A common design pattern that we like to use in relation to sub-containers is the <a href="https://en.wikipedia.org/wiki/Facade_pattern">Facade pattern</a>.  This pattern is used to abstract away a related group of dependencies so that it can be used at a higher level when used by other modules in the code base.  This is relevant here because often when you are defining sub-containers in your application it is very useful to also define a Facade class that is used to interact with this sub-container as a whole.  So, to apply it to the spaceship example above, you might have a SpaceshipFacade class that represents very high-level operations on a spaceship such as "Start Engine", "Take Damage", "Fly to destination", etc.  And then internally, the SpaceshipFacade class can delegate the specific handling of all the parts of these requests to the relevant single-responsibility dependencies that exist within the sub-container.

Let's do some examples in the following sections.

## <a id="hello-world-for-facades"></a>Hello World Example For Sub-Containers/Facade

```csharp
public class Greeter
{
    readonly string _message;

    public Greeter(string message)
    {
        _message = message;
    }

    public void DisplayGreeting()
    {
        Debug.Log(_message);
    }
}

public class GameController : IInitializable
{
    readonly Greeter _greeter;

    public GameController(Greeter greeter)
    {
        _greeter = greeter;
    }

    public void Initialize()
    {
        _greeter.DisplayGreeting();
    }
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameController>().To<GameController>().AsSingle();
        Container.Bind<Greeter>().FromSubContainerResolve().ByMethod(InstallGreeter);
    }

    void InstallGreeter(DiContainer subContainer)
    {
        subContainer.Bind<Greeter>().AsSingle();
        subContainer.BindInstance("Hello world!");
    }
}
```

The important thing to understand here is that any bindings that we add inside the `InstallGreeter` method will only be visible to objects within this sub-container.  The only exception is the Facade class (in this case, Greeter) since it is bound to the parent container using the FromSubContainerResolve binding.  In other words, in this example, the string "Hello World" is only visible by the Greeter class.

Note that you should always add a bind statement for whatever class is given to FromSubContainerResolve, otherwise this will fail.

Note also that it is often better to use `ByInstaller` instead of `ByMethod`.  This is because when you use `ByMethod` it is easy to accidentally reference the Container instead of the subContainer.  Also, by using `ByInstaller` you can pass arguments into the Installer itself.

## <a id="using-tickables-within-sub-containers"></a>Using IInitializable / ITickable / IDisposable within Sub-Containers

One issue with the Hello World example above is that if I wanted to add some ITickable's or IInitializable's or IDisposable's to my sub-container it would not work.  For example, I might try doing this:

```csharp
public class GoodbyeHandler : IDisposable
{
    public void Dispose()
    {
        Log.Trace("Goodbye World!");
    }
}

public class HelloHandler : IInitializable
{
    public void Initialize()
    {
        Log.Trace("Hello world!");
    }
}

public class Greeter
{
    public Greeter()
    {
        Debug.Log("Created Greeter!");
    }
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Greeter>().FromSubContainerResolve().ByMethod(InstallGreeter).NonLazy();
    }

    void InstallGreeter(DiContainer subContainer)
    {
        subContainer.Bind<Greeter>().AsSingle();

        subContainer.BindAllInterfaces<GoodbyeHandler>().To<GoodbyeHandler>().AsSingle();
        subContainer.BindAllInterfaces<HelloHandler>().To<HelloHandler>().AsSingle();
    }
}
```

However, while we will find that our `Greeter` class is created (due to the fact we're using `NonLazy`) and the text "Created Greeter!" is printed to the console, the Hello and Goodbye messages are not.  To get this working we need to change it to the following:

```csharp
public class GoodbyeHandler : IDisposable
{
    public void Dispose()
    {
        Debug.Log("Goodbye World!");
    }
}

public class HelloHandler : IInitializable
{
    public void Initialize()
    {
        Debug.Log("Hello world!");
    }
}

public class Greeter : Kernel
{
}

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfacesAndSelf<Greeter>().To<Greeter>().FromSubContainerResolve().ByMethod(InstallGreeter).NonLazy();
    }

    void InstallGreeter(DiContainer subContainer)
    {
        subContainer.Bind<Greeter>().AsSingle();

        subContainer.BindAllInterfaces<GoodbyeHandler>().To<GoodbyeHandler>().AsSingle();
        subContainer.BindAllInterfaces<HelloHandler>().To<HelloHandler>().AsSingle();
    }
}
```

Now if we run it, we should see the Hello message, then if we stop playing we should see the Goodbye message.

The reason this works is because we are now binding IInitializable, IDisposable, and ITickable on the root container to the Greeter class given by `Container.BindAllInterfacesAndSelf<Greeter>().To<Greeter>()`.  Greeter now inherits from Kernel, which inherits from all these interfaces and also handles forwarding these calls to the IInitializable's / ITickable's / IDisposable's in our sub container.

## <a id="using-game-object-contexts"></a>Creating Sub-Containers on GameObject's by using Game Object Context

Another issue with the <a href="#hello-world-for-facades">sub-container hello world example</a> above is that it does not work very well for MonoBehaviour classes.  There is nothing preventing us from adding MonoBehaviour bindings such as FromPrefab, FromGameObject, etc. to our sub-container, however these will cause these new game objects to be added to the root of the scene heirarchy, so we'll have to manually track the lifetime of these objects ourselves by calling GameObject.Destroy on them when the Facade is destroyed.  Also, there is no way to have GameObject's that exist in our scene at the start but also exist within our sub-container.  These problems can be solved by using Game Object Context.

For this example, let's try to actually implement something similar to the open world space ship game described in <a href="#sub-containers-and-facades">the sub-container introduction</a>:

* Create a new scene
* Add the following files to your project:

```csharp
using Zenject;
using UnityEngine;

public class Ship : MonoBehaviour
{
    ShipHealthHandler _healthHandler;

    [Inject]
    public void Construct(ShipHealthHandler healthHandler)
    {
        _healthHandler = healthHandler;
    }

    public void TakeDamage(float damage)
    {
        _healthHandler.TakeDamage(damage);
    }
}
```

```csharp
using UnityEngine;
using Zenject;

public class GameRunner : ITickable
{
    readonly Ship _ship;

    public GameRunner(Ship ship)
    {
        _ship = ship;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _ship.TakeDamage(10);
        }
    }
}
```

```csharp
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameRunner>().To<GameRunner>().AsSingle();
    }
}
```

```csharp
using Zenject;
using UnityEngine;

public class ShipHealthHandler : MonoBehaviour
{
    float _health = 100;

    public void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 100), "Health: " + _health);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
    }
}
```

```csharp
using UnityEngine;
using System.Collections;

public class ShipInputHandler : MonoBehaviour
{
    [SerializeField]
    float _speed = 2;

    public void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += Vector3.forward * _speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position -= Vector3.forward * _speed * Time.deltaTime;
        }
    }
}
```

* Right Click inside the Hierarchy tab and select `Zenject -> Scene Context`
* Drag the GameInstaller class to the SceneContext game object
* Add a new row to the Installers property of the SceneContext
* Drag the GameInstaller component to the new row under Installers
* Right Click again inside the Hierarchy tab and select `Zenject -> Game Object Context`
* Rename the new game object GameObjectContext to Ship
* Drag the Ship MonoBehaviour to the Ship GameObject in our Scene. The Ship class will be used as a <a href="https://en.wikipedia.org/wiki/Facade_pattern">Facade</a> for our ship and will be used by other systems to interact with the ship at a high level
* Also add the `ShipInputHandler` component to the Ship game object
* Right click on the Ship GameObject and select 3D Object -> Cube.  This will serve as the placeholder model for our ship.
* Add new game object under ship called HealthHandler, and add the `ShipHealthHandler` component to it
* Your scene should look like this:

<img src="UnityProject/Assets/Zenject/Documentation/ReadMe_files/ShipFacadeExample1.png?raw=true" alt="Ship Facade Example"/>

* The idea here is that everything at or underneath the Ship game object should be considered inside it's own sub-container.  When we're done, we should be able to add multiple ships to our scene, each with their own components ShipHealthHandler, ShipInputHandler, etc. that can treat each other as singletons.
* Try to validate your scene by pressing CTRL+SHIFT+V.  You should get an error that looks like this: `Unable to resolve type 'ShipHealthHandler' while building object with type 'Ship'.`
* This is because the ShipHealthHandler component has not been added to our sub-container.  To address this:
    * Click on the HealthHandler game object and then click Add Component and type Zenject Binding (if you don't know what that is read <a href="#scene-bindings">this</a>)
    * Drag the Ship Health Handler Component to the Components field of Zenject Binding
* Validate again by pressing CTRL+SHIFT+V.  You should now get this error instead: `Unable to resolve type 'Ship' while building object with type 'GameRunner'.` 
* Our Ship component also needs to be added to the container.  To address this, once again:
    * Click on the Ship game object and then click Add Component and type Zenject Binding
    * Drag the Ship Component to the Components field of Zenject Binding
* If we attempt to validate again you should notice the same error occurs.  This is because by default, ZenjectBinding only adds its components to the nearest container - in this case Ship.  This is not what we want though.  We want Ship to be added to the scene container because we want to use it as the Facade for our sub-container.  To address this we can explicitly tell ZenjectBinding which context to apply the binding to by dragging the SceneContext game object and dropping it on to the Context property of the Zenject binding
* Validation should now pass succesfully.
* If you run the scene now, you should see a health display in the middle of the screen, and you should be able to press Space bar to apply damage, and the up/down arrows to move the ship

Also note that we can add installers to our ship sub-container in the same way that we add installers to our Scene Context - just by dropping them into the Installers property of GameObjectContext.  In this example we used MonoBehaviour's for everything but we can also add however many plain C# classes we want here and have those classes available everywhere in the sub-container just like we do here for MonoBehaviour's by using ZenjectBinding.

## <a id="dynamic-game-object-contexts"></a>Creating Game Object Context's Dynamically

Continuing with the ship example <a href="#using-game-object-contexts">above</a>, let's pretend that we now want to create ships dynamically, after the game has started.

* First, create a prefab for the entire `Ship` GameObject that we created above and then delete it from the Scene.
* Then just change 

```csharp
public class GameRunner : ITickable
{
    readonly Ship.Factory _shipFactory;

    Vector3 lastShipPosition;

    public GameRunner(Ship.Factory shipFactory)
    {
        _shipFactory = shipFactory;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ship = _shipFactory.Create();
            ship.transform.position = lastShipPosition;

            lastShipPosition += Vector3.forward * 2;
        }
    }
}
```

```csharp
public class GameInstaller : MonoInstaller
{
    [SerializeField]
    GameObject ShipPrefab;

    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameRunner>().To<GameRunner>().AsSingle();

        Container.BindFactory<Ship, Ship.Factory>().FromSubContainerResolve().ByPrefab(ShipPrefab);
    }
}
```

* After doing this, make sure to drag and drop the newly created Ship prefab into the ShipPrefab property of GameInstaller in the inspector
* Now if we run our scene, we can hit Space to add multiple Ship's to our scene.

## <a id="dynamic-game-object-contexts"></a>Creating Game Object Context's Dynamically With Parameters

Let's make this even more interesting by passing a parameter into our ship facade.  Let's make the speed of the ship configurable from within the GameController class.

* Change our classes to the following:

```csharp
public class GameRunner : ITickable
{
    readonly Ship.Factory _shipFactory;

    Vector3 lastShipPosition;

    public GameRunner(Ship.Factory shipFactory)
    {
        _shipFactory = shipFactory;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ship = _shipFactory.Create(Random.RandomRange(2, 20));
            ship.transform.position = lastShipPosition;

            lastShipPosition += Vector3.forward * 2;
        }
    }
}
```

```csharp
public class GameInstaller : MonoInstaller
{
    [SerializeField]
    GameObject ShipPrefab;

    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameRunner>().To<GameRunner>().AsSingle();

        Container.BindFactory<float, Ship, Ship.Factory>().FromSubContainerResolve().ByPrefab<ShipInstaller>(ShipPrefab);
    }
}
```

```csharp
using Zenject;
using UnityEngine;

public class Ship : MonoBehaviour
{
    ShipHealthHandler _healthHandler;

    [Inject]
    public void Construct(ShipHealthHandler healthHandler)
    {
        _healthHandler = healthHandler;
    }

    public void TakeDamage(float damage)
    {
        _healthHandler.TakeDamage(damage);
    }

    public class Factory : Factory<float, Ship>
    {
    }
}
```

```csharp
using UnityEngine;
using System.Collections;
using Zenject;

public class ShipInputHandler : MonoBehaviour
{
    [Inject]
    float _speed;

    public void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += Vector3.forward * _speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position -= Vector3.forward * _speed * Time.deltaTime;
        }
    }
}
```

Also, add this new file:

```csharp
using System;
using Zenject;

public class ShipInstaller : MonoInstaller
{
    [Inject]
    float _speed;

    public override void InstallBindings()
    {
        Container.BindInstance(_speed).WhenInjectedInto<ShipInputHandler>();
    }
}
```

After that compiles, add ShipInstaller to the Ship prefab and also drag it to the Installers field of the GameObjectContext.

Note the changes that we made here:
- ShipInputHandler now has the speed injected instead of using Unity's SerializeField.
- The nested Factory class in Ship has a float parameter added to it
- In GameInstaller, the binding for the factory is different
- In GameRunner, we now need to pass a float parameter to the factory's create method

One important difference with creating a Sub-Container using a factory, is that the parameters you supply to the factory are not necessarily forwarded to the facade class.  In this example, the parameter is a float value for speed, which we want to forward to the ShipInputHandler class instead.  That is why these parameters are always forwarded to an installer for the sub-container, so that you can decide for yourself at install time what to do with the parameter.  Another reason for this is that in some cases the parameter might be used to choose different bindings.

You might also want to be able to drop Ship prefabs into the scene at design time.  This way, you can have some Ships that start in the scene, but you can also create them dynamically.  One way to do that would be to change ShipInstaller to the following:

```csharp
using System;
using Zenject;
using UnityEngine;

public class ShipInstaller : MonoInstaller
{
    [SerializeField]
    [InjectOptional]
    float _speed;

    public override void InstallBindings()
    {
        Container.BindInstance(_speed).WhenInjectedInto<ShipInputHandler>();
    }
}
```

This way, you can drop the Ship prefab into the scene and control the speed in the inspector, but you can also create them dynamically and pass the speed into the factory as a parameter.

For a more real-world example see the "Sample2" demo project which makes heavy use of Game Object Contexts.

## <a id="zenject-order-of-operations"></a>Zenject Order Of Operations

Warning: This section gets fairly in depth so if you're not interested in peering under the hood of Zenject, you might want to skip it!

A Zenject driven application is executed by the following steps:

1. Unity Awake() phase begins
1. SceneContext.Awake() method is called.  *NOTE:* This should always be the first thing executed in your scene.  By default it should work this way out of the box, because the executionOrder property should be set to -9999 for the SceneContext class.  You can verify that this is the case by selecting `Edit -> Project Settings -> Script Execution Order` and making sure that SceneContext is at the top (which should then be followed by GlobalFacade and SceneFacade as well)
1. If this is the first scene to be loaded during this play session, SceneContext will create the ProjectContext prefab.  If ProjectContext has already been created by a previous scene, we skip to step 10 to directly initialize the SceneContext
1. ProjectContext creates a new DiContainer object to be used to contain all instances meant to persist across scenes
1. ProjectContext iterates through all the Installers that have been added to its prefab via the Unity Inspector, and updates them to point to the new DiContainer.  It then calls InstallBindings() on each installer.
1. Each Installer registers different sets of dependencies directly on to the given DiContainer by calling one of the Bind<> methods.  Note that the order that this binding occurs should not generally matter, because nothing should be instantiated using the DiContainer until all the installers are fully completed.  Each installer can also call other installers by executing Container.Install<> (Note that you can pass arguments to this method as well)
1. The ProjectContext then injects all MonoBehaviours that have been added to its prefab with their dependencies. All [Inject] methods within the ProjectContext prefab are called at this time as well.  Note that since MonoBehaviours are instantiated by Unity we cannot use constructor injection and therefore [Inject] injection, field injection or property injection must be used instead.
1. After filling in the dependencies for each game object on its prefab, the ProjectContext then constructs an instance of GlobalFacade.  This class represents the root of the object graph for the ProjectContext.  The GlobalFacade class has dependencies on TickableManager, InitializableManager, and DisposableManager classes.  Therefore Zenject constructs instances of those before creating GlobalFacade.  Those manager classes contain dependencies for all objects bound to the `ITickable`, `IInitializable`, and `IDisposable` interfaces.  So once again Zenject resolves all instances bound to any of these interfaces before constructing the manager classes.  This is important to know because this is why when you bind something to `ITickable`/`IInitializable`/`IDisposable`, it is always created at startup.
1. If any required dependencies cannot be resolved, a ZenjectResolveException is thrown
1. Steps 4-9 are repeated except this time with the SceneContext.  All objects in the scene (except those objects that are parented to the ProjectContext) are injected using the bindings that have been installed in the SceneContext's installers.  Also note that because the DiContainer used by SceneContext is a sub-container of the DiContainer for the ProjectContext, objects in the scene will also be injected using bindings declared in a global installer as well. And similar to the process for ProjectContext, a SceneFacade object is created which represents the root of the object graph for all dependencies in the scene.
1. Once again, if any required dependencies in the scene cannot be resolved, a ZenjectResolveException is thrown
1. All other MonoBehaviour's in the scene have their Awake() method called
1. Unity Start() phase begins
1. GlobalFacade.Start() method is called.  This will trigger the Initialize() method on all `IInitializable` objects in the order specified in the ProjectContext installers.
1. SceneFacade.Start() method is called.  This will trigger the Initialize() method on all `IInitializable` objects in the order specified in the SceneContext installers.
1. All other MonoBehaviour's in your scene has their Start() method called
1. Unity Update() phase begins
1. GlobalFacade.Update() is called, which results in Tick() being called for all `ITickable` objects (in the order specified in the ProjectContext installers)
1. SceneFacade.Update() is called, which results in Tick() being called for all `ITickable` objects (in the order specified in the SceneContext installers)
1. All other MonoBehaviour's in your scene has their Update() method called
1. Steps 18 - 19 are repeated for LateUpdate
1. At the same time, Steps 18 - 19 are repeated for FixedUpdate according to the physics timestep
1. App is exited
1. Dispose() is called on all objects mapped to `IDisposable` within the SceneContext installers (see <a href="#implementing-idisposable">here</a> for details)
1. Dispose() is called on all objects mapped to `IDisposable` within the ProjectContext installers (see <a href="#implementing-idisposable">here</a> for details)

## <a id="auto-mocking-using-moq"></a>Auto-Mocking using Moq

One of the really cool features of DI is the fact that it makes testing code much, much easier.  This is because you can easily substitute one dependency for another by using a different Composition Root.  For example, if you only want to test a particular class (let's call it Foo) and don't care about testing its dependencies, you might write 'mocks' for them so that you can isolate Foo specifically.

```csharp
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
```

In this example, we have a class Foo that interacts with a web server to retrieve content.  This would normally be very difficult to test for the following reasons:

* You would have to set up an environment where it can properly connect to a web server (configuring ports, urls, etc.)
* Running the test could be slower and limit how much testing you can do
* The web server itself could contain bugs so you couldn't with certainty isolate Foo as the problematic part of the test
* You can't easily configure the values returned from the web server to test sending various inputs to the Foo class

However, if we create a mock class for IWebServer then we can address all these problems:

```csharp
public class MockWebServer : IWebServer
{
    ...
}
```

Then hook it up in our installer:

```csharp
Container.Bind<IWebServer>().To<MockWebServer>().AsSingle();
```

Then you can implement the fields of the IWebServer interface and configure them based on what you want to test on Foo. Hopefully You can see how this can make life when writing tests much easier.

Zenject also allows you to even avoid having to write the MockWebServer class in favour of using a library called "Moq" which does all the work for you.

Note that by default, Auto-mocking is not enabled in Zenject.  If you wish to use the auto-mocking feature then you need to go to your Zenject install directory and extract the contents of "Extras/ZenjectAutoMocking.zip".  Note also that AutoMocking is incompatible with webplayer builds, and you will also need to change your "Api Compatibility Level" from ".NET 2.0 Subset" to ".NET 2.0" (you can find this in PC build settings)

After extracting the auto mocking package it is just a matter of using the following syntax to mock out various parts of your project:

```csharp
Container.Bind<IFoo>().ToMock();
```

However, this approach will not allow you to take advantage of the advanced features of Moq.  For more advanced usages, see the documentation for Moq

## <a id="questions"></a>Frequently Asked Questions

* **<a id="isthisoverkill"></a>Isn't this overkill?  I mean, is using statically accessible singletons really that bad?**

    For small enough projects, I would agree with you that using a global singleton might be easier and less complicated.  But as your project grows in size, using global singletons will make your code unwieldy.  Good code is basically synonymous with loosely coupled code, and to write loosely coupled code you need to (A) actually be aware of the dependencies between classes and (B) code to interfaces (however I don't literally mean to use interfaces everywhere, as explained [here](#overusinginterfaces))

    In terms of (A), using global singletons, it's not obvious at all what depends on what, and over time your code will become really convoluted, as everything will tend towards depending on everything.  There could always be some method somewhere deep in a call stack that does some hail mary request to some other class anywhere in your code base.  In terms of (B), you can't really code to interfaces with global singletons because you're always referring to a concrete class

    With a DI framework, in terms of (A), it's a bit more work to declare the dependencies you need up-front in your constructor, but this can be a good thing too because it forces you to be aware of the dependencies between classes.

    And in terms of (B), it also forces you to code to interfaces.  By declaring all your dependencies as constructor parameters, you are basically saying "in order for me to do X, I need these contracts fulfilled".  These constructor parameters might not actually be interfaces or abstract classes, but it doesn't matter - in an abstract sense, they are still contracts, which isn't the case when you are creating them within the class or using global singletons.

    Then the result will be more loosely coupled code, which will make it 100x easier to refactor, maintain, test, understand, re-use, etc.

* **<a id="aot-support"></a>Does this work on AOT platforms such as iOS and WebGL?**

    Yes.  However, there are a few things that you should be aware.  One of the things that Unity's IL2CPP compiler does is strip out any code that is not used.  It calculates what code is used by statically analyzing the code to find usage.  This is great, except that this will miss any methods/types that are not used explicitly.  In particular, any classes that are created solely through Zenject will have their constructors ignored by the IL2CPP compiler.  In order to address this, the [Inject] attribute that is sometimes applied to constructors also serves to automatically mark the constructor to IL2CPP to not strip out.   In other words, to fix this issue all you have to do is mark every constructor that you create through Zenject with an [Inject] attribute when compiling for WebGL / iOS.

* **<a id="faq-performance"></a>How is performance?**

    DI can affect start-up time when it builds the initial object graph. However it can also affect performance any time you instantiate new objects at run time.

    Zenject uses C# reflection which is typically slow, but in Zenject this work is cached so any performance hits only occur once for each class type.  In other words, Zenject avoids costly reflection operations by making a trade-off between performance and memory to ensure good performance.

    For some benchmarks on Zenject versus other DI frameworks, see [here](https://github.com/svermeulen/IocPerformance/tree/Zenject).

    Zenject should also produce zero per-frame heap allocations.

* **<a id="net-framework"></a>Can I use .NET framework 4.0 and above?**

    By default Unity uses .NET framework 3.5 and so Zenject assumes that this is what you want.  If you are compiling Zenject with a version greater than this, this is fine, but you'll have to either delete or comment out the contents of Func.cs.

* **<a id="howtousecoroutines"></a>How do I use Unity style Coroutines in normal C# classes?**

    With Zenject, there is less of a need to make every class a `MonoBehaviour`.  But it is often still desirable to be able to call `StartCoroutine` to add asynchronous methods.

    One solution here is to use a dedicated class and just call `StartCoroutine` on that instead.  For example:

        public class AsyncProcessor : MonoBehaviour
        {
            // Purposely left empty
        }

        public class Foo : IInitializable
        {
            AsyncProcessor _asyncProcessor;

            public Foo(AsyncProcessor asyncProcessor)
            {
                _asyncProcessor = asyncProcessor;
            }

            public void Initialize()
            {
                _asyncProcessor.StartCoroutine(RunAsync());
            }

            public IEnumerator RunAsync()
            {
                Debug.Log("Foo started");
                yield return new WaitForSeconds(2.0f);
                Debug.Log("Foo finished");
            }
        }

        public class TestInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                Container.Bind<IInitializable>().To<Foo>().AsSingle();
                Container.Bind<AsyncProcessor>().ToSingleGameObject();
            }
        }

    If you need more control than this, another option is to use a coroutine library that implements similar functionality to what Unity provides.  This is what we do.  See [here](https://github.com/svermeulen/UnityCoroutinesWithoutMonoBehaviours) for the library that we use for this.

* **<a id="memorypools"></a>How do I use Zenject with pools to minimize memory allocations?**

    Currently, Zenject does not support memory pooling.  When you bind something to transient or use a factory, Zenject will always create a brand new instance from scratch.  We realize that this can be inefficient in cases where you are creating many objects (especially on mobile) so it is something we want to address at some point.

## <a id="cheatsheet"></a>Installers Cheat-Sheet

Below are a bunch of randomly assorted examples of bindings that you might include in one of your installers.

For more examples, you may also be interested in reading some of the Unit tests (see `Zenject/OptionalExtras/UnitTests` and `Zenject/OptionalExtras/IntegrationTests` directories)

```csharp

///////////// ToTransient

// Create a new instance of Foo for every class that asks for it
Container.Bind<Foo>().ToTransient();

// Create a new instance of Foo for every class that asks for an IFoo
Container.Bind<IFoo>().ToTransient<Foo>();

// Non generic versions
Container.Bind(typeof(IFoo)).ToTransient();
Container.Bind(typeof(IFoo)).ToTransient(typeof(Foo));

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

// This is equivalent to ToTransient
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
Container.Bind<IFoo>().ToTransient<Foo>().When(
    ctx => ctx.AllObjectTypes.Contains(typeof(Bar)));

///////////// Complex conditions example

var foo1 = new Foo();
var foo2 = new Foo();

Container.Bind<Bar>("Bar1").ToTransient();
Container.Bind<Bar>("Bar2").ToTransient();

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

## <a id="further-help"></a>Further Help

For general troubleshooting / support, please use the [zenject subreddit](http://www.reddit.com/r/zenject) or the [zenject google group](https://groups.google.com/forum/#!forum/zenject/).  If you have found a bug, you are also welcome to create an issue on the [github page](https://github.com/modesttree/Zenject), or a pull request if you have a fix / extension.  You can also follow [@Zenject](https://twitter.com/Zenject) on twitter for updates.  Finally, you can also email me directly at sfvermeulen@gmail.com

## <a id="release-notes"></a>Release Notes

3.10 (March 26, 2016)
- Fixed to actually support Windows Store platform
- Added pause/resume methods to TickableManager
- Bug fix - OnlyInjectWhenActive flag did not work on root inactive game objects 

3.9 (Feb 7, 2016)
- Added a lot more error checking when using the ToSingle bindings. It will no longer allow mixing different ToSingle types
- Fixed ToSingleGameObject and ToSingleMonoBehaviour to allow multiple bindings to the same result
- Made it easier to construct SceneCompositionRoot objects dynamically
- Added untyped versions of BindIFactory.ToFactory method
- Removed the ability warn on missing ITickable/IInitializable bindings
- Added a bunch of integration tests
- Reorganized folder structure

3.8 (Feb 4, 2016)
- Changed back to only initializing the ProjectCompositionRoot when starting a scene with a SceneCompositionRoot rather than always starting it in every scene

3.7 (Jan 31, 2016)
- Changed to not bother parenting transforms to the CompositionRoot object by default (This is still optional with a checkbox however)
- Added string parameter to BindMonoBehaviourFactory method to allow specifying the name of an empty GameObject to use for organization
- Changed FacadeFactory to inherit from IFactory
- Changed ProjectCompositionRoot to initialize using Unity's new [RuntimeInitializeOnLoadMethod] attribute
- Added easier ability to validate specific scenes from the command line outside of Unity
- Added AutoBindInstaller class and ZenjectBinding attribute to make it easier to add MonoBehaviours that start in the scene to the container
- Added optional parameter to the [Inject] attribute to specify which container to retrieve from in the case of nested containers
- Fixed some unity-specific bind commands to play more nicely with interfaces

3.6 (Jan 24, 2016)
- Another change to signals to not require parameter types to the bind methods

3.5 (Jan 17, 2016)
- Made breaking change to require separate bind commands for signals and triggers, to allow adding different conditionals on each.

3.4 (Jan 7, 2016)
- Cleaned up directory structure
- Fixed bug with Global bindings not getting their Tick() called in the correct order
- Fixes to the releases automation scripts

3.2 (December 20, 2015)
- Added the concept of "Commands" and "Signals".  See documentation for details.
- Fixed validation for decorator scenes that open decorator scenes.
- Changed to be more strict when using a combination of differents kinds of ToSingle<>, since there should only be one way to create the singleton.
- Added ToSingleFactory bind method, for cases where you have complex construction requirements and don't want to use ToSingleMethod
- Removed the InjectFullScene flag on SceneCompositionRoot.  Now always injects on the full scene.
- Renamed AllowNullBindings to IsValidating so it can be used for other kinds of validation-only logic
- Renamed BinderUntyped to UntypedBinder and BinderGeneric to GenericBinder
- Added the ability to install MonoInstaller's directly from inside other installers by calling Container.Install<MyCustomMonoInstaller>().  In this case it tries to load a prefab from Resources/Installers/MyCustomMonoInstaller.prefab before giving up.  This can be helpful to keep scenes incredibly small instead of having many installer prefabs.
- Added the ability to install MonoInstaller's directly from inside other installers.  In this case it tries to load a prefab from the resources directory before giving up.
- Added some better error output in a few places
- Fixed some iOS AOT issues
- Added BindFacade<> method to DiContainer, to allow creating nested containers without needing to use a factory.
- Added an Open button in scene decorator comp root for easily jumping to the decorated scene
- Removed support for object graph visualization since I hadn't bothered maintaining it
- Got the optional Moq extension method ToMock() working again
- Fixed scene decorators to play more nicely with Unity's own way of handling LoadLevelAdditive.  Decorated scenes are now organized in the scene heirarchy under scene headings just like when calling LoadLevelAdditive normally

3.1
- Changes related to upgrading to Unity 5.3
- Fixed again to make zero heap allocations per frame

3.0
- Added much better support for nested containers.  It now works more closely to what you might expect:  Any parent dependencies are always inherited in sub-containers, even for optional injectables.  Also removed BindScope and FallbackContainer since these were really just workarounds for this feature missing.  Also added [InjectLocal] attribute for cases where you want to inject dependencies only from the local container.
- Changed the way execution order is specified in the installers.  Now the order for Initialize / Tick / Dispose are all given by one property similar to how unity does it, using ExecutionOrderInstaller
- Added ability to pass arguments to Container.Install<>
- Added support for using Facade pattern in combination with nested containers to allow easily created distinct 'islands' of dependencies.  See documentation for details
- Changed validation to be executed on DiContainer instead of through BindingValidator for ease of use
- Added automatic support for WebGL by marking constructors as [Inject]

2.8
* Fixed to properly use explicit default parameter values in Constructor/PostInject methods.  For eg: public Foo(int bar = 5) should consider bar to be optional and use 5 if not resolved.

2.7
* Bug fix to ensure global composition root always gets initialized before the scene composition root
* Changed scene decorators to use LoadLevelAdditive instead of LoadLevel to allow more complex setups involving potentially several decorators within decorators

2.6
* Added new bind methods: ToResource, ToTransientPrefabResource, ToSinglePrefabResource
* Added ability to have multiple sets of global installers
* Fixed support for using zenject with .NET 4.5
* Created abstract base class CompositionRoot for both SceneCompositionRoot and ProjectCompositionRoot
* Better support for using the same DiContainer from multiple threads
* Added back custom list inspector handler to make it easier to re-arrange etc.
* Removed the extension methods on DiContainer to avoid a gotcha that occurs when not including 'using Zenject
* Changed to allow having a null root transform given to DiContainer
* Changed to assume any parameters with hard coded default values (eg: int x = 5) are InjectOptional
* Fixed bug with asteroids project which was causing exceptions to be thrown on the second run due to the use of tags

2.5
* Added support for circular dependencies in the PostInject method or as fields (just not constructor parameters)
* Fixed issue with identifiers that was occurring when having both [Inject] and [InjectOptional] attributes on a field/constructor parameter.  Now requires that only one be set
* Removed BindValue in favour of just using Bind for both reference and value types for simplicity
* Removed GameObjectInstantiator class since it was pretty awkward and confusing.  Moved methods directly into IInstantiator/DiContainer.  See IInstantiator class.
* Extracted IResolver and IBinder interfaces from DiContainer

2.4
* Refactored the way IFactory is used to be a lot cleaner. It now uses a kind of fluent syntax through its own bind method BindIFactory<>

2.3
* Added "ParentContexts" property to InjectContext, to allow very complex conditional bindings that involve potentially several identifiers, etc.
* Removed InjectionHelper class and moved methods into DiContainer to simplify API and also to be more discoverable
* Added ability to build dlls for use in outside unity from the assembly build solution

2.2
* Changed the way installers invoke other installers.  Previously you would Bind them to IInstaller and now you call Container.Install<MyInstaller> instead.  This is better because it allows you to immediately call Rebind<> afterwards

2.1
* Simplified interface a bit more by moving more methods into DiContainer such as Inject and Instantiate.  Moved all helper methods into extension methods for readability. Deleted FieldsInjector and Instantiator classes as part of this
* Renamed DiContainer.To() method to ToInstance since I had witnessed some confusion with it for new users.  Did the same with ToSingleInstance
* Added support for using Zenject outside of Unity by building with the ZEN_NOT_UNITY3D define set
* Bug fix - Validation was not working in some cases for prefabs.
* Renamed some of the parameters in InjectContext for better understandability.
* Renamed DiContainer.ResolveMany to DiContainer.ResolveAll
* Added 'InjectFullScene' flag to CompositionRoot to allow injecting across the entire unity scene instead of just objects underneath the CompositionRoot

2.0

* Added ability to inject dependencies via parameters to the [PostInject] method just like it does with constructors.  Especially useful for MonoBehaviours.
* Fixed the order that [PostInject] methods are called in for prefabs
* Changed singletons created via ToSinglePrefab to identify based on identifier and prefab and not component type. This allows things like ToSingle<Foo>(prefab1) and ToSingle<Bar>(prefab1) to use the same prefab, so you can map singletons to multiple components on the same prefab. This also works with interfaces.
* Removed '.As()' method in favour of specifying the identifier in the first Bind() statement
* Changed identifiers to be strings instead of object to avoid accidental usage
* Renamed ToSingle(obj) to ToSingleInstance to avoid conflict with specifying an identifier
* Fixed validation to work properly for ToSinglePrefab
* Changed to allow using conditions to override a default binding. When multiple providers are found it will now try and use the one with conditions.  So for example you can define a default with `Container.Bind<IFoo>().ToSingle<Foo1>()` and then override for specific classes with `Container.Bind<IFoo>().ToSingle<Foo2>().WhenInjectedInto<Bar>()`, etc.

1.19

* Upgraded to Unity 5
* Added an optional identifier to InjectOptional attribute
* Changed the way priorities are interpreted for tickables, disposables, etc. Zero is now used as default for any unspecified priorities.  This is helpful because it allows you to choose priorities that occur either before or after the unspecified priorities.
* Added some helper methods to ZenEditorUtil for use by CI servers to validate all scenes

1.18

* Added minor optimizations to reduce per-frame allocation to zero
* Fixed unit tests to be compatible with unity test tools
* Minor bug fix with scene decorators, GameObjectInstantiator.

1.17

* Bug fix.  Was not forwarding parameters correctly when instantiating objects from prefabs

1.16

* Removed the word 'ModestTree' from namespaces since Zenject is open source and not proprietary to the company ModestTree.

1.15

* Fixed bug with ToSinglePrefab which was causing it to create multiple instances when used in different bindings.

1.14

* Added flag to CompositionRoot for whether to inject into inactive game objects or ignore them completely
* Added BindAllInterfacesToSingle method to DiContainer
* Changed to call PostInject[] on children first when instantiating from prefab
* Added ILateTickable interface, which works just like ITickable or IFixedTickable for unity's LateUpdate event
* Added support for 'decorators', which can be used to add dependencies to another scene

1.13

* Minor bug fix to global composition root.  Also fixed a few compiler warnings.

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

    The MIT License (MIT)

    Copyright (c) 2010-2015 Modest Tree Media  http://www.modesttree.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.


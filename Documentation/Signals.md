
## Signals

## <a id="theory"></a>Motivation / Theory

Given two classes A and B that need to communicate, your options are usually:

1. Directly call a method on B from A.  In this case, A is strongly coupled with B.
2. Inverse the dependency by having B observe an event on A.  In this case, B is strongly coupled with A.

So, often you have to ask yourself, should A know about B or should B know about A?

As a third option, in some cases it might actually be better for neither one to know about the other. This way your code is kept as loosely coupled as possible.  You can achieve this by having A and B interact with an intermediary object instead of directly with each other.  In Zenject this intermediary object is called a Signal.

## <a id="quick-start"></a>Signals Quick Start

If you just want to get up and running immediately, see the following example which shows basic usage:

```csharp
public class UserJoinedSignal : Signal<UserJoinedSignal, string>
{
}

public class Greeter1
{
    public void SayHello(string userName)
    {
        Debug.Log("Hello " + userName + "!");
    }
}

public class Greeter2 : IInitializable, IDisposable
{
    UserJoinedSignal _userJoinedSignal;

    public Greeter2(UserJoinedSignal userJoinedSignal)
    {
        _userJoinedSignal = userJoinedSignal;
    }

    public void Initialize()
    {
        _userJoinedSignal += OnUserJoined;
    }

    public void Dispose()
    {
        _userJoinedSignal -= OnUserJoined;
    }

    void OnUserJoined(string username)
    {
        Debug.Log("Hello again " + username + "!");
    }
}

public class GameInitializer : IInitializable
{
    readonly UserJoinedSignal _userJoinedSignal;

    public GameInitializer(UserJoinedSignal userJoinedSignal)
    {
        _userJoinedSignal = userJoinedSignal;
    }

    public void Initialize()
    {
        _userJoinedSignal.Fire("Bob");
    }
}

public class GameInstaller : MonoInstaller<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<SignalManager>().AsSingle();
        Container.DeclareSignal<UserJoinedSignal>();

        Container.BindInterfacesTo<Greeter2>().AsSingle();

        Container.BindSignal<string, UserJoinedSignal>()
            .To<Greeter1>(x => x.SayHello).AsSingle();

        Container.BindInterfacesTo<GameInitializer>().AsSingle();
    }
}
```

To run, just create copy and paste the code above into a new file named GameInstaller then create an empty scene with a new scene context and attach the new installer.

As you can see in the the above example, there are two ways of creating signal handlers.  You can either directly bind a handler method to a signal in an installer (Greeter1) or you can have your signal handler attach and detach itself to the signal (Greeter2).

For more details on what's going on above see the following sections.

## <a id="declaration"></a>Signals Declaration

Signals are defined like this:

```csharp
public class DoSomethingSignal : Signal<DoSomethingSignal>
{
}
```

Note that the signal class must provide itself as a generic argument to the Signal base class.

Classes that derive from Signal should always be left empty - their only purpose is to represent a single action.

Any parameters passed along with the signal need to be included as more generic arguments:

```csharp
public class DoSomethingSignal : Signal<DoSomethingSignal, string, int>
{
}
```

In this case, the signal would take a string and an int parameter.

Then in an installer they must be declared somewhere:

```csharp
public override void InstallBindings()
{
    Container.DeclareSignal<DoSomethingSignal>();
}
```

Note that the declaration is the same regardless of the parameter list.

The format of the DeclareSignal statement is the following:

<pre>
Container.DeclareSignal&lt;<b>SignalType</b>&gt;()
    .<b>RequireHandler</b>()
    .When(<b>Condition</b>);
</pre>

The When Condition can be any Zenject condition just like any other binding (see Zenject docs for details).  When using installer handlers (see below) this can be useful to restrict which classes are allowed to fire the signal

The `RequireHandler()` method is optional.  If not included, then the signal will be allowed to fire with zero handlers attached.  If `RequireHandler()` is added to the binding, then an exception will be thrown if the signal is fired and there isn't any handlers attached.

Then in the firing class:

```csharp
public class Bar : ITickable
{
    readonly DoSomethingSignal _signal;

    public Bar(DoSomethingSignal signal)
    {
        _signal = signal;
    }

    public void DoSomething()
    {
        _signal.Fire();
    }
}
```

## <a id="handlers"></a>Signal Handlers

There are three ways of adding handlers to a signal:

1. C# events
2. UniRx Stream
3. Installer Binding

### <a id="handler-events"></a>C# Event Signal Handler

Probably the easiest method to add a handler is to add it directly from within the handler class.  For example:

```csharp
public class Greeter : IInitializable, IDisposable
{
    AppStartedSignal _appStartedSignal;

    public Greeter(AppStartedSignal appStartedSignal)
    {
        _appStartedSignal = appStartedSignal;
    }

    public void Initialize()
    {
        _appStartedSignal += OnAppStarted;
    }

    public void Dispose()
    {
        _appStartedSignal -= OnAppStarted;
    }

    void OnAppStarted()
    {
        Debug.Log("Hello world!");
    }
}
```

Or, equivalently:

```csharp
public class Greeter : IInitializable, IDisposable
{
    AppStartedSignal _appStartedSignal;

    public Greeter(AppStartedSignal appStartedSignal)
    {
        _appStartedSignal = appStartedSignal;
    }

    public void Initialize()
    {
        _appStartedSignal.Listen(OnAppStarted);
    }

    public void Dispose()
    {
        _appStartedSignal.Unlisten(OnAppStarted);
    }

    void OnAppStarted()
    {
        Debug.Log("Hello world!");
    }
}
```

### <a id="handler-unirx"></a>UniRx Signal Handler

If you are a fan of <a href="https://github.com/neuecc/UniRx">UniRx</a>, as we are, then you might also want to treat the signal as a UniRx stream.  For example:

```csharp
public class Greeter : MonoBehaviour
{
    [Inject]
    AppStartedSignal _appStartedSignal;

    public void Start()
    {
        _appStartedSignal.Stream.Subscribe(OnAppStarted).AddTo(this);
    }

    void OnAppStarted()
    {
        Debug.Log("Hello World!");
    }
}
```

NOTE:  Integration with UniRx is disabled by default.  To enable, you must add the define `ZEN_SIGNALS_ADD_UNIRX` to your project, which you can do by selecting Edit -> Project Settings -> Player and then adding `ZEN_SIGNALS_ADD_UNIRX` in the "Scripting Define Symbols" section

### <a id="handler-binding"></a>Installer Binding Signal Handler

Finally, you can also add signal handlers directly within an installer.  For example:

```csharp
public class Greeter1
{
    public void SayHello()
    {
        Debug.Log("Hello!");
    }
}

public class GreeterInstaller : MonoInstaller<GreeterInstaller>
{
    public override void InstallBindings()
    {
        Container.BindSignal<AppStartedSignal>()
            .To<Greeter1>(x => x.SayHello).AsSingle();
    }
}
```

Or, when the signal has parameters:

```csharp
public class Greeter1
{
    public void SayHello(string name)
    {
        Debug.Log("Hello " + name + "!");
    }
}

public class GreeterInstaller : MonoInstaller<GreeterInstaller>
{
    public override void InstallBindings()
    {
        Container.BindSignal<string, AppStartedSignal>()
            .To<Greeter1>(x => x.SayHello).AsSingle();
    }
}
```

This approach has the following advantages:
- More flexible, because you can wire it up in the installer and can have multiple installer configurations
- More loosely coupled, because the handler class can remain completely ignorant of the signal
- Less error prone, because you don't have to remember to unsubscribe.  The signal will automatically be unsubscribed when the 'context' is disposed of.  This means that if you add a handler within a sub-container, the handler will automatically unsubscribe when the sub-container is disposed of
- You can more easily control which classes are allowed to fire the signal.  You can do this by adding a When() conditional to the declaration.  (You can't do this with the other handler types because the listener also needs access to the signal to add itself to it)

Which approach to signal handlers depends on the specifics of each case and personal preference.

### Signals With Subcontainers

One interesting feature of signals is that the signal handlers do not need to be in the same container as the signal declaration.  The declaration can either be in the same container, a parent container, or a sub-container, and it should trigger the handlers regardless of where they are declared.  Note that the declaration will however determine which container the signal can be fired from (the signal itself will be accessible as a dependency for the container it is declared in and all sub-containers just like other bindings)

For example, you can declare a signal in your ProjectContext and then add signal handlers for each particular scene.  Then, when each scene exits, the signal handler that was added in that scene will no longer be called when the signal is fired.

Or, you could add signal handlers in the ProjectContext and then declare the signal in some particular scene.

For example, You might use this to implement your GUI entirely in its own scene, loaded alongside the main backend scene.  Then you could have the GUI scene strictly fire Signals, which would then have method bindings in the game scene.

Something else you'll have noticed in the ExitGameSignal example above is that we also needed to install the SignalManager class:

```csharp
Container.Bind<SignalManager>().AsSingle();
```

This needs to be installed somewhere, and it doesn't matter which container this is installed to, as long as the container is "above" or equal to every place that either signals or signal handlers are used.  Therefore, the ideal place to put this line would be in the ProjectContext since this is the highest level container that exists.

### Signals With Identifiers

If you want to define multiple instances of the same signal, you would need to use identifiers.  This works identically to how normal zenject binding identifiers work. For example:

```csharp
Container.DeclareSignal<FooSignal>().WithId("foo");
```

Then for installer handlers:

```csharp
Container.BindSignal<FooSignal>().WithId("foo").To<Bar>(x => x.DoSomething).AsSingle();
```

Then to access it to fire it, or to add a C# event / unirx handlers:

```csharp
public class Qux
{
    FooSignal _signal;

    public Qux(
        [Inject(Id = "foo")] FooSignal signal)
    {
        _signal = signal;
    }

    public void Run()
    {
        _signal.Fire();
    }
}
```


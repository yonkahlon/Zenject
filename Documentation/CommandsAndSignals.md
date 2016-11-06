
## <a id="signals-and-commands"></a>Signals And Commands

Zenject also includes an optional extension that allows you to define "Commands" and "Signals".

A signal can be thought of as a single event, that when triggered will notify a number of listeners.  A command is an object with a single method named `Execute`, that will forward the request to a specific handler.

The advantage of using Signals and Commands is that the result will often be more loosely coupled code.  Given two classes A and B that need to communicate, your options are usually:

1. Directly call a method on B from A.  In this case, A is strongly coupled with B.
2. Inverse the dependency by having B observe an event on A.  In this case, B is strongly coupled with A.

Both cases result in the classes being coupled in some way.  Now if instead you create a command object, which is called by A and which invokes a method on B, then the result is less coupling.  Granted, A is still coupled to the command class, but in some cases that is better than being directly coupled to B.  Using signals works similarly, in that you can remove the coupling by having A trigger a signal, which is observed by B.

## <a id="signals"></a>Signals

Signals are defined like this:

```csharp
public class PressedButtonSignal : Signal<PressedButtonSignal>
{
}
```

Then in an Installer:

```csharp
public override void InstallBindings()
{
    Container.BindSignal<PressedButtonSignal>();
}
```

Then in the firing class:

```csharp
public class Bar
{
    readonly PressedButtonSignal _signal;

    public Bar(PressedButtonSignal signal)
    {
        _signal = signal;
    }

    public void DoSomething()
    {
        _signal.Fire();
    }
}
```

And in the listening class:

```csharp
public class Foo : IInitializable, IDisposable
{
    PressedButtonSignal _signal;

    public Foo1(PressedButtonSignal signal)
    {
        _signal = signal;
    }

    public void Initialize()
    {
        _signal += OnPressed;

        // You can also do this which is equivalent
        // _signal.Listen(OnPressed);
    }

    public void Dispose()
    {
        _signal -= OnPressed;

        // You can also do this which is equivalent
        // _signal.Unlisten(OnPressed);
    }

    void OnPressed()
    {
        Debug.Log("Received OnPressed event");
    }
}
```

Signals can be especially useful for system-wide events that are not associated with any particular class.

Something else worth noting is that signals will throw exceptions if all listeners have not properly removed themselves by the time the scene exits.  So for example, if we were to remove the line `_signal -= OnPressed` from above, we would see error messages in the log.  If this behaviour is too strict for your liking, you might consider commenting out the assert in the Signal.Dispose methods.

Also note that you can add parameters to your signals by adding the parameter types to the generic arguments of the Signal base class.  For example:

```csharp
public class PressedButtonSignal : Signal<PressedButtonSignal, string>
{
}

public class Bar
{
    readonly PressedButtonSignal _signal;

    public Bar(PressedButtonSignal signal)
    {
        _signal = signal;
    }

    public void DoSomething()
    {
        _signal.Fire("some data");
    }
}

public class Foo : IInitializable, IDisposable
{
    PressedButtonSignal _signal;

    public Foo1(PressedButtonSignal signal)
    {
        _signal = signal;
    }

    public void Initialize()
    {
        _signal += OnPressed;
    }

    public void Dispose()
    {
        _signal -= OnPressed;
    }

    void OnPressed(string data)
    {
        Debug.Log("Received OnPressed event with data: " + data);
    }
}
```

## <a id="commands"></a>Commands

Commands are defined like this

```csharp
public class ResetSceneCommand : Command { }

public class ResetSceneCommandWithParameter : Command<string> { }
```

Unlike with signals, there are several different ways of declaring a command in an installer.  Perhaps the simplest way would be the following:

```csharp
public override void InstallBindings()
{
    ...
    Container.BindCommand<ResetSceneCommand>().To<ResetSceneHandler>(x => x.Reset).AsSingle();
    ...
    Container.BindCommand<ResetSceneCommandWithParameter, string>().To<ResetSceneHandler>(x => x.Reset).AsSingle();
    ...
}

public class ResetSceneHandler
{
    public void Reset()
    {
        ... [reset scene] ...
    }
}
```

This bind statement will result in an object of type `ResetSceneCommand` being added to the container.  Any time a class calls Execute on `ResetSceneCommand`, it will trigger the Reset method on the `ResetSceneHandler` class.  For example:

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
    Container.BindCommand<ResetSceneCommand>().To<ResetSceneHandler>(x => x.Reset).AsSingle().WhenInjectedInto<Foo>();
    ...
}
```

Note that in this case we are using `AsSingle` - this means that the same instance of `ResetSceneHandler` will be used every time the command is executed.  Alternatively, you could declare it using `AsTransient<>` which would instantiate a new instance of `ResetSceneHandler` every time Execute() is called.  For example:

```csharp
Container.BindCommand<ResetSceneCommand>().To<ResetSceneHandler>(x => x.Reset).AsTransient();
```

This might be useful if the `ResetSceneCommand` class involves some long-running operations that require unique sets of member variables/dependencies.


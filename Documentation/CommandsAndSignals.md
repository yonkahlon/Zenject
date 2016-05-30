
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


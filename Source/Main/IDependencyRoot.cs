using System;

namespace Zenject
{
    // It is helpful to have a standard interface represent the root of the object graph
    // One reason is to be able to validate any container by just checking IDependencyRoot
    // Another reason is that it allows for a place to force instantiate classes for cases where they would
    // not be included in the object graph otherwise
    // eg: Container.Bind<object>().ToSingle<Foo>().WhenInjectedInto<IDependencyRoot>();
    // (everything deriving from IDependencyRoot should have an optional list of objects)
    public interface IDependencyRoot
    {
    }
}

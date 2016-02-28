using System;

namespace Zenject
{
    // This value will determine which container the binding will use to create the given object
    // So for example, given this binding:
    //
    //      Container.Bind<Foo>().ToTransient<Foo>(ContainerTypes.RuntimeContainer)
    //
    // If we are creating a class Bar that has Foo as a constructor parameter, then Foo
    // will be created using the same container that Bar was created in.  If instead,
    // we use this binding:
    //
    //      Container.Bind<Foo>().ToTransient<Foo>(ContainerTypes.BindContainer)
    //
    // Then Foo will be created using the same 'Container' instance that it was bound
    // with.
    //
    // Note that when not provided we assume RuntimeContainer
    public enum ContainerTypes
    {
        BindContainer,
        RuntimeContainer,
    }
}

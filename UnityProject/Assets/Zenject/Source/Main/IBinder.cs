using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IBinder
    {
        // _____ Bind<> _____
        // Map the given type to a way of obtaining it
        // See ITypeBinderOld.cs for the full list of methods to call on the return value
        // Note that this can include open generic types as well such as List<>
        TypeBinderGeneric<TContract> Bind<TContract>(string identifier);
        TypeBinderGeneric<TContract> Bind<TContract>();

        // _____ Bind _____
        // Non-generic version of Bind<> for cases where you only have the runtime type
        // Note that this can include open generic types as well such as List<>
        TypeBinderNonGeneric Bind(params Type[] contractTypes);
        TypeBinderNonGeneric Bind(string identifier, params Type[] contractTypes);

        // _____ BindFactory<> _____
        // TBD
        FactoryBinder<TContract, TFactory> BindFactory<TContract, TFactory>()
            where TFactory : Factory<TContract>;

        FactoryBinder<TContract, TFactory> BindFactory<TContract, TFactory>(string identifier)
            where TFactory : Factory<TContract>;

        FactoryBinder<TParam1, TContract, TFactory> BindFactory<TParam1, TContract, TFactory>()
            where TFactory : Factory<TParam1, TContract>;

        FactoryBinder<TParam1, TContract, TFactory> BindFactory<TParam1, TContract, TFactory>(string identifier)
            where TFactory : Factory<TParam1, TContract>;

        FactoryBinder<TParam1, TParam2, TContract, TFactory> BindFactory<TParam1, TParam2, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TContract>;

        FactoryBinder<TParam1, TParam2, TContract, TFactory> BindFactory<TParam1, TParam2, TContract, TFactory>(string identifier)
            where TFactory : Factory<TParam1, TParam2, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TContract, TFactory>(string identifier)
            where TFactory : Factory<TParam1, TParam2, TParam3, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TParam4, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TParam4, TContract, TFactory>(string identifier)
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory>()
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>;

        FactoryBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory> BindFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract, TFactory>(string identifier)
            where TFactory : Factory<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>;

        // _____ BindAllInterfaces _____
        // Bind all the interfaces for the given type to the same thing.
        //
        // Example:
        //
        //    public class Foo : ITickable, IInitializable
        //    {
        //    }
        //
        //    Container.BindAllInterfaces<Foo>().To<Foo>().AsSingle();
        //
        //  This line above is equivalent to the following:
        //
        //    Container.Bind<ITickable>().To<Foo>().AsSingle();
        //    Container.Bind<IInitializable>().To<Foo>().AsSingle();
        //
        // Note here that we do not bind Foo to itself.  For that, use BindAllInterfacesAndSelf
        TypeBinderNonGeneric BindAllInterfaces<T>();
        TypeBinderNonGeneric BindAllInterfaces(Type type);

        // _____ BindAllInterfacesAndSelf _____
        // Same as BindAllInterfaces except also binds to self
        TypeBinderNonGeneric BindAllInterfacesAndSelf<T>();
        TypeBinderNonGeneric BindAllInterfacesAndSelf(Type type);

        // _____ BindInstance _____
        //  This is simply a shortcut to using the ToInstance method.
        //
        //  Example:
        //      Container.BindInstance(new Foo());
        //
        //  This line above is equivalent to the following:
        //
        //      Container.Bind<Foo>().ToInstance(new Foo());
        //
        ScopeBinder BindInstance<TContract>(string identifier, TContract obj);
        ScopeBinder BindInstance<TContract>(TContract obj);

        // _____ HasBinding _____
        // Returns true if the given type is bound to something in the container
        bool HasBinding(InjectContext context);
        bool HasBinding<TContract>();
        bool HasBinding<TContract>(string identifier);

        // _____ Install _____
        // This will cause the container to Inject all dependencies on the given installers,
        // and then call InstallBindings() on them to add more bindings to the container.
        void Install(IEnumerable<IInstaller> installers);
        void Install(IInstaller installer);
        void Install(IInstaller installer, IEnumerable<object> extraArgs);

        // _____ Install _____
        // This will create a new instance of the given type, inject all dependencies into it,
        // and then call InstallBindings() on them to add more bindings to the container.
        void Install<T>()
            where T : IInstaller;

        void Install<T>(IEnumerable<object> extraArgs)
            where T : IInstaller;

        void Install(Type installerType);
        void Install(Type installerType, IEnumerable<object> extraArgs);

        // This is only necessary if you have to pass in null values as parameters to the installer
        void InstallExplicit(IInstaller installer, List<TypeValuePair> extraArgs);
        void InstallExplicit(Type installerType, List<TypeValuePair> extraArgs);

        // _____ Install _____
        // Returns true if the given installer type has already been installed on this container
        bool HasInstalled(Type installerType);

        bool HasInstalled<T>()
            where T : IInstaller;

        // Returns true if the DiContainer waas made for validation purposes only
        // This is run at edit time.  This is one of the reasons you should never instantiate
        // any objects during the bind phase.
        bool IsValidating
        {
            get;
            set;
        }
    }
}


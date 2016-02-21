using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using Zenject.Internal;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer : IInstantiator, IResolver, IBinder
    {
#if !ZEN_NOT_UNITY3D
        public const string DefaultParentId = "InstantiateDefaultParent";
#endif

        readonly Dictionary<BindingId, List<ProviderBase>> _providers = new Dictionary<BindingId, List<ProviderBase>>();
        readonly HashSet<Type> _installedInstallers = new HashSet<Type>();
        readonly Stack<Type> _installsInProgress = new Stack<Type>();
        readonly DiContainer _parentContainer;
        readonly Stack<LookupId> _resolvesInProgress = new Stack<LookupId>();
        readonly SingletonProviderCreator _singletonProviderFactory;
        readonly SingletonRegistry _singletonRegistry;
        readonly bool _isValidating;

#if !ZEN_NOT_UNITY3D
        bool _hasLookedUpParent;
        Transform _defaultParent;
#endif

        public DiContainer(bool isValidating)
        {
            _isValidating = isValidating;

            _singletonRegistry = new SingletonRegistry();
            _singletonProviderFactory = new SingletonProviderCreator(this, _singletonRegistry);

            Binder.Bind<DiContainer>().ToInstance(this);
            Binder.Bind<IBinder>().ToInstance(this);
            Binder.Bind<IResolver>().ToInstance(this);
            Binder.Bind<IInstantiator>().ToInstance(this);
        }

        public DiContainer()
            : this(false)
        {
        }

        public DiContainer(DiContainer parentContainer, bool isValidating)
            : this(isValidating)
        {
            _parentContainer = parentContainer;
        }

        public DiContainer(DiContainer parentContainer)
            : this(parentContainer, false)
        {
        }

        public IResolver Resolver
        {
            get
            {
                return this;
            }
        }

        public IBinder Binder
        {
            get
            {
                return this;
            }
        }

        public IInstantiator Instantiator
        {
            get
            {
                return this;
            }
        }

        public SingletonProviderCreator SingletonProviderCreator
        {
            get
            {
                return _singletonProviderFactory;
            }
        }

        public SingletonRegistry SingletonRegistry
        {
            get
            {
                return _singletonRegistry;
            }
        }

#if !ZEN_NOT_UNITY3D
        public Transform DefaultParent
        {
            get
            {
                // We should be able to cache this since we should be able to assume that
                // this property isn't called until after the install phase
                if (!_hasLookedUpParent)
                {
                    _hasLookedUpParent = true;

                    // Use an InjectContext so we can specify local = true
                    // and optional = true
                    var ctx = new InjectContext(
                        this, typeof(Transform), DefaultParentId,
                        true, null, null, "", null, null, null, InjectSources.Local);

                    _defaultParent = Resolver.Resolve<Transform>(ctx);
                }

                return _defaultParent;
            }
        }
#endif

        public DiContainer ParentContainer
        {
            get
            {
                return _parentContainer;
            }
        }

        public bool ChecksForCircularDependencies
        {
            get
            {
#if ZEN_MULTITHREADING
                // When multithreading is supported we can't use a static field to track the lookup
                // TODO: We could look at the inject context though
                return false;
#else
                return true;
#endif
            }
        }

        public IEnumerable<Type> InstalledInstallers
        {
            get
            {
                return _installedInstallers;
            }
        }

        // True if this container was created for the purposes of validation
        // Useful to avoid instantiating things that we shouldn't during this step
        bool IBinder.IsValidating
        {
            get
            {
                return _isValidating;
            }
        }

        public IEnumerable<BindingId> AllContracts
        {
            get
            {
                return _providers.Keys;
            }
        }

        // Note that this list is not exhaustive or even accurate so use with caution
        public IEnumerable<Type> AllConcreteTypes
        {
            get
            {
                return (from x in _providers from p in x.Value select p.GetInstanceType()).Where(x => x != null && !x.IsInterface && !x.IsAbstract).Distinct();
            }
        }

        public DiContainer CreateSubContainer()
        {
            return new DiContainer(this, _isValidating);
        }

        public void RegisterProvider(
            ProviderBase provider, BindingId bindingId)
        {
            if (_providers.ContainsKey(bindingId))
            {
                // Prevent duplicate singleton bindings:
                if (_providers[bindingId].Find(item => ReferenceEquals(item, provider)) != null)
                {
                    throw new ZenjectBindException(
                        "Found duplicate singleton binding for contract '{0}' and id '{1}'".Fmt(bindingId.Type, bindingId.Identifier));
                }

                _providers[bindingId].Add(provider);
            }
            else
            {
                _providers.Add(bindingId, new List<ProviderBase> {provider});
            }
        }

        public int UnregisterProvider(ProviderBase provider)
        {
            int numRemoved = 0;

            foreach (var keyValue in _providers)
            {
                numRemoved += keyValue.Value.RemoveAll(x => x == provider);
            }

            Assert.That(numRemoved > 0, "Tried to unregister provider that was not registered");

            // Remove any empty contracts
            foreach (var bindingId in _providers.Where(x => x.Value.IsEmpty()).Select(x => x.Key).ToList())
            {
                _providers.Remove(bindingId);
            }

            provider.Dispose();

            return numRemoved;
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateResolve<TContract>()
        {
            return Resolver.ValidateResolve<TContract>((string)null);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateResolve<TContract>(string identifier)
        {
            return Resolver.ValidateResolve(typeof(TContract), identifier);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateResolve(Type contractType)
        {
            return Resolver.ValidateResolve(contractType, (string)null);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateResolve(Type contractType, string identifier)
        {
            return Resolver.ValidateResolve(new InjectContext(this, contractType, identifier));
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateAll(params Type[] ignoreTypes)
        {
            foreach (var error in Resolver.ValidateValidatables(ignoreTypes))
            {
                yield return error;
            }

            // Use ToList() in case it changes somehow during iteration
            foreach (var pair in _providers.ToList())
            {
                var bindingId = pair.Key;

                if (ignoreTypes.Where(i => bindingId.Type.DerivesFromOrEqual(i)).Any())
                {
                    continue;
                }

                foreach (var provider in pair.Value)
                {
                    var injectCtx = new InjectContext(
                        this, bindingId.Type, bindingId.Identifier);

                    foreach (var error in provider.ValidateBinding(injectCtx))
                    {
                        yield return error;
                    }
                }
            }
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateValidatables(params Type[] ignoreTypes)
        {
            // Use ToList() in case it changes somehow during iteration
            foreach (var pair in _providers.ToList())
            {
                var bindingId = pair.Key;

                if (ignoreTypes.Where(i => bindingId.Type.DerivesFromOrEqual(i)).Any())
                {
                    continue;
                }

                // Validate all IValidatableFactory's
                List<ProviderBase> validatableFactoryProviders;

                var providers = pair.Value;

                if (bindingId.Type.DerivesFrom<IValidatableFactory>())
                {
                    validatableFactoryProviders = providers;
                }
                else
                {
                    validatableFactoryProviders = providers.Where(x => x.GetInstanceType().DerivesFrom<IValidatableFactory>()).ToList();
                }

                var injectCtx = new InjectContext(this, bindingId.Type, bindingId.Identifier);

                foreach (var provider in validatableFactoryProviders)
                {
                    var factory = (IValidatableFactory)provider.GetInstance(injectCtx);

                    var type = factory.ConstructedType;
                    var providedArgs = factory.ProvidedTypes;

                    foreach (var error in Resolver.ValidateObjectGraph(type, injectCtx, null, providedArgs))
                    {
                        yield return error;
                    }
                }

                // Validate all IValidatable's
                List<ProviderBase> validatableProviders;

                if (bindingId.Type.DerivesFrom<IValidatable>())
                {
                    validatableProviders = providers;
                }
                else
                {
                    validatableProviders = providers.Where(x => x.GetInstanceType().DerivesFrom<IValidatable>()).ToList();
                }

                Assert.That(validatableFactoryProviders.Intersect(validatableProviders).IsEmpty(),
                    "Found provider implementing both IValidatable and IValidatableFactory.  This is not allowed.");

                foreach (var provider in validatableProviders)
                {
                    var factory = (IValidatable)provider.GetInstance(injectCtx);

                    foreach (var error in factory.Validate())
                    {
                        yield return error;
                    }
                }
            }
        }

        // Wrap IEnumerable<> to avoid LINQ mistakes
        internal List<ProviderBase> GetAllProviderMatches(InjectContext context)
        {
            return GetProviderMatchesInternal(context).Select(x => x.Provider).ToList();
        }

        // Be careful with this method since it is a coroutine
        IEnumerable<ProviderPair> GetProviderMatchesInternal(InjectContext context)
        {
            return GetProvidersForContract(context.BindingId, context.SourceType).Where(x => x.Provider.Matches(context));
        }

        IEnumerable<ProviderPair> GetProvidersForContract(BindingId bindingId, InjectSources sourceType)
        {
            switch (sourceType)
            {
                case InjectSources.Local:
                {
                    return GetLocalProviders(bindingId).Select(x => new ProviderPair(x, this));
                }
                case InjectSources.Any:
                {
                    var localPairs = GetLocalProviders(bindingId).Select(x => new ProviderPair(x, this));

                    if (_parentContainer == null)
                    {
                        return localPairs;
                    }

                    return localPairs.Concat(
                        _parentContainer.GetProvidersForContract(bindingId, InjectSources.Any));
                }
                case InjectSources.AnyParent:
                {
                    if (_parentContainer == null)
                    {
                        return Enumerable.Empty<ProviderPair>();
                    }

                    return _parentContainer.GetProvidersForContract(bindingId, InjectSources.Any);
                }
                case InjectSources.Parent:
                {
                    if (_parentContainer == null)
                    {
                        return Enumerable.Empty<ProviderPair>();
                    }

                    return _parentContainer.GetProvidersForContract(bindingId, InjectSources.Local);
                }
            }

            Assert.Throw("Invalid source type");
            return null;
        }

        List<ProviderBase> GetLocalProviders(BindingId bindingId)
        {
            List<ProviderBase> localProviders;

            if (_providers.TryGetValue(bindingId, out localProviders))
            {
                return localProviders;
            }

            // If we are asking for a List<int>, we should also match for any localProviders that are bound to the open generic type List<>
            // Currently it only matches one and not the other - not totally sure if this is better than returning both
            if (bindingId.Type.IsGenericType && _providers.TryGetValue(new BindingId(bindingId.Type.GetGenericTypeDefinition(), bindingId.Identifier), out localProviders))
            {
                return localProviders;
            }

            return new List<ProviderBase>();
        }

        IList IResolver.ResolveAll(InjectContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var matches = GetProviderMatchesInternal(context).ToList();

            if (matches.Any())
            {
                return ReflectionUtil.CreateGenericList(
                    context.MemberType, matches.Select(x => SafeGetInstance(x.Provider, context)).ToArray());
            }

            if (!context.Optional)
            {
                throw new ZenjectResolveException(
                    "Could not find required dependency with type '" + context.MemberType.Name() + "' \nObject graph:\n" + context.GetObjectGraphString());
            }

            return ReflectionUtil.CreateGenericList(context.MemberType, new object[] {});
        }

        List<Type> IResolver.ResolveTypeAll(InjectContext context)
        {
            if (_providers.ContainsKey(context.BindingId))
            {
                return _providers[context.BindingId].Select(x => x.GetInstanceType()).Where(x => x != null).ToList();
            }

            return new List<Type> {};
        }

        void IBinder.Install(IEnumerable<IInstaller> installers)
        {
            foreach (var installer in installers)
            {
                Assert.IsNotNull(installer, "Tried to install a null installer");

                if (installer.IsEnabled)
                {
                    Binder.Install(installer);
                }
            }
        }

        void IBinder.Install(IInstaller installer)
        {
            Assert.That(installer.IsEnabled);

            Resolver.Inject(installer);
            InstallInstallerInternal(installer);
        }

        void IBinder.Install<T>(params object[] extraArgs)
        {
            Binder.Install(typeof(T), extraArgs);
        }

        void IBinder.Install(Type installerType, params object[] extraArgs)
        {
            Assert.That(installerType.DerivesFrom<IInstaller>());

#if !ZEN_NOT_UNITY3D
            if (installerType.DerivesFrom<MonoInstaller>())
            {
                var installer = Instantiator.InstantiatePrefabResourceForComponent<MonoInstaller>("Installers/" + installerType.Name(), extraArgs);

                try
                {
                    InstallInstallerInternal(installer);
                }
                finally
                {
                    // When running, it is nice to keep the installer around so that you can change the settings live
                    // But when validating at edit time, we don't want to add the new game object
                    if (!Application.isPlaying)
                    {
                        GameObject.DestroyImmediate(installer.gameObject);
                    }
                }
            }
            else
#endif
            {
                var installer = (IInstaller)Instantiator.Instantiate(installerType, extraArgs);
                InstallInstallerInternal(installer);
            }
        }

        bool IBinder.HasInstalled<T>()
        {
            return Binder.HasInstalled(typeof(T));
        }

        bool IBinder.HasInstalled(Type installerType)
        {
            return _installedInstallers.Where(x => x == installerType).Any();
        }

        void InstallInstallerInternal(IInstaller installer)
        {
            var installerType = installer.GetType();

            Log.Debug("Installing installer '{0}'", installerType);

            Assert.That(!_installsInProgress.Contains(installerType),
                "Potential infinite loop detected while installing '{0}'", installerType.Name());

            Assert.That(!_installedInstallers.Contains(installerType),
                "Tried installing installer '{0}' twice", installerType.Name());

            _installedInstallers.Add(installerType);
            _installsInProgress.Push(installerType);

            try
            {
                installer.InstallBindings();
            }
            catch (Exception e)
            {
                // This context information is really helpful when bind commands fail
                throw new Exception(
                    "Error occurred while running installer '{0}'".Fmt(installer.GetType().Name()), e);
            }
            finally
            {
                Assert.That(_installsInProgress.Peek().Equals(installerType));
                _installsInProgress.Pop();
            }
        }

        // Try looking up a single provider for a given context
        // Note that this method should not throw zenject exceptions
        internal ProviderLookupResult TryGetUniqueProvider(
            InjectContext context, out ProviderBase provider)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)
            var providers = GetProviderMatchesInternal(context).ToList();

            if (providers.IsEmpty())
            {
                provider = null;
                return ProviderLookupResult.None;
            }

            if (providers.Count > 1)
            {
                // If we find multiple providers and we are looking for just one, then
                // try to intelligently choose one from the list before giving up

                // First try picking the most 'local' dependencies
                // This will bias towards bindings for the lower level specific containers rather than the global high level container
                // This will, for example, allow you to just ask for a DiContainer dependency without needing to specify [Inject(InjectSources.Local)]
                // (otherwise it would always match for a list of DiContainer's for all parent containers)
                var sortedProviders = providers.Select(x => new { Pair = x, Distance = GetContainerHeirarchyDistance(x.Container) }).OrderBy(x => x.Distance).ToList();

                sortedProviders.RemoveAll(x => x.Distance != sortedProviders[0].Distance);

                if (sortedProviders.Count == 1)
                {
                    // We have one match that is the closest
                    provider = sortedProviders[0].Pair.Provider;
                }
                else
                {
                    // Try choosing the one with a condition before giving up and throwing an exception
                    // This is nice because it allows us to bind a default and then override with conditions
                    provider = sortedProviders.Select(x => x.Pair.Provider).Where(x => x.Condition != null).OnlyOrDefault();

                    if (provider == null)
                    {
                        return ProviderLookupResult.Multiple;
                    }
                }
            }
            else
            {
                provider = providers.Single().Provider;
            }

            Assert.IsNotNull(provider);
            return ProviderLookupResult.Success;
        }

        // Return single instance of requested type or assert
        object IResolver.Resolve(InjectContext context)
        {
            ProviderBase provider;

            var result = TryGetUniqueProvider(context, out provider);

            if (result == ProviderLookupResult.Multiple)
            {
                throw new ZenjectResolveException(
                    "Found multiple matches when only one was expected for type '{0}'{1}. \nObject graph:\n {2}"
                    .Fmt(
                        context.MemberType.Name(),
                        (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }

            if (result == ProviderLookupResult.None)
            {
                // If it's a generic list then try matching multiple instances to its generic type
                if (ReflectionUtil.IsGenericList(context.MemberType))
                {
                    var subType = context.MemberType.GetGenericArguments().Single();
                    var subContext = context.ChangeMemberType(subType);

                    return Resolver.ResolveAll(subContext);
                }

                if (context.Optional)
                {
                    return context.FallBackValue;
                }

                throw new ZenjectResolveException(
                    "Unable to resolve type '{0}'{1}. \nObject graph:\n{2}"
                    .Fmt(
                        context.MemberType.Name() + (context.Identifier == null ? "" : " with ID '" + context.Identifier.ToString() + "'"),
                        (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }

            Assert.That(result == ProviderLookupResult.Success);
            Assert.IsNotNull(provider);

            return SafeGetInstance(provider, context);
        }

        object SafeGetInstance(ProviderBase provider, InjectContext context)
        {
            if (ChecksForCircularDependencies)
            {
                var lookupId = new LookupId(provider, context.BindingId);

                // Allow one before giving up so that you can do circular dependencies via postinject or fields
                if (_resolvesInProgress.Where(x => x.Equals(lookupId)).Count() > 1)
                {
                    throw new ZenjectResolveException(
                        "Circular dependency detected! \nObject graph:\n {0}".Fmt(context.GetObjectGraphString()));
                }

                _resolvesInProgress.Push(lookupId);
                try
                {
                    return provider.GetInstance(context);
                }
                finally
                {
                    Assert.That(_resolvesInProgress.Peek().Equals(lookupId));
                    _resolvesInProgress.Pop();
                }
            }
            else
            {
                return provider.GetInstance(context);
            }
        }

        int GetContainerHeirarchyDistance(DiContainer container)
        {
            return GetContainerHeirarchyDistance(container, 0);
        }

        int GetContainerHeirarchyDistance(DiContainer container, int depth)
        {
            if (container == this)
            {
                return depth;
            }

            Assert.IsNotNull(_parentContainer);
            return _parentContainer.GetContainerHeirarchyDistance(container, depth + 1);
        }

        public IEnumerable<Type> GetDependencyContracts<TContract>()
        {
            return GetDependencyContracts(typeof(TContract));
        }

        public IEnumerable<Type> GetDependencyContracts(Type contract)
        {
            foreach (var injectMember in TypeAnalyzer.GetInfo(contract).AllInjectables)
            {
                yield return injectMember.MemberType;
            }
        }

        // Same as Instantiate except you can pass in null value
        // however the type for each parameter needs to be explicitly provided in this case
        object IInstantiator.InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap, InjectContext currentContext, string concreteIdentifier, bool autoInject)
        {
#if PROFILING_ENABLED
            using (ProfileBlock.Start("Zenject.Instantiate({0})", concreteType))
#endif
            {
                return InstantiateInternal(concreteType, extraArgMap, currentContext, concreteIdentifier, autoInject);
            }
        }

        object InstantiateInternal(
            Type concreteType, IEnumerable<TypeValuePair> extraArgs, InjectContext currentContext, string concreteIdentifier, bool autoInject)
        {
#if !ZEN_NOT_UNITY3D
            Assert.That(!concreteType.DerivesFrom<UnityEngine.Component>(),
                "Error occurred while instantiating object of type '{0}'. Instantiator should not be used to create new mono behaviours.  Must use InstantiatePrefabForComponent, InstantiatePrefab, InstantiateComponentOnNewGameObject, InstantiateGameObject, or InstantiateComponent.  You may also want to use GameObjectFactory class or plain old GameObject.Instantiate.", concreteType.Name());
#endif

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);

            if (typeInfo.InjectConstructor == null)
            {
                throw new ZenjectResolveException(
                    "More than one (or zero) constructors found for type '{0}' when creating dependencies.  Use one [Inject] attribute to specify which to use.".Fmt(concreteType));
            }

            // Make a copy since we remove from it below
            var extraArgList = extraArgs.ToList();
            var paramValues = new List<object>();

            foreach (var injectInfo in typeInfo.ConstructorInjectables)
            {
                object value;

                if (!InstantiateUtil.PopValueWithType(extraArgList, injectInfo.MemberType, out value))
                {
                    value = Resolver.Resolve(injectInfo.CreateInjectContext(this, currentContext, null, concreteIdentifier));
                }

                paramValues.Add(value);
            }

            object newObj;

            //Log.Debug("Zenject: Instantiating type '{0}'", concreteType.Name());
            try
            {
#if PROFILING_ENABLED
                using (ProfileBlock.Start("{0}.{0}()", concreteType))
#endif
                {
                    newObj = typeInfo.InjectConstructor.Invoke(paramValues.ToArray());
                }
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    "Error occurred while instantiating object with type '{0}'".Fmt(concreteType.Name()), e);
            }

            if (autoInject)
            {
                Resolver.InjectExplicit(newObj, extraArgList, true, typeInfo, currentContext, concreteIdentifier);
            }
            else
            {
                if (!extraArgList.IsEmpty())
                {
                    throw new ZenjectResolveException(
                        "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                        .Fmt(newObj.GetType().Name(), String.Join(",", extraArgList.Select(x => x.Type.Name()).ToArray()), currentContext.GetObjectGraphString()));
                }
            }

            return newObj;
        }

        // Iterate over fields/properties on the given object and inject any with the [Inject] attribute
        void IResolver.InjectExplicit(
            object injectable, IEnumerable<TypeValuePair> extraArgs,
            bool shouldUseAll, ZenjectTypeInfo typeInfo, InjectContext context,
            string concreteIdentifier)
        {
            Assert.IsEqual(typeInfo.TypeAnalyzed, injectable.GetType());
            Assert.That(injectable != null);

#if !ZEN_NOT_UNITY3D
            Assert.That(injectable.GetType() != typeof(GameObject),
                "Use InjectGameObject to Inject game objects instead of Inject method");
#endif

            // Make a copy since we remove from it below
            var extraArgsList = extraArgs.ToList();

            foreach (var injectInfo in typeInfo.FieldInjectables.Concat(typeInfo.PropertyInjectables))
            {
                object value;

                if (InstantiateUtil.PopValueWithType(extraArgsList, injectInfo.MemberType, out value))
                {
                    injectInfo.Setter(injectable, value);
                }
                else
                {
                    value = Resolver.Resolve(
                        injectInfo.CreateInjectContext(
                            this, context, injectable, concreteIdentifier));

                    if (injectInfo.Optional && value == null)
                    {
                        // Do not override in this case so it retains the hard-coded value
                    }
                    else
                    {
                        injectInfo.Setter(injectable, value);
                    }
                }
            }

            foreach (var method in typeInfo.PostInjectMethods)
            {
#if PROFILING_ENABLED
                using (ProfileBlock.Start("{0}.{1}()", injectable.GetType(), method.MethodInfo.Name))
#endif
                {
                    var paramValues = new List<object>();

                    foreach (var injectInfo in method.InjectableInfo)
                    {
                        object value;

                        if (!InstantiateUtil.PopValueWithType(extraArgsList, injectInfo.MemberType, out value))
                        {
                            value = Resolver.Resolve(
                                injectInfo.CreateInjectContext(this, context, injectable, concreteIdentifier));
                        }

                        paramValues.Add(value);
                    }

                    method.MethodInfo.Invoke(injectable, paramValues.ToArray());
                }
            }

            if (shouldUseAll && !extraArgsList.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                    .Fmt(injectable.GetType().Name(), String.Join(",", extraArgsList.Select(x => x.Type.Name()).ToArray()), context.GetObjectGraphString()));
            }
        }

#if !ZEN_NOT_UNITY3D

        // NOTE: gameobject here is not a prefab prototype, it is an instance
        Component IInstantiator.InstantiateComponent(
            Type componentType, GameObject gameObject, params object[] extraArgMap)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            var monoBehaviour = (Component)gameObject.AddComponent(componentType);
            Resolver.Inject(monoBehaviour, extraArgMap);
            return monoBehaviour;
        }

        GameObject IInstantiator.InstantiatePrefabResourceExplicit(
            string resourcePath, IEnumerable<object> extraArgMap, InjectContext context)
        {
            return Instantiator.InstantiatePrefabResourceExplicit(resourcePath, extraArgMap, context, false);
        }

        GameObject IInstantiator.InstantiatePrefabResourceExplicit(
            string resourcePath, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab, "Could not find prefab at resource location '{0}'".Fmt(resourcePath));
            return Instantiator.InstantiatePrefabExplicit(prefab, extraArgMap, context, includeInactive);
        }

        GameObject IInstantiator.InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context)
        {
            return Instantiator.InstantiatePrefabExplicit(prefab, extraArgMap, context, false);
        }

        GameObject IInstantiator.InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive)
        {
            return Instantiator.InstantiatePrefabExplicit(prefab, extraArgMap, context, includeInactive, null);
        }

        GameObject IInstantiator.InstantiatePrefabExplicit(
            GameObject prefab, IEnumerable<object> extraArgMap, InjectContext context, bool includeInactive, string groupName)
        {
            var gameObj = (GameObject)GameObject.Instantiate(prefab);

            gameObj.transform.SetParent(GetTransformGroup(groupName), false);

            gameObj.SetActive(true);

            Resolver.InjectGameObject(gameObj, true, includeInactive, extraArgMap, context);

            return gameObj;
        }

        // Create a new empty game object under the default parent
        GameObject IInstantiator.InstantiateGameObject(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.SetParent(DefaultParent, false);
            return gameObj;
        }

        object IInstantiator.InstantiateComponentOnNewGameObjectExplicit(
            Type componentType, string name, List<TypeValuePair> extraArgMap, InjectContext currentContext)
        {
            Assert.That(componentType.DerivesFrom<Component>(), "Expected type '{0}' to derive from UnityEngine.Component", componentType.Name());

            var gameObj = Instantiator.InstantiateGameObject(name);

            if (componentType == typeof(Transform))
            {
                Assert.That(extraArgMap.IsEmpty());
                return gameObj.transform;
            }

            var component = (Component)gameObj.AddComponent(componentType);

            Resolver.InjectExplicit(component, extraArgMap, currentContext);

            return component;
        }

        object IInstantiator.InstantiatePrefabResourceForComponentExplicit(
            Type componentType, string resourcePath, List<TypeValuePair> extraArgs, InjectContext currentContext)
        {
            var prefab = (GameObject)Resources.Load(resourcePath);
            Assert.IsNotNull(prefab, "Could not find prefab at resource location '{0}'".Fmt(resourcePath));
            return Instantiator.InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext);
        }

        object IInstantiator.InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext)
        {
            return Instantiator.InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext, false);
        }

        object IInstantiator.InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext, bool includeInactive)
        {
            return Instantiator.InstantiatePrefabForComponentExplicit(
                componentType, prefab, extraArgs, currentContext, includeInactive, null);
        }

        object IInstantiator.InstantiatePrefabForComponentExplicit(
            Type componentType, GameObject prefab, List<TypeValuePair> extraArgs,
            InjectContext currentContext, bool includeInactive, string groupName)
        {
            Assert.That(prefab != null, "Null prefab found when instantiating game object");

            Assert.That(componentType.IsInterface || componentType.DerivesFrom<Component>(),
                "Expected type '{0}' to derive from UnityEngine.Component", componentType.Name());

            var gameObj = (GameObject)GameObject.Instantiate(prefab);

            try
            {
                gameObj.transform.SetParent(GetTransformGroup(groupName), false);

                gameObj.SetActive(true);

                Component requestedScript = null;

                // Inject on the children first since the parent objects are more likely to use them in their post inject methods
                foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(gameObj, true, includeInactive).ToList())
                {
                    if (component == null)
                    {
                        Log.Warn("Found null component while instantiating prefab '{0}'.  Possible missing script.", prefab.name);
                        continue;
                    }

                    if (component.GetType().DerivesFromOrEqual(componentType))
                    {
                        Assert.IsNull(requestedScript,
                            "Found multiple matches with type '{0}' when instantiating new game object from prefab '{1}'", componentType, prefab.name);
                        requestedScript = component;

                        Resolver.InjectExplicit(component, extraArgs);
                    }
                    else
                    {
                        Resolver.Inject(component);
                    }
                }

                if (requestedScript == null)
                {
                    throw new ZenjectResolveException(
                        "Could not find component with type '{0}' when instantiating new game object".Fmt(componentType));
                }

                return requestedScript;
            }
            catch (Exception e)
            {
                // If we do get exceptions don't leave half-initialized objects around
                GameObject.DestroyImmediate(gameObj);

                throw new Exception(
                    "Error while instantiating prefab '{0}'".Fmt(prefab.name), e);
            }
        }

        Transform GetTransformGroup(string groupName)
        {
            if (DefaultParent == null)
            {
                if (groupName == null)
                {
                    return null;
                }

                return (GameObject.Find("/" + groupName) ?? new GameObject(groupName)).transform;
            }

            if (groupName == null)
            {
                return DefaultParent;
            }

            foreach (Transform child in DefaultParent)
            {
                if (child.name == groupName)
                {
                    return child;
                }
            }

            var group = new GameObject(groupName).transform;
            group.SetParent(DefaultParent, false);
            return group;
        }

#endif

        ////////////// Convenience methods for IInstantiator ////////////////

        T IInstantiator.Instantiate<T>(
            params object[] extraArgs)
        {
            return (T)Instantiator.Instantiate(typeof(T), extraArgs);
        }

        object IInstantiator.Instantiate(
            Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return Instantiator.InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        T IInstantiator.InstantiateExplicit<T>(
            List<TypeValuePair> extraArgMap)
        {
            return (T)Instantiator.InstantiateExplicit(typeof(T), extraArgMap);
        }

        T IInstantiator.InstantiateExplicit<T>(
            List<TypeValuePair> extraArgMap, InjectContext context)
        {
            return (T)Instantiator.InstantiateExplicit(
                typeof(T), extraArgMap, context);
        }

        object IInstantiator.InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap)
        {
            return Instantiator.InstantiateExplicit(
                concreteType, extraArgMap, new InjectContext(this, concreteType, null));
        }

        object IInstantiator.InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap, InjectContext context)
        {
            return Instantiator.InstantiateExplicit(
                concreteType, extraArgMap, context, null, true);
        }

#if !ZEN_NOT_UNITY3D
        TContract IInstantiator.InstantiateComponent<TContract>(
            GameObject gameObject, params object[] args)
        {
            return (TContract)Instantiator.InstantiateComponent(typeof(TContract), gameObject, args);
        }

        GameObject IInstantiator.InstantiatePrefab(
            GameObject prefab, params object[] args)
        {
            return Instantiator.InstantiatePrefabExplicit(prefab, args, null);
        }

        GameObject IInstantiator.InstantiatePrefab(
            bool includeInactive, GameObject prefab, params object[] args)
        {
            return Instantiator.InstantiatePrefabExplicit(prefab, args, null, includeInactive);
        }

        GameObject IInstantiator.InstantiatePrefabResource(
            string resourcePath, params object[] args)
        {
            return Instantiator.InstantiatePrefabResourceExplicit(resourcePath, args, null, false);
        }

        GameObject IInstantiator.InstantiatePrefabResource(
            bool includeInactive, string resourcePath, params object[] args)
        {
            return Instantiator.InstantiatePrefabResourceExplicit(resourcePath, args, null, includeInactive);
        }

        /////////////// InstantiatePrefabForComponent

        T IInstantiator.InstantiatePrefabForComponent<T>(
            GameObject prefab, params object[] extraArgs)
        {
            return (T)Instantiator.InstantiatePrefabForComponent(typeof(T), prefab, extraArgs);
        }

        object IInstantiator.InstantiatePrefabForComponent(
            Type concreteType, GameObject prefab, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return Instantiator.InstantiatePrefabForComponentExplicit(
                concreteType, prefab, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        T IInstantiator.InstantiatePrefabForComponent<T>(
            bool includeInactive, GameObject prefab, params object[] extraArgs)
        {
            return (T)Instantiator.InstantiatePrefabForComponent(includeInactive, typeof(T), prefab, extraArgs);
        }

        object IInstantiator.InstantiatePrefabForComponent(
            bool includeInactive, Type concreteType, GameObject prefab, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
            "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return Instantiator.InstantiatePrefabForComponentExplicit(
                concreteType, prefab,
                InstantiateUtil.CreateTypeValueList(extraArgs),
                new InjectContext(this, concreteType, null), includeInactive);
        }

        // This is used instead of Instantiate to support specifying null values
        T IInstantiator.InstantiatePrefabForComponentExplicit<T>(
            GameObject prefab, List<TypeValuePair> extraArgMap)
        {
            return (T)Instantiator.InstantiatePrefabForComponentExplicit(typeof(T), prefab, extraArgMap);
        }

        object IInstantiator.InstantiatePrefabForComponentExplicit(
            Type concreteType, GameObject prefab, List<TypeValuePair> extraArgMap)
        {
            return Instantiator.InstantiatePrefabForComponentExplicit(
                concreteType, prefab, extraArgMap, new InjectContext(this, concreteType, null));
        }


        /////////////// InstantiatePrefabForComponent

        T IInstantiator.InstantiatePrefabResourceForComponent<T>(
            string resourcePath, params object[] extraArgs)
        {
            return (T)Instantiator.InstantiatePrefabResourceForComponent(typeof(T), resourcePath, extraArgs);
        }

        object IInstantiator.InstantiatePrefabResourceForComponent(
            Type concreteType, string resourcePath, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
            "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiatePrefabForComponentExplicit", concreteType);

            return Instantiator.InstantiatePrefabResourceForComponentExplicit(
                concreteType, resourcePath, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        T IInstantiator.InstantiatePrefabResourceForComponentExplicit<T>(
            string resourcePath, List<TypeValuePair> extraArgMap)
        {
            return (T)Instantiator.InstantiatePrefabResourceForComponentExplicit(typeof(T), resourcePath, extraArgMap);
        }

        object IInstantiator.InstantiatePrefabResourceForComponentExplicit(
            Type concreteType, string resourcePath, List<TypeValuePair> extraArgMap)
        {
            return Instantiator.InstantiatePrefabResourceForComponentExplicit(
                concreteType, resourcePath, extraArgMap, new InjectContext(this, concreteType, null));
        }

        /////////////// InstantiateComponentOnNewGameObject

        T IInstantiator.InstantiateComponentOnNewGameObject<T>(
            string name, params object[] extraArgs)
        {
            return (T)Instantiator.InstantiateComponentOnNewGameObject(typeof(T), name, extraArgs);
        }

        object IInstantiator.InstantiateComponentOnNewGameObject(
            Type concreteType, string name, params object[] extraArgs)
        {
            Assert.That(!extraArgs.ContainsItem(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateComponentOnNewGameObjectExplicit", concreteType);

            return Instantiator.InstantiateComponentOnNewGameObjectExplicit(
                concreteType, name, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        T IInstantiator.InstantiateComponentOnNewGameObjectExplicit<T>(
            string name, List<TypeValuePair> extraArgMap)
        {
            return (T)Instantiator.InstantiateComponentOnNewGameObjectExplicit(typeof(T), name, extraArgMap);
        }

        object IInstantiator.InstantiateComponentOnNewGameObjectExplicit(
            Type concreteType, string name, List<TypeValuePair> extraArgMap)
        {
            return Instantiator.InstantiateComponentOnNewGameObjectExplicit(
                concreteType, name, extraArgMap, new InjectContext(this, concreteType, null));
        }
#endif

        ////////////// Convenience methods for IResolver ////////////////

#if !ZEN_NOT_UNITY3D
        // Inject dependencies into child game objects
        void IResolver.InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive)
        {
            Resolver.InjectGameObject(gameObject, recursive, includeInactive, Enumerable.Empty<object>());
        }

        void IResolver.InjectGameObject(
            GameObject gameObject, bool recursive)
        {
            Resolver.InjectGameObject(gameObject, recursive, false);
        }

        void IResolver.InjectGameObject(
            GameObject gameObject)
        {
            Resolver.InjectGameObject(gameObject, true, false);
        }

        void IResolver.InjectGameObject(
            GameObject gameObject,
            bool recursive, bool includeInactive, IEnumerable<object> extraArgs)
        {
            Resolver.InjectGameObject(
                gameObject, recursive, includeInactive, extraArgs, null);
        }

        void IResolver.InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive,
            IEnumerable<object> extraArgs, InjectContext context)
        {
            // Inject on the children first since the parent objects are more likely to use them in their post inject methods
            foreach (var component in ZenUtilInternal.GetInjectableComponentsBottomUp(gameObject, recursive, includeInactive).ToList())
            {
                if (component == null)
                {
                    Log.Warn("Found null component while injecting game object '{0}'.  Possible missing script.", gameObject.name);
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                Resolver.Inject(component, extraArgs, false, context);
            }
        }
#endif

        void IResolver.Inject(object injectable)
        {
            Resolver.Inject(injectable, Enumerable.Empty<object>());
        }

        void IResolver.Inject(object injectable, IEnumerable<object> additional)
        {
            Resolver.Inject(injectable, additional, true);
        }

        void IResolver.Inject(object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            Resolver.Inject(
                injectable, additional, shouldUseAll, new InjectContext(this, injectable.GetType(), null));
        }

        void IResolver.Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll, InjectContext context)
        {
            Resolver.Inject(
                injectable, additional, shouldUseAll, context, TypeAnalyzer.GetInfo(injectable.GetType()));
        }

        void IResolver.Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll,
            InjectContext context, ZenjectTypeInfo typeInfo)
        {
            Assert.That(!additional.ContainsItem(null),
                "Null value given to injection argument list. In order to use null you must provide a List<TypeValuePair> and not just a list of objects");

            Resolver.InjectExplicit(
                injectable, InstantiateUtil.CreateTypeValueList(additional),
                shouldUseAll, typeInfo, context, null);
        }

        void IResolver.InjectExplicit(object injectable, List<TypeValuePair> additional)
        {
            Resolver.InjectExplicit(
                injectable, additional, new InjectContext(this, injectable.GetType(), null));
        }

        void IResolver.InjectExplicit(object injectable, List<TypeValuePair> additional, InjectContext context)
        {
            Resolver.InjectExplicit(
                injectable, additional, true,
                TypeAnalyzer.GetInfo(injectable.GetType()), context, null);
        }

        List<Type> IResolver.ResolveTypeAll(Type type)
        {
            return Resolver.ResolveTypeAll(new InjectContext(this, type, null));
        }

        TContract IResolver.Resolve<TContract>()
        {
            return Resolver.Resolve<TContract>((string)null);
        }

        TContract IResolver.Resolve<TContract>(string identifier)
        {
            return Resolver.Resolve<TContract>(new InjectContext(this, typeof(TContract), identifier));
        }

        TContract IResolver.TryResolve<TContract>()
        {
            return Resolver.TryResolve<TContract>((string)null);
        }

        TContract IResolver.TryResolve<TContract>(string identifier)
        {
            return (TContract)Resolver.TryResolve(typeof(TContract), identifier);
        }

        object IResolver.TryResolve(Type contractType)
        {
            return Resolver.TryResolve(contractType, null);
        }

        object IResolver.TryResolve(Type contractType, string identifier)
        {
            return Resolver.Resolve(new InjectContext(this, contractType, identifier, true));
        }

        object IResolver.Resolve(Type contractType)
        {
            return Resolver.Resolve(new InjectContext(this, contractType, null));
        }

        object IResolver.Resolve(Type contractType, string identifier)
        {
            return Resolver.Resolve(new InjectContext(this, contractType, identifier));
        }

        TContract IResolver.Resolve<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (TContract) Resolver.Resolve(context);
        }

        List<TContract> IResolver.ResolveAll<TContract>()
        {
            return Resolver.ResolveAll<TContract>((string)null);
        }

        List<TContract> IResolver.ResolveAll<TContract>(bool optional)
        {
            return Resolver.ResolveAll<TContract>(null, optional);
        }

        List<TContract> IResolver.ResolveAll<TContract>(string identifier)
        {
            return Resolver.ResolveAll<TContract>(identifier, false);
        }

        List<TContract> IResolver.ResolveAll<TContract>(string identifier, bool optional)
        {
            var context = new InjectContext(this, typeof(TContract), identifier, optional);
            return Resolver.ResolveAll<TContract>(context);
        }

        List<TContract> IResolver.ResolveAll<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (List<TContract>) Resolver.ResolveAll(context);
        }

        IList IResolver.ResolveAll(Type contractType)
        {
            return Resolver.ResolveAll(contractType, null);
        }

        IList IResolver.ResolveAll(Type contractType, string identifier)
        {
            return Resolver.ResolveAll(contractType, identifier, false);
        }

        IList IResolver.ResolveAll(Type contractType, bool optional)
        {
            return Resolver.ResolveAll(contractType, null, optional);
        }

        IList IResolver.ResolveAll(Type contractType, string identifier, bool optional)
        {
            var context = new InjectContext(this, contractType, identifier, optional);
            return Resolver.ResolveAll(context);
        }

        ////////////// IBinder ////////////////

        void IBinder.UnbindAll()
        {
            foreach (var provider in _providers.Values.SelectMany(x => x))
            {
                provider.Dispose();
            }

            _providers.Clear();
        }

        bool IBinder.Unbind<TContract>()
        {
            return Binder.Unbind<TContract>(null);
        }

        bool IBinder.Unbind<TContract>(string identifier)
        {
            List<ProviderBase> providersToRemove;
            var bindingId = new BindingId(typeof(TContract), identifier);

            if (_providers.TryGetValue(bindingId, out providersToRemove))
            {
                _providers.Remove(bindingId);

                // Only dispose if the provider is not bound to another type
                foreach (var provider in providersToRemove)
                {
                    if (_providers.Where(x => x.Value.ContainsItem(provider)).IsEmpty())
                    {
                        provider.Dispose();
                    }
                }

                return true;
            }

            return false;
        }

        BindingConditionSetter IBinder.BindInstance<TContract>(string identifier, TContract obj)
        {
            return Binder.Bind<TContract>(identifier).ToInstance(obj);
        }

        BindingConditionSetter IBinder.BindInstance<TContract>(TContract obj)
        {
            return Binder.Bind<TContract>().ToInstance(obj);
        }

        IGenericBinder<TContract> IBinder.Bind<TContract>()
        {
            return Binder.Bind<TContract>(null);
        }

        IUntypedBinder IBinder.Bind(Type contractType)
        {
            return Binder.Bind(contractType, null);
        }

        bool IBinder.HasBinding(InjectContext context)
        {
            List<ProviderBase> providers;

            if (!_providers.TryGetValue(context.BindingId, out providers))
            {
                return false;
            }

            return providers.Where(x => x.Matches(context)).HasAtLeast(1);
        }

        bool IBinder.HasBinding<TContract>()
        {
            return Binder.HasBinding<TContract>(null);
        }

        bool IBinder.HasBinding<TContract>(string identifier)
        {
            return Binder.HasBinding(
                new InjectContext(this, typeof(TContract), identifier));
        }

        void IBinder.BindAllInterfacesToSingleFacadeMethod<TConcrete>(Action<IBinder> installerMethod)
        {
            Binder.BindAllInterfacesToSingleFacadeMethod(typeof(TConcrete), installerMethod);
        }

        void IBinder.BindAllInterfacesToSingleFacadeMethod(Type concreteType, Action<IBinder> installerMethod)
        {
            Binder.BindAllInterfacesToSingleFacadeMethod(concreteType, null, installerMethod);
        }

        void IBinder.BindAllInterfacesToSingleFacadeMethod(
            Type concreteType, string concreteIdentifier, Action<IBinder> installerMethod)
        {
            foreach (var interfaceType in concreteType.GetInterfaces())
            {
                Assert.That(concreteType.DerivesFrom(interfaceType));

                Binder.Bind(interfaceType).ToSingleFacadeMethod(concreteType, concreteIdentifier, installerMethod);
            }
        }

        void IBinder.BindAllInterfacesToSingle<TConcrete>()
        {
            Binder.BindAllInterfacesToSingle(typeof(TConcrete));
        }

        void IBinder.BindAllInterfacesToSingle(Type concreteType)
        {
            Binder.BindAllInterfacesToSingle(concreteType, null);
        }

        void IBinder.BindAllInterfacesToSingle(Type concreteType, string concreteIdentifier)
        {
            foreach (var interfaceType in concreteType.GetInterfaces())
            {
                Assert.That(concreteType.DerivesFrom(interfaceType));
                Binder.Bind(interfaceType).ToSingle(concreteType, concreteIdentifier);
            }
        }

        void IBinder.BindAllInterfacesToInstance(object value)
        {
            Binder.BindAllInterfacesToInstance(value.GetType(), value);
        }

        void IBinder.BindAllInterfacesToInstance(Type concreteType, object value)
        {
            Assert.That((value == null && _isValidating) || value.GetType().DerivesFromOrEqual(concreteType));

            foreach (var interfaceType in concreteType.GetInterfaces())
            {
                Assert.That(concreteType.DerivesFrom(interfaceType));
                Binder.Bind(interfaceType).ToInstance(concreteType, value);
            }
        }

        IFactoryUntypedBinder<TContract> IBinder.BindIFactoryUntyped<TContract>(string identifier)
        {
            return new IFactoryUntypedBinder<TContract>(this, identifier);
        }

        IFactoryUntypedBinder<TContract> IBinder.BindIFactoryUntyped<TContract>()
        {
            return Binder.BindIFactoryUntyped<TContract>(null);
        }

        IIFactoryBinder<TContract> IBinder.BindIFactory<TContract>(string identifier)
        {
            return new IFactoryBinder<TContract>(this, identifier);
        }

        IIFactoryBinder<TContract> IBinder.BindIFactory<TContract>()
        {
            return Binder.BindIFactory<TContract>(null);
        }

        IIFactoryBinder<TParam1, TContract> IBinder.BindIFactory<TParam1, TContract>(string identifier)
        {
            return new IFactoryBinder<TParam1, TContract>(this, identifier);
        }

        IIFactoryBinder<TParam1, TContract> IBinder.BindIFactory<TParam1, TContract>()
        {
            return Binder.BindIFactory<TParam1, TContract>(null);
        }

        IIFactoryBinder<TParam1, TParam2, TContract> IBinder.BindIFactory<TParam1, TParam2, TContract>(string identifier)
        {
            return new IFactoryBinder<TParam1, TParam2, TContract>(this, identifier);
        }

        IIFactoryBinder<TParam1, TParam2, TContract> IBinder.BindIFactory<TParam1, TParam2, TContract>()
        {
            return Binder.BindIFactory<TParam1, TParam2, TContract>(null);
        }

        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> IBinder.BindIFactory<TParam1, TParam2, TParam3, TContract>(string identifier)
        {
            return new IFactoryBinder<TParam1, TParam2, TParam3, TContract>(this, identifier);
        }

        IIFactoryBinder<TParam1, TParam2, TParam3, TContract> IBinder.BindIFactory<TParam1, TParam2, TParam3, TContract>()
        {
            return Binder.BindIFactory<TParam1, TParam2, TParam3, TContract>(null);
        }

        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> IBinder.BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(string identifier)
        {
            return new IFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract>(this, identifier);
        }

        IIFactoryBinder<TParam1, TParam2, TParam3, TParam4, TContract> IBinder.BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>()
        {
            return Binder.BindIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(null);
        }

        IGenericBinder<TContract> IBinder.Rebind<TContract>()
        {
            Binder.Unbind<TContract>();
            return Binder.Bind<TContract>();
        }

        IGenericBinder<TContract> IBinder.Bind<TContract>(string identifier)
        {
            Assert.That(!typeof(TContract).DerivesFromOrEqual<IInstaller>(),
                "Deprecated usage of Bind<IInstaller>, use Install<IInstaller> instead");
            return new GenericBinder<TContract>(this, identifier);
        }

        // Note that this can include open generic types as well such as List<>
        IUntypedBinder IBinder.Bind(Type contractType, string identifier)
        {
            Assert.That(!contractType.DerivesFromOrEqual<IInstaller>(),
                "Deprecated usage of Bind<IInstaller>, use Install<IInstaller> instead");
            return new UntypedBinder(this, contractType, identifier);
        }

#if !ZEN_NOT_UNITY3D

        BindingConditionSetter IBinder.BindMonoFacadeFactory<TFactory>(
            GameObject prefab)
        {
            return Binder.BindMonoFacadeFactory<TFactory>(prefab, null);
        }

        BindingConditionSetter IBinder.BindMonoFacadeFactory<TFactory>(
            GameObject prefab, string groupName)
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindMonoFacadeFactory for type '{0}'".Fmt(typeof(TFactory).Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Binder.Bind<TFactory>().ToMethod((ctx) => ctx.Instantiator.InstantiateExplicit<TFactory>(
                new List<TypeValuePair>() { InstantiateUtil.CreateTypePair(prefab), InstantiateUtil.CreateTypePair(groupName) }));
        }

        BindingConditionSetter IBinder.BindGameObjectFactory<TFactory>(GameObject prefab)
        {
            return Binder.BindGameObjectFactory<TFactory>(prefab, null);
        }

        BindingConditionSetter IBinder.BindGameObjectFactory<TFactory>(
            GameObject prefab, string groupName)
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindGameObjectFactory for type '{0}'".Fmt(typeof(TFactory).Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Binder.Bind<TFactory>().ToMethod((ctx) => ctx.Instantiator.InstantiateExplicit<TFactory>(
                new List<TypeValuePair>() { InstantiateUtil.CreateTypePair(prefab), InstantiateUtil.CreateTypePair(groupName) }));
        }
#endif

        BindingConditionSetter IBinder.BindFacadeFactoryMethod<TFacade, TFacadeFactory>(
            Action<IBinder> facadeInstaller)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(facadeInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryMethod<TParam1, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1> facadeInstaller)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(facadeInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryMethod<TParam1, TParam2, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1, TParam2> facadeInstaller)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(facadeInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryMethod<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(
            Action<IBinder, TParam1, TParam2, TParam3> facadeInstaller)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(facadeInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TFacade, TFacadeFactory, TInstaller>()
        {
            return Binder.BindFacadeFactoryInstaller<TFacade, TFacadeFactory>(typeof(TInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TFacade, TFacadeFactory, TInstaller>()
        {
            return Binder.BindFacadeFactoryInstaller<TParam1, TFacade, TFacadeFactory>(typeof(TInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TParam2, TFacade, TFacadeFactory, TInstaller>()
        {
            return Binder.BindFacadeFactoryInstaller<TParam1, TParam2, TFacade, TFacadeFactory>(typeof(TInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TParam2, TParam3, TFacade, TFacadeFactory, TInstaller>()
        {
            return Binder.BindFacadeFactoryInstaller<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(typeof(TInstaller));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TFacade, TFacadeFactory>(Type installerType)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(installerType));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TFacade, TFacadeFactory>(Type installerType)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(installerType));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TParam2, TFacade, TFacadeFactory>(Type installerType)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(installerType));
        }

        BindingConditionSetter IBinder.BindFacadeFactoryInstaller<TParam1, TParam2, TParam3, TFacade, TFacadeFactory>(Type installerType)
        {
            return Binder.Bind<TFacadeFactory>().ToMethod(
                x => x.Instantiator.Instantiate<TFacadeFactory>(installerType));
        }

        ////////////// Other ////////////////

        // Walk the object graph for the given type
        // Should never throw an exception - returns them instead
        // Note: If you just want to know whether a binding exists for the given TContract,
        // use HasBinding instead
        IEnumerable<ZenjectResolveException> IResolver.ValidateResolve(InjectContext context)
        {
            ProviderBase provider = null;
            var result = TryGetUniqueProvider(context, out provider);

            if (result == DiContainer.ProviderLookupResult.Success)
            {
                Assert.IsNotNull(provider);

                if (ChecksForCircularDependencies)
                {
                    var lookupId = new LookupId(provider, context.BindingId);

                    // Allow one before giving up so that you can do circular dependencies via postinject or fields
                    if (_resolvesInProgress.Where(x => x.Equals(lookupId)).Count() > 1)
                    {
                        yield return new ZenjectResolveException(
                            "Circular dependency detected! \nObject graph:\n {0}".Fmt(context.GetObjectGraphString()));
                    }

                    _resolvesInProgress.Push(lookupId);
                    try
                    {
                        foreach (var error in provider.ValidateBinding(context))
                        {
                            yield return error;
                        }
                    }
                    finally
                    {
                        Assert.That(_resolvesInProgress.Peek().Equals(lookupId));
                        _resolvesInProgress.Pop();
                    }
                }
                else
                {
                    foreach (var error in provider.ValidateBinding(context))
                    {
                        yield return error;
                    }
                }
            }
            else if (result == DiContainer.ProviderLookupResult.Multiple)
            {
                yield return new ZenjectResolveException(
                    "Found multiple matches when only one was expected for dependency with type '{0}'{1} \nObject graph:\n{2}"
                    .Fmt(
                        context.MemberType.Name(),
                        (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                        context.GetObjectGraphString()));
            }
            else
            {
                Assert.That(result == DiContainer.ProviderLookupResult.None);

                if (ReflectionUtil.IsGenericList(context.MemberType))
                {
                    var subType = context.MemberType.GetGenericArguments().Single();
                    var subContext = context.ChangeMemberType(subType);

                    var matches = GetAllProviderMatches(subContext);

                    if (matches.IsEmpty())
                    {
                        if (!context.Optional)
                        {
                            yield return new ZenjectResolveException(
                                "Could not find dependency with type 'List(0)'{1}.  If the empty list is also valid, you can allow this by using the [InjectOptional] attribute.' \nObject graph:\n{2}"
                                .Fmt(
                                    subContext.MemberType.Name(),
                                    (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                                    context.GetObjectGraphString()));
                        }
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            foreach (var error in match.ValidateBinding(context))
                            {
                                yield return error;
                            }
                        }
                    }
                }
                else
                {
                    if (!context.Optional)
                    {
                        yield return new ZenjectResolveException(
                            "Could not find required dependency with type '{0}'{1} \nObject graph:\n{2}"
                            .Fmt(
                                context.MemberType.Name(),
                                (context.ObjectType == null ? "" : " when injecting into '{0}'".Fmt(context.ObjectType.Name())),
                                context.GetObjectGraphString()));
                    }
                }
            }
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateObjectGraph<TConcrete>(
            params Type[] extras)
        {
            return Resolver.ValidateObjectGraph(typeof(TConcrete), extras);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateObjectGraph<TConcrete>(InjectContext context, params Type[] extras)
        {
            return Resolver.ValidateObjectGraph(typeof(TConcrete), context, extras);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateObjectGraph(
            Type contractType, params Type[] extras)
        {
            return Resolver.ValidateObjectGraph(
                contractType, new InjectContext(this, contractType), extras);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateObjectGraph(
            Type contractType, InjectContext context, params Type[] extras)
        {
            return Resolver.ValidateObjectGraph(
                contractType, context, null, extras);
        }

        IEnumerable<ZenjectResolveException> IResolver.ValidateObjectGraph(
            Type concreteType, InjectContext currentContext, string concreteIdentifier, params Type[] extras)
        {
            if (concreteType.IsAbstract)
            {
                throw new ZenjectResolveException(
                    "Expected contract type '{0}' to be non-abstract".Fmt(concreteType.Name()));
            }

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);
            var extrasList = extras.ToList();

            foreach (var dependInfo in typeInfo.AllInjectables)
            {
                Assert.IsEqual(dependInfo.ObjectType, concreteType);

                if (TryTakingFromExtras(dependInfo.MemberType, extrasList))
                {
                    continue;
                }

                var context = dependInfo.CreateInjectContext(this, currentContext, null, concreteIdentifier);

                foreach (var error in Resolver.ValidateResolve(context))
                {
                    yield return error;
                }
            }

            if (!extrasList.IsEmpty())
            {
                yield return new ZenjectResolveException(
                    "Found unnecessary extra parameters passed when injecting into '{0}' with types '{1}'.  \nObject graph:\n{2}"
                    .Fmt(concreteType.Name(), String.Join(",", extrasList.Select(x => x.Name()).ToArray()), currentContext.GetObjectGraphString()));
            }
        }

        bool TryTakingFromExtras(Type contractType, List<Type> extrasList)
        {
            foreach (var extraType in extrasList)
            {
                if (extraType.DerivesFromOrEqual(contractType))
                {
                    var removed = extrasList.Remove(extraType);
                    Assert.That(removed);
                    return true;
                }
            }

            return false;
        }

        ////////////// Types ////////////////

        class ProviderPair
        {
            public readonly ProviderBase Provider;
            public readonly DiContainer Container;

            public ProviderPair(
                ProviderBase provider,
                DiContainer container)
            {
                Provider = provider;
                Container = container;
            }
        }

        public enum ProviderLookupResult
        {
            Success,
            Multiple,
            None
        }

        struct LookupId
        {
            public readonly ProviderBase Provider;
            public readonly BindingId BindingId;

            public LookupId(
                ProviderBase provider, BindingId bindingId)
            {
                Provider = provider;
                BindingId = bindingId;
            }
        }
    }
}

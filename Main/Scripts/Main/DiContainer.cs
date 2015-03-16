using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer
    {
        readonly Dictionary<BindingId, List<ProviderBase>> _providers = new Dictionary<BindingId, List<ProviderBase>>();
        readonly SingletonProviderMap _singletonMap;
        readonly PrefabSingletonProviderMap _prefabSingletonMap;

        static Stack<Type> _lookupsInProgress = new Stack<Type>();

        bool _allowNullBindings;

        ProviderBase _fallbackProvider;
        Instantiator _instantiator;

        readonly List<IInstaller> _installedInstallers = new List<IInstaller>();

        public DiContainer()
        {
            _singletonMap = new SingletonProviderMap(this);
            _prefabSingletonMap = new PrefabSingletonProviderMap(this);
            _instantiator = new Instantiator(this);

            Bind<DiContainer>().To(this);
            Bind<Instantiator>().To(_instantiator);
            Bind<SingletonProviderMap>().To(_singletonMap);
            Bind<PrefabSingletonProviderMap>().To(_prefabSingletonMap);
            Bind<SingletonInstanceHelper>().To(new SingletonInstanceHelper(_singletonMap));
        }

        public IEnumerable<IInstaller> InstalledInstallers
        {
            get
            {
                return _installedInstallers;
            }
        }

        // This can be used to handle the case where the given contract is not
        // found in any other providers, and the contract is not optional
        // For example, to automatically mock-out missing dependencies, you can
        // do this:
        // _container.FallbackProvider = new TransientMockProvider(_container);
        // It can also be used to create nested containers:
        // var nestedContainer = new DiContainer();
        // nestedContainer.FallbackProvider = new DiContainerProvider(mainContainer);
        public ProviderBase FallbackProvider
        {
            get
            {
                return _fallbackProvider;
            }
            set
            {
                _fallbackProvider = value;
            }
        }

        // This flag is used during validation
        // in which case we use nulls to indicate whether we have an instance or not
        // Should be set to false otherwise
        public bool AllowNullBindings
        {
            get
            {
                return _allowNullBindings;
            }
            set
            {
                _allowNullBindings = value;
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

        // This is the list of concrete types that are in the current object graph
        // Useful for error messages (and complex binding conditions)
        internal static Stack<Type> LookupsInProgress
        {
            get
            {
                return _lookupsInProgress;
            }
        }

        internal static string GetCurrentObjectGraph()
        {
            if (_lookupsInProgress.Count == 0)
            {
                return "";
            }

            return _lookupsInProgress.Select(t => t.Name()).Reverse().Aggregate((i, str) => i + "\n" + str);
        }

        public BindingConditionSetter BindGameObjectFactory<T>(GameObject prefab)
            // This would be useful but fails with VerificationException's in webplayer builds for some reason
            //where T : GameObjectFactory
            where T : class
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindGameObjectFactory for type '{0}'".Fmt(typeof(T).Name()));
            }

            // We could bind the factory ToSingle but doing it this way is better
            // since it allows us to have multiple game object factories that
            // use different prefabs and have them injected into different places
            return Bind<T>().ToMethod((c, ctx) => c.Instantiate<T>(prefab));
        }

        public BindingConditionSetter BindFactoryToMethodUntyped<TContract>(Func<DiContainer, object[], TContract> method)
        {
            return Bind<IFactoryUntyped<TContract>>().To(new FactoryMethodUntyped<TContract>(this, method));
        }

        public BindingConditionSetter BindFactoryToMethod<TContract>(Func<DiContainer, TContract> method)
        {
            return Bind<IFactory<TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TContract>>(method));
        }

        public BindingConditionSetter BindFactoryToMethod<TParam1, TContract>(Func<DiContainer, TParam1, TContract> method)
        {
            return Bind<IFactory<TParam1, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TContract>>(method));
        }

        public BindingConditionSetter BindFactoryToMethod<TParam1, TParam2, TContract>(Func<DiContainer, TParam1, TParam2, TContract> method)
        {
            return Bind<IFactory<TParam1, TParam2, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TParam2, TContract>>(method));
        }

        public BindingConditionSetter BindFactoryToMethod<TParam1, TParam2, TParam3, TContract>(Func<DiContainer, TParam1, TParam2, TParam3, TContract> method)
        {
            return Bind<IFactory<TParam1, TParam2, TParam3, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TParam2, TParam3, TContract>>(method));
        }

        public BindingConditionSetter BindFactory<TContract>()
        {
            return Bind<IFactory<TContract>>().ToSingle<Factory<TContract>>();
        }

        public BindingConditionSetter BindFactory<TParam1, TContract>()
        {
            return Bind<IFactory<TParam1, TContract>>().ToSingle<Factory<TParam1, TContract>>();
        }

        public BindingConditionSetter BindFactory<TParam1, TParam2, TContract>()
        {
            return Bind<IFactory<TParam1, TParam2, TContract>>().ToSingle<Factory<TParam1, TParam2, TContract>>();
        }

        public BindingConditionSetter BindFactory<TParam1, TParam2, TParam3, TContract>()
        {
            return Bind<IFactory<TParam1, TParam2, TParam3, TContract>>().ToSingle<Factory<TParam1, TParam2, TParam3, TContract>>();
        }

        // Bind IFactory<TContract> such that it creates instances of type TConcrete
        public BindingConditionSetter BindFactoryToFactory<TContract, TConcrete>()
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TContract>((c) => c.Resolve<IFactory<TConcrete>>().Create());
        }

        public BindingConditionSetter BindFactoryToFactory<TParam1, TContract, TConcrete>()
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TParam1, TContract>((c, param1) => c.Resolve<IFactory<TParam1, TConcrete>>().Create(param1));
        }

        public BindingConditionSetter BindFactoryToFactory<TParam1, TParam2, TContract, TConcrete>()
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TParam1, TParam2, TContract>((c, param1, param2) => c.Resolve<IFactory<TParam1, TParam2, TConcrete>>().Create(param1, param2));
        }

        public BindingConditionSetter BindFactoryToFactory<TParam1, TParam2, TParam3, TContract, TConcrete>()
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TParam1, TParam2, TParam3, TContract>((c, param1, param2, param3) => c.Resolve<IFactory<TParam1, TParam2, TParam3, TConcrete>>().Create(param1, param2, param3));
        }

        public BindingConditionSetter BindFactoryToCustomFactory<TContract, TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TContract>((c) => c.Resolve<TFactory>().Create());
        }

        // Bind IFactory<TContract> to the given factory
        public BindingConditionSetter BindFactoryToCustomFactory<TParam1, TContract, TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return BindFactoryToMethod<TParam1, TContract>((c, param1) => c.Resolve<TFactory>().Create(param1));
        }

        // This occurs so often that we might as well have a convenience method
        public BindingConditionSetter BindFactoryUntyped<TContract>()
        {
            if (typeof(TContract).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Error while binding factory for type '{0}'. Must use version of BindFactory which includes a reference to a prefab you wish to instantiate"
                    .Fmt(typeof(TContract).Name()));
            }

            return Bind<IFactoryUntyped<TContract>>().ToSingle<FactoryUntyped<TContract>>();
        }

        public BindingConditionSetter BindFactoryUntyped<TContract, TConcrete>() where TConcrete : TContract
        {
            if (typeof(TContract).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Error while binding factory for type '{0}'. Must use version of BindFactory which includes a reference to a prefab you wish to instantiate"
                    .Fmt(typeof(TConcrete).Name()));
            }

            return Bind<IFactoryUntyped<TContract>>().ToSingle<FactoryUntyped<TContract, TConcrete>>();
        }

        public BindingConditionSetter BindFactoryForPrefab<TContract>(GameObject prefab) where TContract : Component
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindFactoryForPrefab for type '{0}'".Fmt(typeof(TContract).Name()));
            }

            // We could use ToSingleMethod here but then we'd have issues when using .When() conditionals to inject
            // multiple factories in different places
            return Bind<IFactory<TContract>>()
                .ToMethod((container, ctx) => container.Instantiate<GameObjectFactory<TContract>>(prefab));
        }

        public void BindAllInterfacesToSingle<TConcrete>()
            where TConcrete : class
        {
            BindAllInterfacesToSingle(typeof(TConcrete));
        }

        public void BindAllInterfacesToSingle(Type concreteType)
        {
            foreach (var interfaceType in concreteType.GetInterfaces())
            {
                Assert.That(concreteType.DerivesFrom(interfaceType));
                Bind(interfaceType).ToSingle(concreteType);
            }
        }

        public ValueBinder<TContract> BindValue<TContract>() where TContract : struct
        {
            return BindValue<TContract>(null);
        }

        public ValueBinder<TContract> BindValue<TContract>(string identifier) where TContract : struct
        {
            return new ValueBinder<TContract>(this, identifier);
        }

        public ReferenceBinder<TContract> Rebind<TContract>() where TContract : class
        {
            Unbind<TContract>();
            return Bind<TContract>();
        }

        public ReferenceBinder<TContract> Bind<TContract>()
            where TContract : class
        {
            return Bind<TContract>(null);
        }

        public ReferenceBinder<TContract> Bind<TContract>(string identifier)
            where TContract : class
        {
            return new ReferenceBinder<TContract>(this, identifier, _singletonMap, _prefabSingletonMap);
        }

        // Note that this can include open generic types as well such as List<>
        public BinderUntyped Bind(Type contractType)
        {
            return Bind(contractType, null);
        }

        public BinderUntyped Bind(Type contractType, string identifier)
        {
            return new BinderUntyped(this, contractType, identifier, _singletonMap, _prefabSingletonMap);
        }

        public BindScope CreateScope()
        {
            return new BindScope(this, _singletonMap, _prefabSingletonMap);
        }

        // See comment in LookupInProgressAdder
        internal LookupInProgressAdder PushLookup(Type type)
        {
            return new LookupInProgressAdder(this, type);
        }

        public void RegisterProvider(
            ProviderBase provider, BindingId bindingId)
        {
            if (_providers.ContainsKey(bindingId))
            {
                // Prevent duplicate singleton bindings:
                if (_providers[bindingId].Find(item => ReferenceEquals(item, provider)) != null)
                {
                    throw new ZenjectException(
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

        public IEnumerable<ZenjectResolveException> ValidateValidatables(params Type[] ignoreTypes)
        {
            foreach (var pair in _providers)
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

                    foreach (var error in ValidateObjectGraph(type, providedArgs))
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

        // Walk the object graph for the given type
        // Throws ZenjectResolveException if there is a problem
        // Note: If you just want to know whether a binding exists for the given TContract,
        // use HasBinding instead
        // Returns all ZenjectResolveExceptions found
        public IEnumerable<ZenjectResolveException> ValidateResolve<TContract>()
        {
            return ValidateResolve(new InjectContext(this, typeof(TContract)));
        }

        public IEnumerable<ZenjectResolveException> ValidateResolve(InjectContext context)
        {
            return BindingValidator.ValidateContract(this, context);
        }

        public IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(params Type[] extras)
        {
            return ValidateObjectGraph(typeof(TConcrete), extras);
        }

        public IEnumerable<ZenjectResolveException> ValidateObjectGraphsForTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                foreach (var error in ValidateObjectGraph(type))
                {
                    yield return error;
                }
            }
        }

        public IEnumerable<ZenjectResolveException> ValidateObjectGraph(Type contractType, params Type[] extras)
        {
            if (contractType.IsAbstract)
            {
                throw new ZenjectResolveException(
                    "Expected contract type '{0}' to be non-abstract".Fmt(contractType.Name()));
            }

            return BindingValidator.ValidateObjectGraph(this, contractType, extras);
        }

        // Wrap IEnumerable<> to avoid LINQ mistakes
        internal List<ProviderBase> GetProviderMatches(InjectContext context)
        {
            return GetProviderMatchesInternal(context).ToList();
        }

        // Be careful with this method since it is a coroutine
        IEnumerable<ProviderBase> GetProviderMatchesInternal(InjectContext context)
        {
            return GetProvidersForContract(context.BindingId).Where(x => x.Matches(context));
        }

        internal IEnumerable<ProviderBase> GetProvidersForContract(BindingId bindingId)
        {
            List<ProviderBase> providers;

            if (_providers.TryGetValue(bindingId, out providers))
            {
                return providers;
            }

            // If we are asking for a List<int>, we should also match for any providers that are bound to the open generic type List<>
            // Currently it only matches one and not the other - not totally sure if this is better than returning both
            if (bindingId.Type.IsGenericType && _providers.TryGetValue(new BindingId(bindingId.Type.GetGenericTypeDefinition(), bindingId.Identifier), out providers))
            {
                return providers;
            }

            return Enumerable.Empty<ProviderBase>();
        }

        public bool HasBinding(InjectContext context)
        {
            List<ProviderBase> providers;

            if (!_providers.TryGetValue(context.BindingId, out providers))
            {
                return false;
            }

            return providers.Where(x => x.Matches(context)).HasAtLeast(1);
        }

        public List<TContract> ResolveMany<TContract>()
        {
            return ResolveMany<TContract>(false);
        }

        public List<TContract> ResolveMany<TContract>(bool optional)
        {
            var context = new InjectContext(this, typeof(TContract), null, optional);
            return ResolveMany<TContract>(context);
        }

        public List<TContract> ResolveMany<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (List<TContract>) ResolveMany(context);
        }

        public object ResolveMany(InjectContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var matches = GetProviderMatchesInternal(context).ToList();

            if (matches.Any())
            {
                return ReflectionUtil.CreateGenericList(
                    context.MemberType, matches.Select(x => x.GetInstance(context)).ToArray());
            }

            if (!context.Optional)
            {
                if (_fallbackProvider != null)
                {
                    var listType = typeof(List<>).MakeGenericType(context.MemberType);
                    var subContext = context.ChangeMemberType(listType);

                    return _fallbackProvider.GetInstance(subContext);
                }

                throw new ZenjectResolveException(
                    "Could not find required dependency with type '" + context.MemberType.Name() + "' \nObject graph:\n" + GetCurrentObjectGraph());
            }

            return ReflectionUtil.CreateGenericList(context.MemberType, new object[] {});
        }

        public List<Type> ResolveTypeMany(InjectContext context)
        {
            if (_providers.ContainsKey(context.BindingId))
            {
                return _providers[context.BindingId].Select(x => x.GetInstanceType()).Where(x => x != null).ToList();
            }

            return new List<Type> {};
        }

        // Installing installers works a bit differently than just Resolving all of
        // them and then calling InstallBindings() on each
        // This is because we want to allow installers to "include" other installers
        // And we also want earlier installers to be able to configure later installers
        // So we need to Resolve() one at a time, removing each installer binding
        // as we go
        public void InstallInstallers()
        {
            var injectCtx = new InjectContext(this, typeof(IInstaller), null);

            while (true)
            {
                var provider = GetProviderMatchesInternal(injectCtx).FirstOrDefault();

                if (provider == null)
                {
                    break;
                }

                var installer = (IInstaller)provider.GetInstance(injectCtx);

                Assert.IsNotNull(installer);

                UnregisterProvider(provider);

                if (_installedInstallers.Where(x => x.GetType() == installer.GetType()).Any())
                {
                    // Do not install the same installer twice
                    continue;
                }

                installer.InstallBindings();
                _installedInstallers.Add(installer);
            }
        }

        // Return single instance of requested type or assert
        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>((string)null);
        }

        public TContract Resolve<TContract>(string identifier)
        {
            return Resolve<TContract>(new InjectContext(this, typeof(TContract), identifier));
        }

        public object Resolve(Type contractType)
        {
            return Resolve(new InjectContext(this, contractType, null));
        }

        public object Resolve(Type contractType, string identifier)
        {
            return Resolve(new InjectContext(this, contractType, identifier));
        }

        public TContract Resolve<TContract>(InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (TContract) Resolve(context);
        }

        public object Resolve(InjectContext context)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var providers = GetProviderMatchesInternal(context).ToList();

            if (providers.IsEmpty())
            {
                // If it's a generic list then try matching multiple instances to its generic type
                if (ReflectionUtil.IsGenericList(context.MemberType))
                {
                    var subType = context.MemberType.GetGenericArguments().Single();
                    var subContext = context.ChangeMemberType(subType);

                    return ResolveMany(subContext);
                }

                if (!context.Optional)
                {
                    if (_fallbackProvider != null)
                    {
                        return _fallbackProvider.GetInstance(context);
                    }

                    throw new ZenjectResolveException(
                        "Unable to resolve type '{0}'{1}. \nObject graph:\n{2}"
                        .Fmt(
                            context.MemberType.Name() + (context.Identifier == null ? "" : " with ID '" + context.Identifier.ToString() + "'"),
                            (context.ParentType == null ? "" : " while building object with type '{0}'".Fmt(context.ParentType.Name())),
                            GetCurrentObjectGraph()));
                }

                return null;
            }
            else if (providers.Count > 1)
            {
                throw new ZenjectResolveException(
                    "Found multiple matches when only one was expected for type '{0}'{1}. \nObject graph:\n {2}"
                    .Fmt(
                        context.MemberType.Name(),
                        (context.ParentType == null ? "" : " while building object with type '{0}'".Fmt(context.ParentType.Name())),
                        GetCurrentObjectGraph()));
            }
            else
            {
                return providers.Single().GetInstance(context);
            }
        }

        public bool Unbind<TContract>()
        {
            return Unbind<TContract>(null);
        }

        public bool Unbind<TContract>(string identifier)
        {
            List<ProviderBase> providersToRemove;
            var bindingId = new BindingId(typeof(TContract), identifier);

            if (_providers.TryGetValue(bindingId, out providersToRemove))
            {
                _providers.Remove(bindingId);

                // Only dispose if the provider is not bound to another type
                foreach (var provider in providersToRemove)
                {
                    if (_providers.Where(x => x.Value.Contains(provider)).IsEmpty())
                    {
                        provider.Dispose();
                    }
                }

                return true;
            }

            return false;
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

        // Same as Instantiate except you can pas in null value
        // however the type for each parameter needs to be explicitly provided in this case
        public T InstantiateExplicit<T>(List<TypeValuePair> extraArgList)
        {
            return _instantiator.InstantiateExplicit<T>(extraArgList);
        }

        public object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgList)
        {
            return _instantiator.InstantiateExplicit(concreteType, extraArgList);
        }

        public T Instantiate<T>(params object[] constructorArgs)
        {
            return _instantiator.Instantiate<T>(constructorArgs);
        }

        public object Instantiate(
            Type concreteType, params object[] constructorArgs)
        {
            return _instantiator.Instantiate(concreteType, constructorArgs);
        }

        // Helper methods
        public bool HasBinding<TContract>()
        {
            return HasBinding<TContract>(null);
        }

        public bool HasBinding<TContract>(string identifier)
        {
            return HasBinding(
                new InjectContext(this, typeof(TContract), identifier));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Expose methods to configure object graph via Bind() methods
    // - Build object graphs via Resolve() method
    public class DiContainer
    {
        readonly Dictionary<Type, List<ProviderBase>> _providers = new Dictionary<Type, List<ProviderBase>>();

        readonly SingletonProviderMap _singletonMap;

        Stack<Type> _lookupsInProgress = new Stack<Type>();

        // This is the list of concrete types that are in the current object graph
        // Useful for error messages (and complex binding conditions)
        internal Stack<Type> LookupsInProgress
        {
            get
            {
                return _lookupsInProgress;
            }
        }

        public DiContainer()
        {
            _singletonMap = new SingletonProviderMap(this);
            Bind<DiContainer>().AsSingle(this);
        }

        internal string GetCurrentObjectGraph()
        {
            if (_lookupsInProgress.Count == 0)
            {
                return "";
            }
            return _lookupsInProgress.Select(t => t.GetPrettyName()).Reverse().Aggregate((i, str) => i + "\n" + str);
        }

        // This occurs so often that we might as well have a convenience method
        public BindingConditionSetter BindFactory<TContract>()
        {
            return Bind<IFactory<TContract>>().AsSingle<Factory<TContract>>();
        }

        public BindingConditionSetter BindFactory<TContract, TConcrete>() where TConcrete : TContract
        {
            return Bind<IFactory<TContract>>().AsSingle<Factory<TContract, TConcrete>>();
        }

        public ValueBinder<TContract> BindValue<TContract>() where TContract : struct
        {
            return new ValueBinder<TContract>(this, _singletonMap);
        }

        public ReferenceBinder<TContract> Bind<TContract>() where TContract : class
        {
            return new ReferenceBinder<TContract>(this, _singletonMap);
        }

        public CustomScope CreateScope()
        {
            return new CustomScope(this, _singletonMap);
        }

        // See comment in LookupInProgressAdder
        internal LookupInProgressAdder PushLookup(Type type)
        {
            return new LookupInProgressAdder(this, type);
        }

        public void RegisterProvider<TContract>(ProviderBase provider)
        {
            if (_providers.ContainsKey(typeof (TContract)))
            {
                // Prevent duplicate singleton bindings:
                Assert.That(_providers[typeof(TContract)].Find(item => ReferenceEquals(item, provider)) == null,
                "Found duplicate singleton binding for contract '" + typeof (TContract) + "'");

                _providers[typeof (TContract)].Add(provider);
            }
            else
            {
                _providers.Add(typeof (TContract), new List<ProviderBase> {provider});
            }
        }

        public void UnregisterProvider(ProviderBase provider)
        {
            int numRemoved = 0;

            foreach (var keyValue in _providers)
            {
                numRemoved += keyValue.Value.RemoveAll(x => x == provider);
            }

            Assert.That(numRemoved > 0, "Tried to unregister provider that was not registered");

            // Remove any empty contracts
            foreach (var contractType in _providers.Where(x => x.Value.IsEmpty()).Select(x => x.Key).ToList())
            {
                _providers.Remove(contractType);
            }

            provider.Dispose();
        }

        public List<TContract> ResolveMany<TContract>()
        {
            return ResolveMany<TContract>(new ResolveContext());
        }

        public List<TContract> ResolveMany<TContract>(ResolveContext context)
        {
            return (List<TContract>) ResolveMany(typeof (TContract), context);
        }

        public object ResolveMany(Type contract)
        {
            return ResolveMany(contract, new ResolveContext());
        }

        List<object> ResolveInternalList(Type contract, ResolveContext context)
        {
            var instances = new List<object>();

            List<ProviderBase> providers;
            if (_providers.TryGetValue(contract, out providers))
            {
                foreach (var provider in providers.Where(x => x.AppliesTo(context)))
                {
                    instances.Add(provider.GetInstance());
                }
            }

            return instances;
        }

        public bool HasBinding(Type contract)
        {
            return HasBinding(contract, new ResolveContext());
        }

        public bool HasBinding(Type contract, ResolveContext context)
        {
            List<ProviderBase> providers;

            if (!_providers.TryGetValue(contract, out providers))
            {
                return false;
            }

            return providers.Where(x => x.AppliesTo(context)).HasAtLeast(1);
        }

        public bool HasBinding<TContract>()
        {
            return HasBinding(typeof(TContract));
        }

        public object ResolveMany(Type contract, ResolveContext context)
        {
            return ResolveMany(contract, context, true);
        }

        public object ResolveMany(Type contract, ResolveContext context, bool optional)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            if (_providers.ContainsKey(contract))
            {
                return ReflectionUtil.CreateGenericList(contract, ResolveInternalList(contract, context).ToArray());
            }

            Assert.That(optional, () =>
                    "Could not find required dependency with type '" + contract.GetPrettyName() + "' \nObject graph:\n" + GetCurrentObjectGraph());

            // All many-dependencies are optional, return an empty list
            return ReflectionUtil.CreateGenericList(contract, new object[] {});
        }

        public List<Type> ResolveTypeMany(Type contract)
        {
            if (_providers.ContainsKey(contract))
            {
                // TODO: fix this to work with providers that have conditions
                var context = new ResolveContext();

                return (from provider in _providers[contract] where provider.AppliesTo(context) select provider.GetInstanceType()).ToList();
            }

            return new List<Type> {};
        }

        // Return single insance of requested type or assert
        public TContract Resolve<TContract>()
        {
            return Resolve<TContract>(new ResolveContext());
        }

        public TContract Resolve<TContract>(ResolveContext context)
        {
            return (TContract) Resolve(typeof (TContract), context);
        }

        public object Resolve(Type contract)
        {
            return Resolve(contract, new ResolveContext());
        }

        public object Resolve(Type contract, ResolveContext context)
        {
            return ResolveInternalSingle(contract, context, false);
        }

        object ResolveInternalSingle(Type contract, ResolveContext context, bool optional)
        {
            // Note that different types can map to the same provider (eg. a base type to a concrete class and a concrete class to itself)

            var objects = ResolveInternalList(contract, context);

            if (!objects.Any())
            {
                if (!optional)
                {
                    throw new ZenjectResolveException(
                        "Unable to resolve type '" + contract + "'. \nObject graph:\n" + GetCurrentObjectGraph());
                }

                return null;
            }

            if (objects.Count > 1)
            {
                if (!optional)
                {
                    throw new ZenjectResolveException(
                        "Found multiple matches when only one was expected for type '" + contract + "'. \nObject graph:\n" + GetCurrentObjectGraph());
                }

                return null;
            }

            return objects.First();
        }

        public List<Type> GetDependencyContracts<TContract>()
        {
            return GetDependencyContracts(typeof(TContract));
        }

        public List<Type> GetDependencyContracts(Type contract)
        {
            var dependencies = new List<Type>();

            foreach (var param in ZenUtil.GetConstructorDependencies(contract))
            {
                dependencies.Add(param.ParameterType);
            }

            foreach (var fieldInfo in ZenUtil.GetFieldDependencies(contract))
            {
                dependencies.Add(fieldInfo.FieldType);
            }

            return dependencies;
        }

        public Dictionary<Type, List<Type>> CalculateObjectGraph<TRoot>()
        {
            return CalculateObjectGraph(typeof(TRoot));
        }

        public Dictionary<Type, List<Type>> CalculateObjectGraph(Type rootContract)
        {
            var map = new Dictionary<Type, List<Type>>();
            var types = ResolveTypeMany(rootContract);
            Assert.IsEqual(types.Count, 1);
            var rootType = types[0];

            map.Add(rootType, new List<Type>());
            AddToObjectGraph(rootType, map);

            return map;
        }

        void AddToObjectGraph(Type type, Dictionary<Type, List<Type>> map)
        {
            var dependList = map[type];

            foreach (var contractType in GetDependencyContracts(type))
            {
                List<Type> dependTypes;

                if (contractType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    var subTypes = contractType.GetGenericArguments();
                    Assert.IsEqual(subTypes.Length, 1);

                    var subType = subTypes[0];
                    dependTypes = ResolveTypeMany(subType);
                }
                else
                {
                    dependTypes = ResolveTypeMany(contractType);
                    Assert.That(dependTypes.Count <= 1);
                }

                foreach (var dependType in dependTypes)
                {
                    dependList.Add(dependType);

                    if (!map.ContainsKey(dependType))
                    {
                        map.Add(dependType, new List<Type>());
                        AddToObjectGraph(dependType, map);
                    }
                }
            }
        }
    }
}

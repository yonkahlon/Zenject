using System;
using System.Collections.Generic;
using ModestTree;
using Zenject.Internal;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
using UnityEditor;
#endif

using System.Linq;

namespace Zenject
{
    public abstract class TypeBinder : BinderBase
    {
        public TypeBinder(
            DiContainer container, List<Type> contractTypes, string bindIdentifier)
            : base(container, contractTypes, bindIdentifier)
        {
        }

        public BindingConditionSetter ToTransient()
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new TransientProvider(contractType, Container)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToTransient(Type concreteType)
        {
#if !ZEN_NOT_UNITY3D
            AssertIsNotComponent(concreteType);
#endif
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new TransientProvider(concreteType, Container));
        }

        public BindingConditionSetter ToSingle()
        {
            return ToSingle((string)null);
        }

        public BindingConditionSetter ToSingle(string concreteIdentifier)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, contractType)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            return ToSingle(concreteType, null);
        }

        public BindingConditionSetter ToSingle(Type concreteType, string concreteIdentifier)
        {
            AssertIsDerivedFromContracts(concreteType);

#if !ZEN_NOT_UNITY3D
            AssertIsNotComponent(concreteType);
#endif
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, concreteType));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToSingleMonoBehaviour(GameObject gameObject)
        {
            return ToSingleMonoBehaviour((string)null, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, GameObject gameObject)
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(
                    contractType => Container.SingletonProviderCreator.CreateProviderFromMonoBehaviour(concreteIdentifier, contractType, gameObject)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToSingleMonoBehaviour(Type concreteType, GameObject gameObject)
        {
            return ToSingleMonoBehaviour(null, concreteType, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsComponent(concreteType);
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(Container.SingletonProviderCreator.CreateProviderFromMonoBehaviour(concreteIdentifier, concreteType, gameObject));
        }

        public BindingConditionSetter ToSinglePrefab(GameObject prefab)
        {
            return ToSinglePrefab((string)null, prefab);
        }

        public BindingConditionSetter ToSinglePrefab(string concreteIdentifier, GameObject prefab)
        {
            AssertContractsAreComponents();
            AssertIsValidPrefab(prefab);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromPrefab(concreteIdentifier, contractType, prefab)).Cast<ProviderBase>());
        }

        // Note that concreteType here could be an interface as well
        public BindingConditionSetter ToSinglePrefab(
            Type concreteType, string concreteIdentifier, GameObject prefab)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertContractsAreComponents();
            AssertIsValidPrefab(prefab);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromPrefab(concreteIdentifier, concreteType, prefab));
        }

        public BindingConditionSetter ToTransientPrefab(GameObject prefab)
        {
            AssertIsValidPrefab(prefab);
            AssertContractsAreComponents();

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProviderFromPrefab(contractType, prefab, Container)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsValidPrefab(prefab);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new GameObjectTransientProviderFromPrefab(concreteType, prefab, Container));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject()
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProvider(contractType)).Cast<ProviderBase>());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject(Type concreteType)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertContractsAreComponents();
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new GameObjectTransientProvider(concreteType));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject((string)null);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string concreteIdentifier)
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(
                    contractType => Container.SingletonProviderCreator.CreateProviderFromGameObject(contractType, concreteIdentifier)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToSingleGameObject(Type concreteType)
        {
            return ToSingleGameObject(concreteType, null);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(Type concreteType, string concreteIdentifier)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsComponent(concreteType);
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromGameObject(concreteType, concreteIdentifier));
        }

        public BindingConditionSetter ToTransientPrefabResource(string resourcePath)
        {
            AssertIsValidResourcePath(resourcePath);
            AssertContractsAreComponents();

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProviderFromPrefabResource(contractType, resourcePath)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath)
        {
            AssertIsValidResourcePath(resourcePath);
            AssertIsComponent(concreteType);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                new GameObjectTransientProviderFromPrefabResource(concreteType, resourcePath));
        }

        public BindingConditionSetter ToSinglePrefabResource(Type concreteType, string concreteIdentifier, string resourcePath)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertContractsAreComponents();
            AssertIsValidResourcePath(resourcePath);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromPrefabResource(concreteIdentifier, concreteType, resourcePath));
        }

        public BindingConditionSetter ToSinglePrefabResource(string resourcePath)
        {
            return ToSinglePrefabResource(null, resourcePath);
        }

        public BindingConditionSetter ToSinglePrefabResource(string concreteIdentifier, string resourcePath)
        {
            AssertIsValidResourcePath(resourcePath);
            AssertContractsAreComponents();

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromPrefabResource(concreteIdentifier, contractType, resourcePath)).Cast<ProviderBase>());
        }

#endif
        protected BindingConditionSetter ToSingleMethodBase<TConcrete>(string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            AssertIsDerivedFromContracts(typeof(TConcrete));

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromMethod(concreteIdentifier, method));
        }

        protected BindingConditionSetter ToSingleFactoryBase<TConcrete, TFactory>(string concreteIdentifier)
            where TFactory : IFactory<TConcrete>
        {
            AssertIsDerivedFromContracts(typeof(TConcrete));

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromFactory<TConcrete, TFactory>(concreteIdentifier));
        }

        protected BindingConditionSetter ToMethodBase<T>(Func<InjectContext, T> method)
        {
            AssertIsDerivedFromContracts(typeof(T));

            return RegisterSingleProvider(
                new MethodProvider<T>(method, Container));
        }

        protected BindingConditionSetter ToResolveBase<TConcrete>(string identifier)
        {
            return ToMethodBase<TConcrete>((ctx) => ctx.Container.Resolve<TConcrete>(
                new InjectContext(
                    ctx.Container, typeof(TConcrete), identifier,
                    false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType)));
        }

        protected BindingConditionSetter ToGetterBase<TObj, TResult>(string identifier, Func<TObj, TResult> method)
        {
            return ToMethodBase((ctx) => method(ctx.Container.Resolve<TObj>(
                new InjectContext(
                    ctx.Container, typeof(TObj), identifier,
                    false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType))));
        }

        public BindingConditionSetter ToInstance(Type concreteType, object instance)
        {
            if (ZenUtilInternal.IsNull(instance) && !Container.IsValidating)
            {
                throw new ZenjectBindException(
                    "Received null instance during Bind command with type '{0}'".Fmt(concreteType.Name()));
            }

            AssertIsDerivedFromContracts(concreteType);

            if (!ZenUtilInternal.IsNull(instance))
            {
                foreach (var contractType in ContractTypes)
                {
                    if (!instance.GetType().DerivesFromOrEqual(contractType))
                    {
                        throw new ZenjectBindException(
                            "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'"
                            .Fmt(concreteType.Name(), contractType.Name()));
                    }
                }
            }

            return RegisterSingleProvider(
                new InstanceProvider(concreteType, instance, Container));
        }

        protected BindingConditionSetter ToSingleInstance(Type concreteType, string concreteIdentifier, object instance)
        {
            AssertIsDerivedFromContracts(concreteType);

            if (ZenUtilInternal.IsNull(instance) && !Container.IsValidating)
            {
                throw new ZenjectBindException(
                    "Received null singleton instance during Bind command when binding type '{0}'".Fmt(concreteType.Name()));
            }

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromInstance(concreteIdentifier, concreteType, instance));
        }

        public BindingConditionSetter ToSingleFacadeMethod(Action<DiContainer> installerFunc)
        {
            return ToSingleFacadeMethod((string)null, installerFunc);
        }

        public BindingConditionSetter ToSingleFacadeMethod(
            string concreteIdentifier, Action<DiContainer> installerFunc)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromFacadeMethod(contractType, concreteIdentifier, installerFunc)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToSingleFacadeMethod(
            Type concreteType, string concreteIdentifier, Action<DiContainer> installerFunc)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromFacadeMethod(concreteType, concreteIdentifier, installerFunc));
        }

        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>()
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(typeof(TInstaller));
        }

        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            string concreteIdentifier)
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(concreteIdentifier, typeof(TInstaller));
        }

        public BindingConditionSetter ToSingleFacadeInstaller(Type installerType)
        {
            return ToSingleFacadeInstaller((string)null, installerType);
        }

        public BindingConditionSetter ToSingleFacadeInstaller(
            string concreteIdentifier, Type installerType)
        {
            if (!installerType.DerivesFrom<Installer>())
            {
                throw new ZenjectBindException(
                    "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromFacadeInstaller(
                    contractType, concreteIdentifier, installerType)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            Type concreteType, string concreteIdentifier)
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(concreteType, concreteIdentifier, typeof(TInstaller));
        }

        public BindingConditionSetter ToSingleFacadeInstaller(
            Type concreteType, string concreteIdentifier, Type installerType)
        {
            AssertIsDerivedFromContracts(concreteType);

            if (!installerType.DerivesFrom<Installer>())
            {
                throw new ZenjectBindException(
                    "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

#if !ZEN_NOT_UNITY3D
            AssertIsNotComponent(concreteType);
#endif

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromFacadeInstaller(
                    concreteType, concreteIdentifier, installerType));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToResource(string resourcePath)
        {
            AssertContractsDeriveFromUnityObject();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new ResourceProvider(contractType, resourcePath)).Cast<ProviderBase>());
        }

        public BindingConditionSetter ToResource(Type concreteType, string resourcePath)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertContractsDeriveFromUnityObject();

            return RegisterSingleProvider(new ResourceProvider(concreteType, resourcePath));
        }

        protected void AssertIsValidPrefab(GameObject prefab)
        {
            if (ZenUtilInternal.IsNull(prefab))
            {
                throw new ZenjectBindException(
                    "Received null prefab during bind command");
            }

#if UNITY_EDITOR
            // This won't execute in dll builds sadly
            if (PrefabUtility.GetPrefabType(prefab) != PrefabType.Prefab)
            {
                throw new ZenjectBindException(
                    "Expected prefab but found game object with name '{0}' during bind command".Fmt(prefab.name));
            }
#endif
        }

        protected void AssertIsNotComponent(Type type)
        {
            if (type.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to NOT derive from UnityEngine.Component"
                    .Fmt(type.Name()));
            }
        }

        protected void AssertContractsDeriveFromUnityObject()
        {
            foreach (var contractType in ContractTypes)
            {
                AssertDerivesFromUnityObject(contractType);
            }
        }

        void AssertDerivesFromUnityObject(Type type)
        {
            if (!type.DerivesFrom<UnityEngine.Object>())
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Object"
                    .Fmt(type.Name()));
            }
        }

        protected void AssertContractsAreNotComponents()
        {
            foreach (var contractType in ContractTypes)
            {
                AssertIsNotComponent(contractType);
            }
        }

        protected void AssertIsValidResourcePath(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                throw new ZenjectBindException(
                    "Null or empty resource path provided");
            }

            // We'd like to validate the path here but unfortunately there doesn't appear to be
            // a way to do this besides loading it
        }

        protected void AssertContractsAreComponents()
        {
            foreach (var contractType in ContractTypes)
            {
                AssertIsComponent(contractType);
            }
        }

        protected void AssertIsComponent(Type type)
        {
            if (!type.DerivesFrom(typeof(Component)) && !type.IsInterface)
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Component"
                    .Fmt(type.Name()));
            }
        }
#endif

        protected void AssertContractsAreNotAbstract()
        {
            foreach (var contractType in ContractTypes)
            {
                AssertIsNotAbstract(contractType);
            }
        }

        protected void AssertIsNotAbstract(Type type)
        {
            if (type.IsAbstract)
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to not be abstract.".Fmt(type));
            }
        }

        protected void AssertIsDerivedFromContracts(Type concreteType)
        {
            foreach (var contractType in ContractTypes)
            {
                if (!concreteType.DerivesFromOrEqual(contractType))
                {
                    throw new ZenjectBindException(
                        "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), contractType.Name()));
                }
            }
        }
    }
}

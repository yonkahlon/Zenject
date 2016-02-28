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
    public abstract class TypeBinder : BinderBase, ITypeBinder
    {
        public TypeBinder(
            DiContainer container, List<Type> contractTypes, string bindIdentifier)
            : base(container, contractTypes, bindIdentifier)
        {
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransient()
        {
            return ToTransient(ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransient(ContainerTypes containerType)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new TransientProvider(contractType, Container, containerType)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransient(Type concreteType)
        {
            return ToTransient(concreteType, ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransient(Type concreteType, ContainerTypes containerType)
        {
#if !ZEN_NOT_UNITY3D
            AssertIsNotComponent(concreteType);
#endif
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new TransientProvider(concreteType, Container, containerType));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingle()
        {
            return ToSingle((string)null);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingle(string concreteIdentifier)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, contractType)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingle(Type concreteType)
        {
            return ToSingle(concreteType, null);
        }

        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleMonoBehaviour(GameObject gameObject)
        {
            return ToSingleMonoBehaviour((string)null, gameObject);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, GameObject gameObject)
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(
                    contractType => Container.SingletonProviderCreator.CreateProviderFromMonoBehaviour(concreteIdentifier, contractType, gameObject)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleMonoBehaviour(Type concreteType, GameObject gameObject)
        {
            return ToSingleMonoBehaviour(null, concreteType, gameObject);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsComponent(concreteType);
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(Container.SingletonProviderCreator.CreateProviderFromMonoBehaviour(concreteIdentifier, concreteType, gameObject));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSinglePrefab(GameObject prefab)
        {
            return ToSinglePrefab((string)null, prefab);
        }

        // See comment in ITypeBinder.cs
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
        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefab(GameObject prefab)
        {
            return ToTransientPrefab(prefab, ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefab(GameObject prefab, ContainerTypes containerType)
        {
            AssertIsValidPrefab(prefab);
            AssertContractsAreComponents();

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProviderFromPrefab(contractType, prefab, Container, containerType)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab)
        {
            return ToTransientPrefab(concreteType, prefab, ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab, ContainerTypes containerType)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsValidPrefab(prefab);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new GameObjectTransientProviderFromPrefab(concreteType, prefab, Container, containerType));
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientGameObject()
        {
            return ToTransientGameObject(ContainerTypes.RuntimeContainer);
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientGameObject(ContainerTypes containerType)
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProvider(Container, contractType, containerType)).Cast<ProviderBase>());
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientGameObject(Type concreteType)
        {
            return ToTransientGameObject(concreteType, ContainerTypes.RuntimeContainer);
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientGameObject(Type concreteType, ContainerTypes containerType)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertContractsAreComponents();
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(new GameObjectTransientProvider(Container, concreteType, containerType));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject((string)null);
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleGameObject(string concreteIdentifier)
        {
            AssertContractsAreComponents();
            AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(
                    contractType => Container.SingletonProviderCreator.CreateProviderFromGameObject(contractType, concreteIdentifier)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleGameObject(Type concreteType)
        {
            return ToSingleGameObject(concreteType, null);
        }

        // Creates a new game object and adds the given type as a new component on it
        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleGameObject(Type concreteType, string concreteIdentifier)
        {
            AssertIsDerivedFromContracts(concreteType);
            AssertIsComponent(concreteType);
            AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromGameObject(concreteType, concreteIdentifier));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefabResource(string resourcePath)
        {
            return ToTransientPrefabResource(resourcePath, ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefabResource(string resourcePath, ContainerTypes containerType)
        {
            AssertIsValidResourcePath(resourcePath);
            AssertContractsAreComponents();

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertContractsAreNotAbstract();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new GameObjectTransientProviderFromPrefabResource(contractType, resourcePath, Container, containerType)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath)
        {
            return ToTransientPrefabResource(concreteType, resourcePath, ContainerTypes.RuntimeContainer);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath, ContainerTypes containerType)
        {
            AssertIsValidResourcePath(resourcePath);
            AssertIsComponent(concreteType);

            // This is fine since it's a lookup on the prefab so we don't instantiate
            //AssertIsNotAbstract(concreteType);

            return RegisterSingleProvider(
                new GameObjectTransientProviderFromPrefabResource(concreteType, resourcePath, Container, containerType));
        }

        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSinglePrefabResource(string resourcePath)
        {
            return ToSinglePrefabResource(null, resourcePath);
        }

        // See comment in ITypeBinder.cs
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

        protected BindingConditionSetter ToResolveBase<TConcrete>(string identifier, ContainerTypes containerType)
        {
            return ToMethodBase<TConcrete>((ctx) =>
                {
                    var container = (containerType == ContainerTypes.RuntimeContainer ? ctx.Container : Container);
                    return container.Resolve<TConcrete>(
                        new InjectContext(
                            container, typeof(TConcrete), identifier,
                            false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType));
                });
        }

        protected BindingConditionSetter ToGetterBase<TObj, TResult>(string identifier, Func<TObj, TResult> method, ContainerTypes containerType)
        {
            return ToMethodBase((ctx) =>
                {
                    var container = (containerType == ContainerTypes.RuntimeContainer ? ctx.Container : Container);
                    return method(container.Resolve<TObj>(
                        new InjectContext(
                            container, typeof(TObj), identifier,
                            false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType)));
                });
        }

        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeMethod(Action<DiContainer> installerFunc)
        {
            return ToSingleFacadeMethod((string)null, installerFunc);
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeMethod(
            string concreteIdentifier, Action<DiContainer> installerFunc)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => Container.SingletonProviderCreator.CreateProviderFromFacadeMethod(contractType, concreteIdentifier, installerFunc)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeMethod(
            Type concreteType, string concreteIdentifier, Action<DiContainer> installerFunc)
        {
#if !ZEN_NOT_UNITY3D
            AssertContractsAreNotComponents();
#endif

            return RegisterSingleProvider(
                Container.SingletonProviderCreator.CreateProviderFromFacadeMethod(concreteType, concreteIdentifier, installerFunc));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>()
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(typeof(TInstaller));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            string concreteIdentifier)
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(concreteIdentifier, typeof(TInstaller));
        }

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeInstaller(Type installerType)
        {
            return ToSingleFacadeInstaller((string)null, installerType);
        }

        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToSingleFacadeInstaller<TInstaller>(
            Type concreteType, string concreteIdentifier)
            where TInstaller : Installer
        {
            return ToSingleFacadeInstaller(concreteType, concreteIdentifier, typeof(TInstaller));
        }

        // See comment in ITypeBinder.cs
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

        // See comment in ITypeBinder.cs
        public BindingConditionSetter ToResource(string resourcePath)
        {
            AssertContractsDeriveFromUnityObject();

            return RegisterProvidersPerContract(
                ContractTypes.Select(contractType => new ResourceProvider(contractType, resourcePath)).Cast<ProviderBase>());
        }

        // See comment in ITypeBinder.cs
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

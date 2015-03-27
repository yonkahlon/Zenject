using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // Convenience methods for DiContainer
    public static class DiContainerExtensions
    {
#if !ZEN_NOT_UNITY3D
        public static BindingConditionSetter BindGameObjectFactory<T>(
            this DiContainer container, GameObject prefab)
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
            return container.Bind<T>().ToMethod((c, ctx) => c.Instantiate<T>(prefab));
        }

        public static BindingConditionSetter BindFactoryForPrefab<TContract>(
            this DiContainer container, GameObject prefab) where TContract : Component
        {
            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindFactoryForPrefab for type '{0}'".Fmt(typeof(TContract).Name()));
            }

            // We could use ToSingleMethod here but then we'd have issues when using .When() conditionals to inject
            // multiple factories in different places
            return container.Bind<IFactory<TContract>>()
                .ToMethod((c, ctx) => c.Instantiate<GameObjectFactory<TContract>>(prefab));
        }
#endif

        public static void Inject(this DiContainer container, object injectable)
        {
            container.Inject(injectable, Enumerable.Empty<object>());
        }

        public static void Inject(this DiContainer container, object injectable, IEnumerable<object> additional)
        {
            container.Inject(injectable, additional, false);
        }

        public static void Inject(this DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            container.Inject(injectable, additional, shouldUseAll, TypeAnalyzer.GetInfo(injectable.GetType()));
        }

        internal static void Inject(
            this DiContainer container, object injectable,
            IEnumerable<object> additional, bool shouldUseAll, ZenjectTypeInfo typeInfo)
        {
            Assert.That(!additional.Contains(null),
                "Null value given to injection argument list. In order to use null you must provide a List<TypeValuePair> and not just a list of objects");

            container.Inject(
                injectable,
                InstantiateUtil.CreateTypeValueList(additional), shouldUseAll, typeInfo);
        }

        public static T Instantiate<T>(
            this DiContainer container, params object[] extraArgs)
        {
            return (T)container.Instantiate(typeof(T), extraArgs);
        }

        public static object Instantiate(
            this DiContainer container, Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return container.InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        public static T InstantiateExplicit<T>(this DiContainer container, List<TypeValuePair> extraArgMap)
        {
            return (T)container.InstantiateExplicit(typeof(T), extraArgMap);
        }

        public static ValueBinder<TContract> BindValue<TContract>(this DiContainer container) where TContract : struct
        {
            return container.BindValue<TContract>(null);
        }

        public static BindingConditionSetter BindInstance<TContract>(this DiContainer container, TContract obj)
            where TContract : class
        {
            return container.Bind<TContract>().ToInstance(obj);
        }

        public static ReferenceBinder<TContract> Bind<TContract>(this DiContainer container)
            where TContract : class
        {
            return container.Bind<TContract>(null);
        }

        public static BinderUntyped Bind(this DiContainer container, Type contractType)
        {
            return container.Bind(contractType, null);
        }

        public static IEnumerable<ZenjectResolveException> ValidateResolve<TContract>(this DiContainer container)
        {
            return container.ValidateResolve<TContract>((string)null);
        }

        public static IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(this DiContainer container, params Type[] extras)
        {
            return container.ValidateObjectGraph(typeof(TConcrete), extras);
        }

        public static List<Type> ResolveTypeAll(this DiContainer container, Type type)
        {
            return container.ResolveTypeAll(new InjectContext(container, type, null));
        }

        public static TContract Resolve<TContract>(this DiContainer container)
        {
            return container.Resolve<TContract>((string)null);
        }

        public static TContract Resolve<TContract>(this DiContainer container, string identifier)
        {
            return container.Resolve<TContract>(new InjectContext(container, typeof(TContract), identifier));
        }

        public static TContract TryResolve<TContract>(this DiContainer container)
            where TContract : class
        {
            return container.TryResolve<TContract>((string)null);
        }

        public static TContract TryResolve<TContract>(this DiContainer container, string identifier)
            where TContract : class
        {
            return (TContract)container.TryResolve(typeof(TContract), identifier);
        }

        public static object TryResolve(this DiContainer container, Type contractType)
        {
            return container.TryResolve(contractType, null);
        }

        public static object TryResolve(this DiContainer container, Type contractType, string identifier)
        {
            return container.Resolve(new InjectContext(container, contractType, identifier, true));
        }

        public static object Resolve(this DiContainer container, Type contractType)
        {
            return container.Resolve(new InjectContext(container, contractType, null));
        }

        public static object Resolve(this DiContainer container, Type contractType, string identifier)
        {
            return container.Resolve(new InjectContext(container, contractType, identifier));
        }

        public static TContract Resolve<TContract>(this DiContainer container, InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (TContract) container.Resolve(context);
        }

        public static bool Unbind<TContract>(this DiContainer container)
        {
            return container.Unbind<TContract>(null);
        }

        public static IEnumerable<Type> GetDependencyContracts<TContract>(this DiContainer container)
        {
            return container.GetDependencyContracts(typeof(TContract));
        }

        public static bool HasBinding<TContract>(this DiContainer container)
        {
            return container.HasBinding<TContract>(null);
        }

        public static bool HasBinding<TContract>(this DiContainer container, string identifier)
        {
            return container.HasBinding(
                new InjectContext(container, typeof(TContract), identifier));
        }

        public static List<TContract> ResolveAll<TContract>(this DiContainer container)
        {
            return container.ResolveAll<TContract>((string)null);
        }

        public static List<TContract> ResolveAll<TContract>(this DiContainer container, bool optional)
        {
            return container.ResolveAll<TContract>(null, optional);
        }

        public static List<TContract> ResolveAll<TContract>(this DiContainer container, string identifier)
        {
            return container.ResolveAll<TContract>(identifier, false);
        }

        public static List<TContract> ResolveAll<TContract>(this DiContainer container, string identifier, bool optional)
        {
            var context = new InjectContext(container, typeof(TContract), identifier, optional);
            return container.ResolveAll<TContract>(context);
        }

        public static List<TContract> ResolveAll<TContract>(this DiContainer container, InjectContext context)
        {
            Assert.IsEqual(context.MemberType, typeof(TContract));
            return (List<TContract>) container.ResolveAll(context);
        }

        public static IList ResolveAll(this DiContainer container, Type contractType)
        {
            return container.ResolveAll(contractType, null);
        }

        public static IList ResolveAll(this DiContainer container, Type contractType, string identifier)
        {
            return container.ResolveAll(contractType, identifier, false);
        }

        public static IList ResolveAll(this DiContainer container, Type contractType, bool optional)
        {
            return container.ResolveAll(contractType, null, optional);
        }

        public static IList ResolveAll(this DiContainer container, Type contractType, string identifier, bool optional)
        {
            var context = new InjectContext(container, contractType, identifier, optional);
            return container.ResolveAll(context);
        }

        public static IEnumerable<ZenjectResolveException> ValidateResolve<TContract>(this DiContainer container, string identifier)
        {
            return container.ValidateResolve(new InjectContext(container, typeof(TContract), identifier));
        }

        public static BindingConditionSetter BindFactoryToMethodUntyped<TContract>(this DiContainer container, Func<DiContainer, object[], TContract> method)
        {
            return container.Bind<IFactoryUntyped<TContract>>().ToInstance(new FactoryMethodUntyped<TContract>(container, method));
        }

        public static BindingConditionSetter BindFactoryToMethod<TContract>(this DiContainer container, Func<DiContainer, TContract> method)
        {
            return container.Bind<IFactory<TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TContract>>(method));
        }

        public static BindingConditionSetter BindFactoryToMethod<TParam1, TContract>(this DiContainer container, Func<DiContainer, TParam1, TContract> method)
        {
            return container.Bind<IFactory<TParam1, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TContract>>(method));
        }

        public static BindingConditionSetter BindFactoryToMethod<TParam1, TParam2, TContract>(this DiContainer container, Func<DiContainer, TParam1, TParam2, TContract> method)
        {
            return container.Bind<IFactory<TParam1, TParam2, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TParam2, TContract>>(method));
        }

        public static BindingConditionSetter BindFactoryToMethod<TParam1, TParam2, TParam3, TContract>(this DiContainer container, Func<DiContainer, TParam1, TParam2, TParam3, TContract> method)
        {
            return container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>().ToMethod((c, ctx) => c.Instantiate<FactoryMethod<TParam1, TParam2, TParam3, TContract>>(method));
        }

        public static BindingConditionSetter BindFactory<TContract>(this DiContainer container)
        {
            return container.Bind<IFactory<TContract>>().ToSingle<Factory<TContract>>();
        }

        public static BindingConditionSetter BindFactory<TParam1, TContract>(this DiContainer container)
        {
            return container.Bind<IFactory<TParam1, TContract>>().ToSingle<Factory<TParam1, TContract>>();
        }

        public static BindingConditionSetter BindFactory<TParam1, TParam2, TContract>(this DiContainer container)
        {
            return container.Bind<IFactory<TParam1, TParam2, TContract>>().ToSingle<Factory<TParam1, TParam2, TContract>>();
        }

        public static BindingConditionSetter BindFactory<TParam1, TParam2, TParam3, TContract>(this DiContainer container)
        {
            return container.Bind<IFactory<TParam1, TParam2, TParam3, TContract>>().ToSingle<Factory<TParam1, TParam2, TParam3, TContract>>();
        }

        // Bind IFactory<TContract> such that it creates instances of type TConcrete
        public static BindingConditionSetter BindFactoryToFactory<TContract, TConcrete>(this DiContainer container)
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TContract>((c) => c.Resolve<IFactory<TConcrete>>().Create());
        }

        public static BindingConditionSetter BindFactoryToFactory<TParam1, TContract, TConcrete>(this DiContainer container)
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TParam1, TContract>((c, param1) => c.Resolve<IFactory<TParam1, TConcrete>>().Create(param1));
        }

        public static BindingConditionSetter BindFactoryToFactory<TParam1, TParam2, TContract, TConcrete>(this DiContainer container)
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TParam1, TParam2, TContract>((c, param1, param2) => c.Resolve<IFactory<TParam1, TParam2, TConcrete>>().Create(param1, param2));
        }

        public static BindingConditionSetter BindFactoryToFactory<TParam1, TParam2, TParam3, TContract, TConcrete>(this DiContainer container)
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TParam1, TParam2, TParam3, TContract>((c, param1, param2, param3) => c.Resolve<IFactory<TParam1, TParam2, TParam3, TConcrete>>().Create(param1, param2, param3));
        }

        public static BindingConditionSetter BindFactoryToCustomFactory<TContract, TConcrete, TFactory>(this DiContainer container)
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TContract>((c) => c.Resolve<TFactory>().Create());
        }

        // Bind IFactory<TContract> to the given factory
        public static BindingConditionSetter BindFactoryToCustomFactory<TParam1, TContract, TConcrete, TFactory>(this DiContainer container)
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return container.BindFactoryToMethod<TParam1, TContract>((c, param1) => c.Resolve<TFactory>().Create(param1));
        }

        // This occurs so often that we might as well have a convenience method
        public static BindingConditionSetter BindFactoryUntyped<TContract>(this DiContainer container)
        {
#if !ZEN_NOT_UNITY3D
            if (typeof(TContract).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Error while binding factory for type '{0}'. Must use version of BindFactory which includes a reference to a prefab you wish to instantiate"
                    .Fmt(typeof(TContract).Name()));
            }
#endif

            return container.Bind<IFactoryUntyped<TContract>>().ToSingle<FactoryUntyped<TContract>>();
        }

        public static BindingConditionSetter BindFactoryUntyped<TContract, TConcrete>(this DiContainer container) where TConcrete : TContract
        {
#if !ZEN_NOT_UNITY3D
            if (typeof(TContract).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Error while binding factory for type '{0}'. Must use version of BindFactory which includes a reference to a prefab you wish to instantiate"
                    .Fmt(typeof(TConcrete).Name()));
            }
#endif

            return container.Bind<IFactoryUntyped<TContract>>().ToSingle<FactoryUntyped<TContract, TConcrete>>();
        }

        public static void BindAllInterfacesToSingle<TConcrete>(this DiContainer container)
            where TConcrete : class
        {
            container.BindAllInterfacesToSingle(typeof(TConcrete));
        }

        public static void BindAllInterfacesToSingle(this DiContainer container, Type concreteType)
        {
            foreach (var interfaceType in concreteType.GetInterfaces())
            {
                Assert.That(concreteType.DerivesFrom(interfaceType));
                container.Bind(interfaceType).ToSingle(concreteType);
            }
        }
    }
}

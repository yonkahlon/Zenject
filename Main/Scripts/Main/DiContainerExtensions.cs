using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif
using ModestTree.Util;

namespace Zenject
{
    // Convenience methods for DiContainer
    public static class DiContainerExtensions
    {
#if !ZEN_NOT_UNITY3D
        // Inject dependencies into child game objects
        public static void InjectGameObject(
            this DiContainer container, GameObject gameObject,
            bool recursive = true, bool includeInactive = false)
        {
            container.InjectGameObject(gameObject, recursive, includeInactive, Enumerable.Empty<object>());
        }

        public static void InjectGameObject(
            this DiContainer container, GameObject gameObject,
            bool recursive, bool includeInactive, IEnumerable<object> extraArgs)
        {
            container.InjectGameObject(
                gameObject, recursive, includeInactive, extraArgs, null);
        }

        public static void InjectGameObject(
            this DiContainer container, GameObject gameObject,
            bool recursive, bool includeInactive, IEnumerable<object> extraArgs, InjectContext context)
        {
            IEnumerable<MonoBehaviour> components;

            if (recursive)
            {
                components = UnityUtil.GetComponentsInChildrenDepthFirst<MonoBehaviour>(gameObject, includeInactive);
            }
            else
            {
                if (!includeInactive && !gameObject.activeSelf)
                {
                    return;
                }

                components = gameObject.GetComponents<MonoBehaviour>();
            }

            foreach (var component in components)
            {
                // null if monobehaviour link is broken
                if (component != null)
                {
                    container.Inject(component, extraArgs, context);
                }
            }
        }
#endif

        public static void Inject(this DiContainer container, object injectable)
        {
            container.Inject(injectable, Enumerable.Empty<object>());
        }

        public static void Inject(this DiContainer container, object injectable, InjectContext context)
        {
            container.Inject(
                injectable, Enumerable.Empty<object>(), false,
                TypeAnalyzer.GetInfo(injectable.GetType()), context);
        }

        public static void Inject(this DiContainer container, object injectable, IEnumerable<object> additional)
        {
            container.Inject(injectable, additional, false);
        }

        public static void Inject(this DiContainer container, object injectable, IEnumerable<object> additional, InjectContext context)
        {
            container.Inject(injectable, additional, false,
                TypeAnalyzer.GetInfo(injectable.GetType()), context);
        }

        public static void Inject(this DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            container.Inject(
                injectable, additional, shouldUseAll,
                TypeAnalyzer.GetInfo(injectable.GetType()), new InjectContext(container, injectable.GetType(), null));
        }

        internal static void Inject(
            this DiContainer container, object injectable,
            IEnumerable<object> additional, bool shouldUseAll, ZenjectTypeInfo typeInfo, InjectContext context)
        {
            Assert.That(!additional.Contains(null),
                "Null value given to injection argument list. In order to use null you must provide a List<TypeValuePair> and not just a list of objects");

            container.Inject(
                injectable,
                InstantiateUtil.CreateTypeValueList(additional), shouldUseAll, typeInfo, context);
        }

        public static ValueBinder<TContract> BindValue<TContract>(this DiContainer container) where TContract : struct
        {
            return container.BindValue<TContract>(null);
        }

        public static BindingConditionSetter BindInstance<TContract>(this DiContainer container, string identifier, TContract obj)
            where TContract : class
        {
            return container.Bind<TContract>(identifier).ToInstance(obj);
        }

        public static BindingConditionSetter BindInstance<TContract>(this DiContainer container, TContract obj)
            where TContract : class
        {
            return container.Bind<TContract>().ToInstance(obj);
        }

        public static BindingConditionSetter BindValueInstance<TContract>(this DiContainer container, TContract value)
            where TContract : struct
        {
            return container.BindValue<TContract>().To(value);
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

        public static IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(
            this DiContainer container, params Type[] extras)
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
            return container.Bind<T>().ToMethod((ctx) => ctx.Container.Instantiate<T>(prefab));
        }
#endif
    }
}

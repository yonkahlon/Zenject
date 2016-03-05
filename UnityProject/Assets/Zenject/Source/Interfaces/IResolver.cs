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
    public interface IResolver
    {
        // When you call any of these Inject methods
        //    Any fields marked [Inject] will be set using the bindings on the container
        //    Any methods marked with a [PostInject] will be called
        //    Any constructor parameters will be filled in with values from the container
        void Inject(object injectable);
        void Inject(object injectable, IEnumerable<object> additional);
        void Inject(object injectable, IEnumerable<object> additional, bool shouldUseAll);

        void Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll,
            InjectContext context);

        void Inject(
            object injectable, IEnumerable<object> additional, bool shouldUseAll,
            InjectContext context, ZenjectTypeInfo typeInfo);

#if !ZEN_NOT_UNITY3D
        // Inject dependencies into any and all child components on the given game object
        void InjectGameObject(
            GameObject gameObject);

        void InjectGameObject(
            GameObject gameObject, bool recursive);

        void InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive);

        void InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive,
            IEnumerable<object> extraArgs);

        void InjectGameObject(
            GameObject gameObject, bool recursive, bool includeInactive,
            IEnumerable<object> extraArgs, InjectContext context);
#endif

        // InjectExplicit is only necessary when you want to inject null values into your object
        // otherwise you can just use Inject()
        void InjectExplicit(
            object injectable, IEnumerable<TypeValuePair> extraArgs,
            bool shouldUseAll, ZenjectTypeInfo typeInfo, InjectContext context,
            string concreteIdentifier);

        void InjectExplicit(object injectable, List<TypeValuePair> additional);
        void InjectExplicit(object injectable, List<TypeValuePair> additional, InjectContext context);

        // Resolve<> - Lookup a value in the container.
        //
        // Note that this may result in a new object being created (for transient bindings) or it
        // may return an already created object (for ToInstance or ToSingle, etc. bindings)
        //
        // If a single unique value for the given type cannot be found, an exception is thrown.
        //
        TContract Resolve<TContract>();
        TContract Resolve<TContract>(string identifier);
        // InjectContext can be used to add more constraints to the object that you want to retrieve
        TContract Resolve<TContract>(InjectContext context);

        // Non-generic versions
        object Resolve(Type contractType);
        object Resolve(Type contractType, string identifier);
        object Resolve(InjectContext context);

        // Same as Resolve<> except it will return null if a value for the given type cannot
        // be found.
        TContract TryResolve<TContract>()
            where TContract : class;
        TContract TryResolve<TContract>(string identifier)
            where TContract : class;

        object TryResolve(Type contractType);
        object TryResolve(Type contractType, string identifier);

        // Same as Resolve<> except it will return all bindings that are associated with the given type
        List<TContract> ResolveAll<TContract>();
        List<TContract> ResolveAll<TContract>(bool optional);
        List<TContract> ResolveAll<TContract>(string identifier);
        List<TContract> ResolveAll<TContract>(string identifier, bool optional);
        List<TContract> ResolveAll<TContract>(InjectContext context);

        // Untyped versions
        IList ResolveAll(InjectContext context);

        IList ResolveAll(Type contractType);
        IList ResolveAll(Type contractType, string identifier);
        IList ResolveAll(Type contractType, bool optional);
        IList ResolveAll(Type contractType, string identifier, bool optional);

        // Returns all the types that would be returned if ResolveAll was called with the given values
        List<Type> ResolveTypeAll(InjectContext context);
        List<Type> ResolveTypeAll(Type type);

        // Validation can be used at edit-time to ensure that no bindings are missing before actually
        // starting your game
        // Normally you don't need to call this yourself
        //
        // Walks the object graph for the given type
        // Should never throw an exception - returns them instead
        //
        // Note: If you just want to know whether a binding exists for the given TContract,
        // use HasBinding instead
        IEnumerable<ZenjectResolveException> ValidateResolve<TContract>();
        IEnumerable<ZenjectResolveException> ValidateResolve<TContract>(string identifier);

        IEnumerable<ZenjectResolveException> ValidateResolve(InjectContext context);

        IEnumerable<ZenjectResolveException> ValidateResolve(Type contractType);
        IEnumerable<ZenjectResolveException> ValidateResolve(Type contractType, string identifier);

        IEnumerable<ZenjectResolveException> ValidateValidatables(params Type[] ignoreTypes);

        // This will validate everything - not just those types that are are in the initial
        // object graph which is what ValidateValidatables does
        // However, this will fail in some valid cases, when using complex conditions that
        // involve looking at the inject stack
        // You probably want to use ValidateValidatables
        IEnumerable<ZenjectResolveException> ValidateAll(params Type[] ignoreTypes);

        IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(
            params Type[] extras);

        IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type concreteType, InjectContext currentContext, string concreteIdentifier,
            params Type[] extras);

        IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type contractType, InjectContext context, params Type[] extras);

        IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            Type contractType, params Type[] extras);

        IEnumerable<ZenjectResolveException> ValidateObjectGraph<TConcrete>(
            InjectContext context, params Type[] extras);
    }
}

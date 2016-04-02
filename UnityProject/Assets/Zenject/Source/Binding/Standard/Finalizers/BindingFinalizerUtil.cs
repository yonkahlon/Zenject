using System;
using System.Collections.Generic;
using Zenject;

namespace ModestTree
{
    public static class BindingFinalizerUtil
    {
        public static ConcreteBindingFinalizer CreateConcreteFinalizerStandard(
            List<Type> concreteTypes, SingletonTypes singletonType,
            DiContainer container, StandardBindingDescriptor binding)
        {
            return new ConcreteBindingFinalizer(
                concreteTypes, singletonType, null,
                (type) => new TransientProvider(
                    type, container, binding.Arguments, binding.ConcreteIdentifier));
        }

        public static ConcreteBindingFinalizer CreateConcreteFinalizerStandard(
            Type concreteType, SingletonTypes singletonType,
            DiContainer container, StandardBindingDescriptor binding)
        {
            return CreateConcreteFinalizerStandard(
                new List<Type>() { concreteType }, singletonType, container, binding);
        }

        public static ConcreteBindingFinalizer CreateConcreteFinalizerStandard<T>(
            SingletonTypes singletonType, DiContainer container, StandardBindingDescriptor binding)
        {
            return CreateConcreteFinalizerStandard(
                typeof(T), singletonType, container, binding);
        }

        public static SelfBindingFinalizer CreateSelfFinalizerStandard(
            SingletonTypes singletonType,
            DiContainer container, StandardBindingDescriptor binding)
        {
            return new SelfBindingFinalizer(
                singletonType, null,
                (type) => new TransientProvider(
                    type, container, binding.Arguments, binding.ConcreteIdentifier));
        }

        public static SingleProviderBindingFinalizer CreateCachedSingleProviderFinalizerStandard<T>(
            DiContainer container)
        {
            return CreateCachedSingleProviderFinalizerStandard<T>(container, new List<TypeValuePair>());
        }

        public static SingleProviderBindingFinalizer CreateCachedSingleProviderFinalizerStandard<T>(
            DiContainer container, List<TypeValuePair> args)
        {
            return new SingleProviderBindingFinalizer(
                new CachedProvider(
                    new TransientProvider(
                        typeof(T), container, args, null)));
        }
    }
}

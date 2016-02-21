using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public static class FacadeUtil
    {
        public static object GetFacadeInstance(
            DiContainer parentContainer, Type facadeType, Action<IBinder> installMethod, Type installerType)
        {
            var subContainer = FacadeUtil.CreateSubContainer(
                parentContainer, installMethod, installerType);

            // Create an inject context so we can specify InjectSources.Local
            var subContext = new InjectContext(
                subContainer, facadeType, null, false,
                null, null, "", null, null, null, InjectSources.Local);

            return subContainer.Resolver.Resolve(subContext);
        }

        public static DiContainer CreateSubContainer(
            DiContainer parentContainer, Action<IBinder> installMethod, Type installerType)
        {
            Assert.That(installMethod != null || installerType != null);

            var subContainer = parentContainer.CreateSubContainer();

            if (installMethod != null)
            {
                installMethod(subContainer);
            }

            if (installerType != null)
            {
                subContainer.Binder.Install(installerType);
            }

            return subContainer;
        }

        public static IEnumerable<ZenjectResolveException> Validate(
            Type facadeType, DiContainer parentContainer,
            Action<IBinder> installMethod, Type installerType)
        {
            var subContainer = CreateSubContainer(
                parentContainer, installMethod, installerType);

            // Create an inject context so we can specify InjectSources.Local
            var subContext = new InjectContext(
                subContainer, facadeType, null, false,
                null, null, "", null, null, null, InjectSources.Local);

            foreach (var error in subContainer.Resolver.ValidateResolve(subContext))
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}


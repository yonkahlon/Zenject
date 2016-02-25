using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public static class FacadeUtil
    {
        public static object GetFacadeInstance(
            DiContainer parentContainer, Type facadeType, Action<DiContainer> installMethod, Type installerType)
        {
            var subContainer = FacadeUtil.CreateSubContainer(
                parentContainer, installMethod, installerType);

            // Create an inject context so we can specify InjectSources.Local
            var subContext = new InjectContext(
                subContainer, facadeType, null, InjectSources.Local);

            return subContainer.Resolve(subContext);
        }

        public static DiContainer CreateSubContainer(
            DiContainer parentContainer, Action<DiContainer> installMethod, Type installerType)
        {
            Assert.That(installMethod != null || installerType != null);

            var subContainer = parentContainer.CreateSubContainer();

            if (installMethod != null)
            {
                Assert.IsNull(installerType);

                installMethod(subContainer);
            }

            if (installerType != null)
            {
                Assert.IsNull(installMethod);

                subContainer.Install(installerType);
            }

            return subContainer;
        }

        public static IEnumerable<ZenjectResolveException> Validate(
            Type facadeType, DiContainer parentContainer,
            Action<DiContainer> installMethod, Type installerType)
        {
            var subContainer = CreateSubContainer(
                parentContainer, installMethod, installerType);

            // Create an inject context so we can specify InjectSources.Local
            var subContext = new InjectContext(
                subContainer, facadeType, null, InjectSources.Local);

            foreach (var error in subContainer.ValidateResolve(subContext))
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}


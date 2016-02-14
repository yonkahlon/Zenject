using System;
using System.Collections.Generic;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class FacadeBinder<TFacade>
        where TFacade : IFacade
    {
        readonly Action<IBinder> _installerFunc;
        readonly DiContainer _container;
        readonly string _identifier;

        public FacadeBinder(
            DiContainer container,
            string identifier,
            Action<IBinder> installerFunc)
        {
            _identifier = identifier;
            _container = container;
            _installerFunc = installerFunc;
        }

        IBinder Binder
        {
            get
            {
                return _container.Binder;
            }
        }

        public void ToSingle()
        {
            AddValidator();
            Binder.Bind<IInitializable>().ToResolve<TFacade>(_identifier);
            Binder.Bind<IDisposable>().ToResolve<TFacade>(_identifier);
            Binder.Bind<ITickable>().ToResolve<TFacade>(_identifier);
            Binder.Bind<ILateTickable>().ToResolve<TFacade>(_identifier);
            Binder.Bind<IFixedTickable>().ToResolve<TFacade>(_identifier);
            Binder.Bind<TFacade>(_identifier).ToSingleMethod<TFacade>(CreateFacade);
        }

        void AddValidator()
        {
#if !ZEN_NOT_UNITY3D
            if (!Application.isPlaying)
#endif
            {
                // Unlike with facade factories, we don't really have something to be IValidatable
                // so we have to add a separate object for this in this case
                Binder.Bind<IValidatable>().ToInstance(new Validator(_container, _installerFunc));
            }
        }

        TFacade CreateFacade(InjectContext ctx)
        {
            return FacadeFactory<TFacade>.CreateSubContainer(_container, _installerFunc).Resolver
                .Resolve<TFacade>();
        }

        class Validator : IValidatable
        {
            readonly DiContainer _container;
            readonly Action<IBinder> _installerFunc;

            public Validator(DiContainer container, Action<IBinder> installerFunc)
            {
                _container = container;
                _installerFunc = installerFunc;
            }

            public IEnumerable<ZenjectResolveException> Validate()
            {
                return FacadeFactory<TFacade>.Validate(_container, _installerFunc);
            }
        }
    }
}

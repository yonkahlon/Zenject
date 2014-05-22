using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class Module
    {
        protected DiContainer _container;

        public DiContainer Container
        {
            set
            {
                _container = value;
            }
        }

        public abstract void AddBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}

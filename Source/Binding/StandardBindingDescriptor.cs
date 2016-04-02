using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public enum CreationTypes
    {
        Transient,
        Singleton,
        Cached,
    }

    public class StandardBindingDescriptor : IBindingDescriptor
    {
        readonly List<TypeValuePair> _arguments = new List<TypeValuePair>();

        public List<TypeValuePair> Arguments
        {
            get
            {
                return _arguments;
            }
            set
            {
                _arguments.Clear();
                _arguments.AddRange(value);
            }
        }

        public BindingCondition Condition
        {
            get;
            set;
        }

        public List<Type> ContractTypes
        {
            get;
            set;
        }

        public IBindingFinalizer Finalizer
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public string ConcreteIdentifier
        {
            get;
            set;
        }

        public bool InheritInSubContainers
        {
            get;
            set;
        }

        public CreationTypes CreationType
        {
            get;
            set;
        }

        public void Finalize(DiContainer container)
        {
            if (Finalizer == null)
            {
                throw Assert.CreateException(
                    "Unfinished binding! Finalizer was not given.");
            }

            Finalizer.FinalizeBinding(container, this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    public class CommandBinderBase<TCommand, TAction>
        where TCommand : ICommand
        where TAction : class
    {
        readonly StandardBindingDescriptor _binding;
        readonly DiContainer _container;

        public CommandBinderBase(string identifier, DiContainer container)
        {
            _container = container;

            _binding = new StandardBindingDescriptor();
            _binding.ContractTypes = new List<Type>()
            {
                typeof(TCommand),
            };
            _binding.Identifier = identifier;

            container.StartBinding(_binding);
        }

        protected StandardBindingDescriptor Binding
        {
            get
            {
                return _binding;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}


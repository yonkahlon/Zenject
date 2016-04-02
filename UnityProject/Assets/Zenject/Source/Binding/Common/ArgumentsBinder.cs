using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    public class ArgumentsBinder : ConditionBinder
    {
        public ArgumentsBinder(StandardBindingDescriptor binding)
            : base(binding)
        {
        }

        public ConditionBinder WithArguments(params object[] args)
        {
            Binding.Arguments = InjectUtil.CreateArgList(args);
            return this;
        }

        public ConditionBinder WithArgumentsExplicit(IEnumerable<TypeValuePair> extraArgs)
        {
            Binding.Arguments = extraArgs.ToList();
            return this;
        }
    }
}

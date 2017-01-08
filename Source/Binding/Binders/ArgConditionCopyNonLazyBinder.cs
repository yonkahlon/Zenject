using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    public class ArgConditionCopyNonLazyBinder : ConditionCopyNonLazyBinder
    {
        public ArgConditionCopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ConditionCopyNonLazyBinder WithArguments(params object[] args)
        {
            BindInfo.Arguments = InjectUtil.CreateArgList(args);
            return this;
        }

        public ConditionCopyNonLazyBinder WithArgumentsExplicit(IEnumerable<TypeValuePair> extraArgs)
        {
            BindInfo.Arguments = extraArgs.ToList();
            return this;
        }
    }
}

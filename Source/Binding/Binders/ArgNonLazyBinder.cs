using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    public class ArgNonLazyBinder : NonLazyBinder
    {
        public ArgNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public NonLazyBinder WithArguments(params object[] args)
        {
            BindInfo.Arguments = InjectUtil.CreateArgList(args);
            return this;
        }

        public NonLazyBinder WithArgumentsExplicit(IEnumerable<TypeValuePair> extraArgs)
        {
            BindInfo.Arguments = extraArgs.ToList();
            return this;
        }
    }
}


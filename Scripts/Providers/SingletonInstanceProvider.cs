using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public class SingletonInstanceProvider : ProviderBase
    {
        readonly object _instance;

        public SingletonInstanceProvider(object instance)
        {
            _instance = instance;
        }

        public override Type GetInstanceType()
        {
            return _instance.GetType();
        }

        public override object GetInstance()
        {
            return _instance;
        }

        public override void ValidateBinding()
        {
        }
    }
}

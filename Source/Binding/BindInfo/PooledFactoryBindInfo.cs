using System;

namespace Zenject
{
    public enum PoolExpandMethods
    {
        OneAtATime,
        Double,
        Fixed,
    }

    public class PooledFactoryBindInfo
    {
        public PooledFactoryBindInfo()
        {
        }

        public PoolExpandMethods ExpandMethod
        {
            get; set;
        }

        public int InitialSize
        {
            get; set;
        }
    }
}


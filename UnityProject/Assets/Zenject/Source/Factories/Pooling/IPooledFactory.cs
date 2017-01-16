namespace Zenject
{
    public interface IPooledFactory
    {
        int NumCreated { get; }
        int NumActive { get; }
        int NumInactive { get; }
    }

    public interface IPooledFactory<TValue> : IPooledFactory
    {
        TValue Spawn();
        void Despawn(TValue item);
    }

    public interface IPooledFactory<in TParam1, TValue> : IPooledFactory
    {
        TValue Spawn(TParam1 param);
        void Despawn(TValue item);
    }

    public interface IPooledFactory<in TParam1, in TParam2, TValue> : IPooledFactory
    {
        TValue Spawn(TParam1 param1, TParam2 param2);
        void Despawn(TValue item);
    }

    public interface IPooledFactory<in TParam1, in TParam2, in TParam3, TValue> : IPooledFactory
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3);
        void Despawn(TValue item);
    }

    public interface IPooledFactory<in TParam1, in TParam2, in TParam3, in TParam4, TValue> : IPooledFactory
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
        void Despawn(TValue item);
    }

    public interface IPooledFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, TValue> : IPooledFactory
    {
        TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
        void Despawn(TValue item);
    }
}


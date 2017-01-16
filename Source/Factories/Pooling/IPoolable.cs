namespace Zenject
{
    public interface IPoolableBase
    {
        void OnDespawned();
    }

    public interface IPoolable : IPoolableBase
    {
        void OnSpawned();
    }

    public interface IPoolable<TParam1> : IPoolableBase
    {
        void OnSpawned(TParam1 p1);
    }

    public interface IPoolable<TParam1, TParam2> : IPoolableBase
    {
        void OnSpawned(TParam1 p1, TParam2 p2);
    }

    public interface IPoolable<TParam1, TParam2, TParam3> : IPoolableBase
    {
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4> : IPoolableBase
    {
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5> : IPoolableBase
    {
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5);
    }
}

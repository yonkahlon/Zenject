namespace ModestTree.Zenject
{
    public interface IEntryPoint
    {
        int InitPriority { get; }

        void Initialize();
    }
}

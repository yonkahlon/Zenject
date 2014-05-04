namespace ModestTree.Zenject
{
    public interface IEntryPoint
    {
        // Return null if you don't care when your initialize gets called
        int? InitPriority { get; }

        void Initialize();
    }
}

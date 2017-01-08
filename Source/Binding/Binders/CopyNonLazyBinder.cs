using ModestTree;
namespace Zenject
{
    public class CopyNonLazyBinder : NonLazyBinder
    {
        public CopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public NonLazyBinder CopyIntoAllSubContainers()
        {
            BindInfo.CopyIntoAllSubContainers = true;
            return this;
        }

        // Would this be useful?
        //public NonLazyBinder CopyIntoDirectSubContainers()
    }
}

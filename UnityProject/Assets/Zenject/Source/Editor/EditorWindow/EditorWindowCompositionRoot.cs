using System.Collections.Generic;
using UnityEngine;

namespace Zenject
{
    [CreateAssetMenu(fileName = "UntitledCompositionRoot", menuName = "Zenject/Editor Window Composition Root", order = 1)]
    public class EditorWindowCompositionRoot : ScriptableObject
    {
        [SerializeField]
        List<MonoEditorInstaller> _installers = null;

        public void Initialize(
            DiContainer container, EditorWindowFacade root)
        {
            foreach (var installer in _installers)
            {
                installer.Container = container;
                installer.InstallBindings();
            }

            container.Bind<TickableManager>().ToSelf().AsSingle();
            container.Bind<InitializableManager>().ToSelf().AsSingle();
            container.Bind<DisposableManager>().ToSelf().AsSingle();
            container.Bind<GuiRenderableManager>().ToSelf().AsSingle();

            container.Bind<IDependencyRoot>().ToInstance(root);

            container.Inject(root);
        }
    }
}

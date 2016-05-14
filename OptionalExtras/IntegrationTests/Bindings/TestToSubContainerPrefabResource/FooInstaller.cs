using UnityEngine;
using System.Collections;
using Zenject;

namespace Zenject.Tests.ToSubContainerPrefabResource
{
    public class FooInstaller : MonoInstaller
    {
        [SerializeField]
        Bar _bar;

        public override void InstallBindings()
        {
            Container.BindInstance(_bar);
            Container.Bind<Gorp>("gorp").AsSingle();
        }
    }
}

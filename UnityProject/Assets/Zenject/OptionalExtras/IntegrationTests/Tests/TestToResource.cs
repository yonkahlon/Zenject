using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace ModestTree
{
    public class TestToResource : MonoInstallerTestFixture
    {
        protected override float Delay
        {
            get
            {
                return 1.0f;
            }
        }

        [InstallerTest]
        public void Test1()
        {
            Container.Bind<Texture>().ToResource("TestTexture");

            Container.Bind<IInitializable>().ToSingleGameObject<Runner1>();
        }

        public class Runner1 : MonoBehaviour, IInitializable
        {
            Texture _texture;

            [PostInject]
            public void Construct(Texture texture)
            {
                _texture = texture;
            }

            public void Initialize()
            {
            }

            void OnGUI()
            {
                var rect = new Rect(0, 0, Screen.width * 0.5f, Screen.height * 0.5f);

                GUI.DrawTexture(rect, _texture);
            }
        }
    }
}

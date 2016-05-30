using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject.TestFramework;
using Zenject;

namespace Zenject.Tests.ToResource
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        const string ResourcePath = "TestToResource/TestTexture";
        const string ResourcePath2 = "TestToResource/TestTexture2";

        [Test]
        public void TestBasic()
        {
            Container.Bind<Texture>().FromResource(ResourcePath);

            Container.Bind<Runner>().FromGameObject().AsSingle().WithArguments(1);
            Container.BindRootResolve<Runner>();
        }

        [Test]
        public void TestTransient()
        {
            Container.Bind<Texture>().FromResource(ResourcePath).AsTransient();
            Container.Bind<Texture>().FromResource(ResourcePath);
            Container.Bind<Texture>().To<Texture>().FromResource(ResourcePath);

            Container.Bind<Runner>().FromGameObject().AsSingle().WithArguments(3);
            Container.BindRootResolve<Runner>();
        }

        [Test]
        public void TestCached()
        {
            Container.Bind<Texture>().FromResource(ResourcePath).AsCached();

            Container.Bind<Runner>().FromGameObject().AsSingle().WithArguments(1);
            Container.BindRootResolve<Runner>();
        }

        [Test]
        public void TestSingle()
        {
            Container.Bind<Texture>().FromResource(ResourcePath).AsSingle();
            Container.Bind<Texture>().FromResource(ResourcePath).AsSingle();

            Container.Bind<Runner>().FromGameObject().AsSingle().WithArguments(2);
            Container.BindRootResolve<Runner>();
        }

        [Test]
        [ExpectedException]
        [ExpectedValidationException]
        public void TestSingleWithError()
        {
            Container.Bind<Texture>().FromResource(ResourcePath).AsSingle();
            Container.Bind<Texture>().FromResource(ResourcePath2).AsSingle();

            Container.Bind<Runner>().FromGameObject().AsSingle().WithArguments(2);
            Container.BindRootResolve<Runner>();
        }

        public class Runner : MonoBehaviour
        {
            List<Texture> _textures;

            [Inject]
            public void Construct(List<Texture> textures, int expectedAmount)
            {
                _textures = textures;

                Assert.IsEqual(textures.Count, expectedAmount);
            }

            void OnGUI()
            {
                int top = 0;

                foreach (var tex in _textures)
                {
                    var rect = new Rect(0, top, Screen.width * 0.5f, Screen.height * 0.5f);

                    GUI.DrawTexture(rect, tex);

                    top += 200;
                }
            }
        }
    }
}

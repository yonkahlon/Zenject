using System.Linq;
using ModestTree;
using Zenject.TestFramework;

namespace Zenject.Tests.TestParenting
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        [Test]
        public void TestChildWithParent_Manual()
        {
            var parentContext = GetScene("TestParenting").GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SceneContext>()).Single();
            var childContext = GetScene("TestParenting_Child").GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SceneContext>()).Single();
            Assert.That(childContext.Container.ParentContainer == parentContext.Container, "Containers should be nested");
        }

//        [Test]
//        public void TestChildWithParent()
//        {
//			UnloadOtherScenes();
//			LoadSceneAdditive("TestParenting_Child");
//
//			var parentContext = GetScene("TestParenting").GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SceneContext>()).Single();
//			var childContext = GetScene("TestParenting_Child").GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SceneContext>()).Single();
//			Assert.That(childContext.Container.ParentContainer == parentContext.Container, "Containers should be nested");
//        }
//
//		[Test]
//		[ExpectedException]
//		public void TestChildWithTooManyParents()
//		{
//			UnloadOtherScenes();
//			LoadSceneAdditive("TestParenting");
//			LoadSceneAdditive("TestParenting_Child");
//		}
//
//        [Test]
//		[ExpectedException]
//        public void TestChildWithMissingParent()
//        {
//			UnloadOtherScenes();
//			LoadSceneAdditive("TestParenting_Orphan");
//        }
//
//        [Test]
//        public void TestIndependentScene()
//        {
//			UnloadOtherScenes();
//			LoadSceneAdditive("TestParenting_Independent");
//        }
    }
}

using Zenject.TestFramework;


namespace Zenject.Tests.TestParenting
{
    public class Fixture : ZenjectIntegrationTestFixture
    {
        [Test]
        public void TestChildWithParent()
        {
			UnloadOtherScenes();
            LoadSceneAdditive("TestParenting_Child");
        }

		[Test]
		[ExpectedException]
		public void TestChildWithTooManyParents()
		{
			UnloadOtherScenes();
			LoadSceneAdditive("TestParenting");
			LoadSceneAdditive("TestParenting_Child");
		}

        [Test]
		[ExpectedException]
        public void TestChildWithMissingParent()
        {
			UnloadOtherScenes();
			LoadSceneAdditive("TestParenting_Orphan");
        }

        [Test]
        public void TestIndependentScene()
        {
			UnloadOtherScenes();
			LoadSceneAdditive("TestParenting_Independent");
        }
    }
}

using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    class GitBranchTest : GitTestBase
    {
        private GitBranch task;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            task = new GitBranch();
            task.BuildEngine = new MockBuild();
            task.RepositoryPath = TagFreeRepository;
        }

        [TearDown]
        public new void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestMasterBranch()
        {
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("master", task.Branch);
        }

        [Test]
        public void TestBranchBranch()
        {
            CheckoutBranch(TagFreeRepository, "branch");
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("branch", task.Branch);
        }
    }
}

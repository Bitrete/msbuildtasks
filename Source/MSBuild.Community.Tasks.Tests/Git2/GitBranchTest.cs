using LibGit2Sharp;
using MSBuild.Community.Tasks.Git2;
using NUnit.Framework;
using System.Linq;

namespace MSBuild.Community.Tasks.Tests.Git2
{
    [TestFixture]
    class GitBranchTest : GitTestBase
    {
        private GitBranch task;

        [SetUp]
        public void SetUp()
        {
            task = new GitBranch();
            task.BuildEngine = new MockBuild();
            task.RepositoryPath = TestRepositoryPath;
        }

        [TearDown]
        public void TearDown()
        {
            CheckoutBranch("master");
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
            CheckoutBranch("branch");
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("branch", task.Branch);
        }
    }
}

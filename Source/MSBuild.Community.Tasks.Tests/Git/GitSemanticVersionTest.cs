using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    class GitSemanticVersionTest : GitTestBase
    {
        private GitSemanticVersion task;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            task = new GitSemanticVersion();
            task.BuildEngine = new MockBuild();
        }

        [TearDown]
        public new void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestV100()
        {
            task.RepositoryPath = LastCommitTaggedRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("1.0.0", task.Version);
            Assert.IsTrue(task.IsRelease);
        }

        [Test]
        public void TestTag()
        {
            task.RepositoryPath = AlphaTagRepository;
            Assert.IsFalse(task.Execute());
            Assert.AreEqual("", task.Version);
            Assert.IsFalse(task.IsRelease);
        }

        [Test]
        public void TestV060OnBranch()
        {
            CheckoutBranch(TagOnBranchRepository, "branch");
            task.RepositoryPath = TagOnBranchRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("0.6.0", task.Version);
            Assert.IsTrue(task.IsRelease);
        }

        [Test]
        public void TestV060OnMaster()
        {
            task.RepositoryPath = TagOnBranchRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("0.6.0", task.Version);
            Assert.IsFalse(task.IsRelease);
        }

        [Test]
        public void TestV050()
        {
            task.RepositoryPath = TagOnMasterRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("0.5.0", task.Version);
            Assert.IsFalse(task.IsRelease);
        }

        [Test]
        public void TestV020()
        {
            task.RepositoryPath = EarlyTagRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("0.2.0", task.Version);
            Assert.IsFalse(task.IsRelease);
        }
    }
}

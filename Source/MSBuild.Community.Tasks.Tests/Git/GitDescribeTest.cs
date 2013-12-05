using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitDescribeTest : GitTestBase
    {
        private GitDescribe task;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            task = new GitDescribe();
            task.BuildEngine = new MockBuild();
        }

        [TearDown]
        public new void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestTagOnLastCommit()
        {
            task.RepositoryPath = LastCommitTaggedRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("v1.0.0", task.Tag);
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.TaggedCommitHash);
            Assert.AreEqual(0, task.CommitCount);
            Assert.AreEqual("v1.0.0-0-g77c95eb", task.Description);
        }

        [Test]
        public void TestTagOnLastCommitDirty()
        {
            task.RepositoryPath = LastCommitTaggedDirtyRepository;
            task.DirtyMark = "dev";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("v1.0.0", task.Tag);
            Assert.IsTrue(task.Dirty);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.TaggedCommitHash);
            Assert.AreEqual(0, task.CommitCount);
            Assert.AreEqual("v1.0.0-0-g77c95eb-dev", task.Description);
        }

        [Test]
        public void TestTagFree()
        {
            task.RepositoryPath = TagFreeRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("", task.Tag);
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("", task.TaggedCommitHash);
            Assert.AreEqual(0, task.CommitCount);
            Assert.AreEqual("77c95eb", task.Description);
        }

        [Test]
        public void TestTagFreeDirty()
        {
            task.RepositoryPath = TagFreeDirtyRepository;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("", task.Tag);
            Assert.IsTrue(task.Dirty);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("", task.TaggedCommitHash);
            Assert.AreEqual(0, task.CommitCount);
            Assert.AreEqual("77c95eb-dirty", task.Description);
        }

        [Test]
        public void TestTagOnMaster()
        {
            task.RepositoryPath = TagOnMasterRepository;
            Assert.IsTrue(task.Execute());
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("v0.5.0", task.Tag);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("86ccf0b435176e1a1a939041b7dfe4824a7548e5", task.TaggedCommitHash);
            Assert.AreEqual(3, task.CommitCount);
            Assert.AreEqual("v0.5.0-3-g77c95eb", task.Description);
        }

        [Test]
        public void TestTagOnBranch1()
        {
            CheckoutBranch(TagOnBranchRepository, "branch");
            task.RepositoryPath = TagOnBranchRepository;
            Assert.IsTrue(task.Execute());
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("v0.6.0", task.Tag);
            Assert.AreEqual("d85c67bab517fcb53ed4ca84e3f68dc51482afe7", task.CommitHash);
            Assert.AreEqual("d85c67bab517fcb53ed4ca84e3f68dc51482afe7", task.TaggedCommitHash);
            Assert.AreEqual(0, task.CommitCount);
            Assert.AreEqual("v0.6.0-0-gd85c67b", task.Description);
        }

        [Test]
        public void TestTagOnBranch2()
        {
            CheckoutBranch(TagOnBranchRepository, "master");
            task.RepositoryPath = TagOnBranchRepository;
            Assert.IsTrue(task.Execute());
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("v0.6.0", task.Tag);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("d85c67bab517fcb53ed4ca84e3f68dc51482afe7", task.TaggedCommitHash);
            Assert.AreEqual(3, task.CommitCount);
            Assert.AreEqual("v0.6.0-3-g77c95eb", task.Description);
        }

        [Test]
        public void TestEarlyTag()
        {
            task.RepositoryPath = EarlyTagRepository;
            Assert.IsTrue(task.Execute());
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("v0.2.0", task.Tag);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("25f17f14afde4a412756d65e1480374e6e46b285", task.TaggedCommitHash);
            Assert.AreEqual(5, task.CommitCount);
            Assert.AreEqual("v0.2.0-5-g77c95eb", task.Description);
        }

        [Test]
        public void TestAlphaTag()
        {
            task.RepositoryPath = AlphaTagRepository;
            Assert.IsTrue(task.Execute());
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("tag", task.Tag);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("8c1541031cf33ff80fc014a3b7b2b9d0ead42bf4", task.TaggedCommitHash);
            Assert.AreEqual(1, task.CommitCount);
            Assert.AreEqual("tag-1-g77c95eb", task.Description);
        }
    }
}

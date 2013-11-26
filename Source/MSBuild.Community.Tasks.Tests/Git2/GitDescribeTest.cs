using MSBuild.Community.Tasks.Git2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Tests.Git2
{
    [TestFixture]
    public class GitDescribeTest :GitTestBase
    {
        private GitDescribe task;

        [SetUp]
        public void SetUp()
        {
            CheckoutBranch("master");
            task = new GitDescribe();
            task.RepositoryPath = TestRepositoryPath;
            task.BuildEngine = new MockBuild();
        }

        [TearDown]
        public void TearDown()
        {
            CheckoutBranch("master");
        }

        [Test]
        public void TestTag005()
        {
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("v0.0.5", task.Tag);
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
            Assert.AreEqual("86ccf0b435176e1a1a939041b7dfe4824a7548e5", task.TaggedCommitHash);
            Assert.AreEqual(3, task.CommitCount);
        }

        [Test]
        public void TestTag001()
        {
            CheckoutBranch("branch");
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("v0.0.1", task.Tag);
            Assert.IsFalse(task.Dirty);
            Assert.AreEqual("d85c67bab517fcb53ed4ca84e3f68dc51482afe7", task.CommitHash);
            Assert.AreEqual("56a4945c4119ee09970fdd27be2e40444114dc3a", task.TaggedCommitHash);
            Assert.AreEqual(3, task.CommitCount);
        }
    }
}

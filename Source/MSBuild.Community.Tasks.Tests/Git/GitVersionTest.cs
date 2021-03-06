﻿using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    class GitVersionTest : GitTestBase
    {
        private GitVersion task;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            task = new GitVersion();
            task.BuildEngine = new MockBuild();
            task.RepositoryPath = TagFreeRepository;
        }

        [TearDown]
        public new void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestHead()
        {
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("77c95eb", task.CommitHash);
        }

        [Test]
        public void TeshHeadLong()
        {
            task.Short = false;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("77c95ebb5565695348f674ee5fdd419af78807c9", task.CommitHash);
        }

        [Test]
        public void TestFirstParent()
        {
            task.PredecessorOffset = 1;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("8c15410", task.CommitHash);
        }

        [Test]
        public void TestFirstParentOfFirstParent()
        {
            task.PredecessorOffset = 2;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("d85c67b", task.CommitHash);
        }

        [Test]
        public void TestSecondParentOfFirstParent()
        {
            task.PredecessorOffset = 3;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("86ccf0b", task.CommitHash);
        }

        [Test]
        public void TestFirstParentOfGrandparent()
        {
            task.PredecessorOffset = 4;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual("8a923cc", task.CommitHash);
        }

        [Test]
        public void TestRepositoryFailure()
        {
            task.RepositoryPath = "bla-bla";
            Assert.IsFalse(task.Execute());
        }
    }
}

using MSBuild.Community.Tasks.Git;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    class SemanticVersionGitDescribeExtractorTest
    {
        private SemanticVersionGitDescribeExtractor extractor;

        [SetUp]
        public void SetUp()
        {
            extractor = new SemanticVersionGitDescribeExtractor();
        }

        [Test]
        public void ReleaseTag()
        {
            Assert.IsTrue(extractor.Extract("v0.0.1-0-g5b7a70a"));
            Assert.AreEqual("0.0.1", extractor.Version);
            Assert.AreEqual("0", extractor.AdditionalCommitsCount);
            Assert.AreEqual("5b7a70a", extractor.Hash);
        }

        [Test]
        public void DevelopmentTag()
        {
            Assert.IsTrue(extractor.Extract("v1.2.3-19-gcafebee"));
            Assert.AreEqual("1.2.3", extractor.Version);
            Assert.AreEqual("19", extractor.AdditionalCommitsCount);
            Assert.AreEqual("cafebee", extractor.Hash);
        }

        [Test]
        public void UntaggetCommit()
        {
            Assert.IsTrue(extractor.Extract("cafebee"));
            Assert.IsEmpty(extractor.Version);
            Assert.IsEmpty(extractor.AdditionalCommitsCount);
            Assert.AreEqual("cafebee", extractor.Hash);
        }

        [Test]
        public void FatalText()
        {
            Assert.IsFalse(extractor.Extract("fatal: Not a git repository (or any of the parent directories): .git"));
            Assert.IsEmpty(extractor.Version);
            Assert.IsEmpty(extractor.AdditionalCommitsCount);
            Assert.IsEmpty(extractor.Hash);
        }

        [Test]
        public void EmptyText()
        {
            Assert.IsFalse(extractor.Extract(string.Empty));
            Assert.IsEmpty(extractor.Version);
            Assert.IsEmpty(extractor.AdditionalCommitsCount);
            Assert.IsEmpty(extractor.Hash);
        }
    }
}

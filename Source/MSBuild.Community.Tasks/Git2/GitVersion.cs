using LibGit2Sharp;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Git2
{
    class GitVersion : GitTask
    {
        /// <summary>
        /// 
        /// </summary>
        public GitVersion()
        {
            PredecessorOffset = 0;
            Short = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public int PredecessorOffset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Short { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Output]
        public string CommitHash { get; private set; }

        protected override bool ExecuteCommand(Repository repository)
        {
            var commit = GetCommit(repository, PredecessorOffset);

            CommitHash = Short ? commit.Sha.Substring(0, 7) : commit.Sha;

            return true;
        }

        private Commit GetCommit(Repository repository, int offset)
        {
            return repository.Commits.Skip(offset).First();
        }
    }
}

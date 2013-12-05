using LibGit2Sharp;
using Microsoft.Build.Framework;
using System.Linq;

namespace MSBuild.Community.Tasks.Git
{
    class GitVersion : GitTask
    {
        /// <summary>
        /// Creates new instance of GitVersion task with default settings
        /// </summary>
        public GitVersion()
        {
            PredecessorOffset = 0;
            Short = true;
        }

        /// <summary>
        /// Number of steps to go backward in commit predescessors history
        /// </summary>
        public int PredecessorOffset { get; set; }

        /// <summary>
        /// Optrion to output commit hash in short form
        /// </summary>
        public bool Short { get; set; }

        /// <summary>
        /// Hash of selected commit (current by default or PredecessorOffset back in history)
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

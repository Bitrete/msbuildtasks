using LibGit2Sharp;
using Microsoft.Build.Framework;
using System;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// Task to get information about the most recent tag
    /// </summary>
    public class GitDescribe : GitTask
    {
        private const string DefaultDirtyMark = "dirty";

        /// <summary>
        /// Constructor
        /// </summary>
        public GitDescribe()
        {
            DirtyMark = DefaultDirtyMark;
            CommitCount = 0;
            TaggedCommitHash = string.Empty;
            CommitHash = string.Empty;
            Tag = string.Empty;
            Dirty = false;
            Description = string.Empty;
        }

        /// <summary>
        /// Set or get mark string for dirty (changed) working copy
        /// </summary>
        public string DirtyMark { get; set; }

        /// <summary>
        /// Commits count since recent tag
        /// </summary>
        [Output]
        public int CommitCount { get; private set; }

        /// <summary>
        /// Commit count from beginning
        /// </summary>
        [Output]
        public int CommitCountFromBeginning { get; private set; }

        /// <summary>
        /// Hash of the most recent tagged commit
        /// </summary>
        [Output]
        public string TaggedCommitHash { get; private set; }

        /// <summary>
        /// Hash of HEAD commit
        /// </summary>
        [Output]
        public string CommitHash { get; private set; }

        /// <summary>
        /// Short representation of HEAD commit hash
        /// </summary>
        [Output]
        public string ShortCommitHash
        {
            get
            {
                return CommitHash.Substring(0, 7);
            }
        }

        /// <summary>
        /// The most recent tag name
        /// </summary>
        [Output]
        public string Tag { get; private set; }

        /// <summary>
        /// Dirty flag, True if where is any changes in repository
        /// </summary>
        [Output]
        public bool Dirty { get; private set; }

        /// <summary>
        /// Outputs description as in git describe command
        /// </summary>
        [Output]
        public string Description { get; private set; }

        /// <summary>
        /// Task logic
        /// </summary>
        /// <param name="repository">Git repository object</param>
        /// <returns>True if was executed successfully, overwise false</returns>
        protected override bool ExecuteCommand(Repository repository)
        {
            Commit headCommit = repository.Head.Commits.First();
            if (headCommit == null)
                throw new InvalidOperationException("Could not find HEAD commit"); //TODO: Select better exception class

            CommitHash = headCommit.Sha;
            CommitCountFromBeginning = GetCommitsCountBetweenHeadAndCommit(repository, null);
            Dirty = IsModified(repository);

            if (repository.Tags.Count() > 0)
            {
                Tag recentTag = null;
                int currentDistance = int.MaxValue;
                foreach (Tag tag in repository.Tags)
                {
                    var distance = GetTagDistanceFromHead(repository, tag);
                    if (distance < currentDistance)
                    {
                        currentDistance = distance;
                        recentTag = tag;
                    }
                }

                Tag = recentTag.Name;
                CommitCount = GetCommitsCountBetweenHeadAndCommit(repository, recentTag.Target);
                TaggedCommitHash = recentTag.Target.Sha;
            }

            Description = BuildDescription();

            return true;
        }

        private int GetCommitsCountBetweenHeadAndCommit(Repository repository, GitObject target)
        {
            var filter = new CommitFilter();
            filter.Since = repository.Head.Commits.First();
            filter.Until = target;

            return repository.Commits.QueryBy(filter).Count();
        }

        private int GetTagDistanceFromHead(Repository repository, Tag tag)
        {
            int result = 0;
            foreach (Commit commit in repository.Commits)
            {
                if (commit.Equals(tag.Target))
                    break;

                result++;
            }

            return result;
        }

        private string BuildDescription()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.Empty.Equals(Tag))
            {
                sb.Append(Tag);
                sb.Append("-");
                sb.Append(CommitCount);
                sb.Append("-");
                sb.Append("g");
            }
            sb.Append(ShortCommitHash);

            if (Dirty)
            {
                sb.Append("-");
                sb.Append(DirtyMark);
            }

            return sb.ToString();
        }

        private bool IsModified(Repository repository)
        {
            foreach (var item in repository.Index.RetrieveStatus())
            {
                var state = item.State;
                if (state != FileStatus.Unaltered)
                    return true;
            }

            return false;
        }
    }
}

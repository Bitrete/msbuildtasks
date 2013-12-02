using LibGit2Sharp;
using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Git2
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
        /// The most recent tag
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
            Commit headCommit = repository.Commits.First();
            CommitHash = headCommit.Sha;

            Dirty = IsModified(repository);

            //TODO: Select better exception class
            if (headCommit == null)
                throw new InvalidOperationException("Could not find HEAD commit");

            if (repository.Tags.Count() > 0)
            {
                Dictionary<Commit, Tag> taggedCommits = new Dictionary<Commit, Tag>();
                foreach (Commit commit in repository.Commits)
                    foreach (Tag tag in repository.Tags)
                        if (tag.Target == commit)
                            taggedCommits.Add(commit, tag);

                Dictionary<Commit, int> commitsDistance = CalculateDistance(repository.Commits.ToList(), taggedCommits);

                Commit recentTaggedCommit = commitsDistance.OrderBy(cd => cd.Value).First().Key;

                Tag recentTag = taggedCommits[recentTaggedCommit];

                Tag = recentTag.Name;
                CommitCount = commitsDistance[recentTaggedCommit];
                TaggedCommitHash = recentTaggedCommit.Sha;
            }

            Description = BuildDescription();

            return true;
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

        private Dictionary<Commit, int> CalculateDistance(List<Commit> leftParents, Dictionary<Commit, Tag> taggedCommits)
        {
            Dictionary<Commit, int> result = new Dictionary<Commit, int>();

            foreach (Commit taggedCommit in taggedCommits.Keys)
            {
                int distance = 0;
                foreach (Commit parent in leftParents)
                {
                    if (parent == taggedCommit)
                    {
                        result.Add(parent, distance);
                        break;
                    }
                    distance++;
                }
            }

            return result;
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

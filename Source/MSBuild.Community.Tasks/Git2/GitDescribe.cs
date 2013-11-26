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
        /// Task logic
        /// </summary>
        /// <param name="repository">Git repository object</param>
        /// <returns>True if was executed successfully, overwise false</returns>
        protected override bool ExecuteCommand(Repository repository)
        {
            Commit headCommit = repository.Commits.First();
            CommitHash = headCommit.Sha;

            Dirty = repository.Diff.Compare(headCommit.Tree, DiffTargets.WorkingDirectory).Patch.Length > 0;

            //TODO: Select better exception class
            if (headCommit == null)
                throw new InvalidOperationException("Could not find HEAD commit");

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

            return true;
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
    }
}

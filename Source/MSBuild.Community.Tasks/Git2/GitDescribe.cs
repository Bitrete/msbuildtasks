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
        /// Task logic
        /// </summary>
        /// <param name="repository">Git repository object</param>
        /// <returns>True if was executed successfully, overwise false</returns>
        protected override bool ExecuteCommand(Repository repository)
        {

            Commit headCommit = repository.Commits.First();

            //TODO: Select better exception class
            if (headCommit == null)
                throw new InvalidOperationException("Could not find HEAD commit");

            Dictionary<Commit, Tag> taggedCommits = new Dictionary<Commit, Tag>();
            foreach (Commit commit in repository.Commits)
                foreach (Tag tag in repository.Tags)
                    if (tag.Target == commit)
                        taggedCommits.Add(commit, tag);

            Dictionary<Commit, int> commitsDistance = CalculateDistance(headCommit, taggedCommits);

            Commit recentTaggedCommit = commitsDistance.OrderBy(cd => cd.Value).First().Key;

            Tag recentTag = taggedCommits[recentTaggedCommit];

            Tag = recentTag.Name;
            CommitCount = commitsDistance[recentTaggedCommit];
            CommitHash = recentTaggedCommit.Sha;

            return true;
        }

        private Dictionary<Commit, int> CalculateDistance(Commit headCommit, Dictionary<Commit, Tag> taggedCommits)
        {
            Dictionary<Commit, int> result = new Dictionary<Commit, int>();

            List<Commit> leftParents = new List<Commit>();
            SelectFirstParent(leftParents, headCommit);

            foreach (Commit taggedCommit in taggedCommits.Keys)
            {
                if (!leftParents.Contains(taggedCommit))
                    continue;

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

        private void SelectFirstParent(List<Commit> parents, Commit commit)
        {
            parents.Add(commit);
            Commit firstParent = commit.Parents.FirstOrDefault();
            if (firstParent != null)
                SelectFirstParent(parents, firstParent);
        }
    }
}

using LibGit2Sharp;
using Microsoft.Build.Framework;
using System.Linq;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// Task to get name of current branch or tag
    /// </summary>
    public class GitBranch : GitTask
    {
        /// <summary>
        /// Name of current branch
        /// </summary>
        [Output]
        public string Branch { get; private set; }

        /// <summary>
        /// Task logic
        /// </summary>
        /// <param name="repository">Repository object</param>
        /// <returns>True if task was executed successfuly, overwise false</returns>
        protected override bool ExecuteCommand(Repository repository)
        {
            var currentBranch = repository.Branches.Single(br => br.IsCurrentRepositoryHead == true);
            Branch = currentBranch.Name;

            return true;
        }
    }
}

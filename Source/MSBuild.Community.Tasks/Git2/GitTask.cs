using LibGit2Sharp;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSBuild.Community.Tasks.Git2
{
    /// <summary>
    /// Base class for Git tasks
    /// </summary>
    public abstract class GitTask : Task
    {
        /// <summary>
        /// Gets or sets Git repostory path
        /// </summary>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Executes task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                using (Repository repository = new Repository(RepositoryPath))
                {
                    return ExecuteCommand(repository);
                }
            }
            catch (LibGit2SharpException ex)
            {
                Log.LogErrorFromException(ex);

                return false;
            }
        }

        /// <summary>
        /// Abstract method to execute command logic
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        protected abstract bool ExecuteCommand(Repository repository);
    }
}

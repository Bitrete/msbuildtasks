using Microsoft.Build.Framework;
using System.Text.RegularExpressions;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// Task to get version from Git tags. Task based on GitDescribe task.
    /// </summary>
    public class GitSemanticVersion : GitDescribe
    {
        private const string VersionRegexString = @"v(?<version>\d+\.\d+\.\d+)";

        private static readonly Regex VersionRegex = new Regex(VersionRegexString);

        /// <summary>
        /// Constructor
        /// </summary>
        public GitSemanticVersion()
            : base()
        {
            Version = string.Empty;
            IsRelease = false;
        }

        /// <summary>
        /// Version extracted from the most recent tag
        /// </summary>
        [Output]
        public string Version { get; private set; }

        /// <summary>
        /// Flag indicating that build was produced from tagge commit
        /// </summary>
        [Output]
        public bool IsRelease { get; private set; }

        /// <summary>
        /// Method to execute task
        /// </summary>
        /// <returns>True if task completed successfully, otherwise false</returns>
        public override bool Execute()
        {
            var result = base.Execute();
            if (result)
            {
                var match = VersionRegex.Match(Tag);
                if (match.Success)
                    Version = match.Groups["version"].Value;
                else
                    result = false;

                IsRelease = CommitCount == 0;
            }

            return result;
        }
    }
}

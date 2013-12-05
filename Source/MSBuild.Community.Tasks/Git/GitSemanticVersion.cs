using Microsoft.Build.Framework;
using System.Text.RegularExpressions;

namespace MSBuild.Community.Tasks.Git
{
    class GitSemanticVersion : GitDescribe
    {
        private const string VersionRegexString = @"v(?<version>\d+\.\d+\.\d+)";

        private static readonly Regex VersionRegex = new Regex(VersionRegexString);

        public GitSemanticVersion()
            : base()
        {
            SemanticVersion = string.Empty;
            IsRelease = false;
        }

        [Output]
        public string SemanticVersion { get; private set; }

        [Output]
        public bool IsRelease { get; private set; }

        public override bool Execute()
        {
            var result = base.Execute();
            if (result)
            {
                var match = VersionRegex.Match(Tag);
                if (match.Success)
                    SemanticVersion = match.Groups["version"].Value;
                else
                    result = false;

                IsRelease = CommitCount == 0;
            }

            return result;
        }
    }
}

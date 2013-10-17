using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// Class to extract required values from GitDescribe output string.
    /// </summary>
    public class SemanticVersionGitDescribeExtractor
    {
        private const string HashRegexString = @"(?<hash>[\dabcdef]{7})";
        private const string GitHashRegexString = "g" + HashRegexString;
        private const string VersionRegexString = @"v(?<version>\d+\.\d+\.\d+)";
        private const string AdditionalCommitsCountRegexString = @"-(?<count>\d+)-";

        private static readonly Regex HashRegex = new Regex(HashRegexString);
        private static readonly Regex GitHashRegex = new Regex(GitHashRegexString);
        private static readonly Regex VersionRegex = new Regex(VersionRegexString);
        private static readonly Regex AdditionalCommitsCountRegex = new Regex(AdditionalCommitsCountRegexString);
        private static readonly Regex DescriptionRegex = new Regex(VersionRegexString + AdditionalCommitsCountRegexString + GitHashRegexString);

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersionGitDescribeExtractor"/> class.
        /// </summary>
        public SemanticVersionGitDescribeExtractor()
        {
            Version = string.Empty;
            AdditionalCommitsCount = string.Empty;
            Hash = string.Empty;
        }

        /// <summary>
        /// Gets tag extracted from input string.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets additional commits count extracted from input string.
        /// </summary>
        public string AdditionalCommitsCount { get; private set; }

        /// <summary>
        /// Gets hash extracted from input string.
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// Extract values from given string.
        /// </summary>
        /// <param name="line">A line of text to extract parameters from.</param>
        public bool Extract(string line)
        {
            if (DescriptionRegex.IsMatch(line))
            {
                Version = ExtractVersion(line);
                AdditionalCommitsCount = ExtractAdditionalCommitsCount(line);
                Hash = ExtractGitHash(line);

                return true;
            } 
            else if (HashRegex.IsMatch(line))
            {
                Hash = ExtractHash(line);

                return true;
            }

            return false;
        }

        private string ExtractVersion(string line)
        {
            var match = VersionRegex.Match(line);
            if (match.Success)
                return match.Groups["version"].Value;

            return string.Empty;
        }

        private string ExtractAdditionalCommitsCount(string line)
        {
            var match = AdditionalCommitsCountRegex.Match(line);
            if (match.Success)
                return match.Groups["count"].Value;

            return string.Empty;
        }

        private string ExtractHash(string line)
        {
            var match = HashRegex.Match(line);
            if (match.Success)
                return match.Groups["hash"].Value;

            return line;
        }

        private string ExtractGitHash(string line)
        {
            var match = GitHashRegex.Match(line);
            if (match.Success)
                return match.Groups["hash"].Value;

            return line;
        }
    }
}

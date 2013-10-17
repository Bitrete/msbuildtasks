using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// Task to get information about the most recent tag reacheable from HEAD commit.
    /// </summary>
    public class SemanticVersionGitDescribe : GitVersion
    {
        private const string Zero = "0";

        private SemanticVersionGitDescribeExtractor extractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersionGitDescribe"/> class.
        /// </summary>
        public SemanticVersionGitDescribe()
        {
            extractor = new SemanticVersionGitDescribeExtractor();

            Command = "describe";
            Revision = "HEAD";
        }

        /// <summary>
        /// Gets a version contained in the most recent tag.
        /// </summary>
        [Output]
        public string SemanticVersion 
        {
            get
            {
                return extractor.Version;
            }
        }

        /// <summary>
        /// Gets the number of additional commits after the most recent tag.
        /// </summary>
        [Output]
        public string AdditionalCommitsCount
        {
            get
            {
                return extractor.AdditionalCommitsCount;
            }
        }

        /// <summary>
        /// Gets the last commit hash.
        /// </summary>
        [Output]
        public string Hash
        {
            get
            {
                return extractor.Hash;
            }
        }

        /// <summary>
        /// Gets the indication that the recent commit is tagged commit.
        /// </summary>
        [Output]
        public bool IsRelease 
        {
            get
            {
                return Zero.Equals(extractor.AdditionalCommitsCount);
            }
        }

        /// <summary>
        /// Generates command arguments.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void GenerateArguments(CommandLineBuilder builder)
        {
            builder.AppendSwitch("--tags");
            builder.AppendSwitch("--always");
            builder.AppendSwitch("--long");

            builder.AppendSwitch(Revision);
        }

        /// <summary>
        /// Parses a single line of text to identify any errors or warnings in canonical format.
        /// </summary>
        /// <param name="singleLine">A single line of text for the method to parse.</param>
        /// <param name="messageImportance">A value of <see cref="T:Microsoft.Build.Framework.MessageImportance"/> that indicates the importance level with which to log the message.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            bool isError = messageImportance == StandardErrorLoggingImportance;

            if (isError)
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            else
            {
                if (!extractor.Extract(singleLine))
                    Log.LogError("Unable to extract a version from the most recent tag.");
            }
        }
    }
}

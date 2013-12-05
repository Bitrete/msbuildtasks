using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MSBuild.Community.Tasks.Tests.Git
{
    public abstract class GitTestBase
    {
        private const string TagFreeRepositoryName = "tag-free-repo";
        private const string TagFreeDirtyRepositoryName = "tag-free-dirty-repo";
        private const string LastCommitTaggedRepositoryName = "last-commit-tagged-repo";
        private const string LastCommitTaggedDirtyRepositoryName = "last-commit-tagged-dirty-repo";
        private const string TagOnBranchRepositoryName = "tag-on-branch-repo";
        private const string TagOnMasterRepositoryName = "tag-on-master-repo";
        private const string AlphaTagRepositoryName = "alpha-tag-repo";
        private const string EarlyTagRepositoryName = "early-tag-repo";

        public static DirectoryInfo TestResources { get; private set; }
        public static string TagFreeRepository { get; private set; }
        public static string TagFreeDirtyRepository { get; private set; }
        public static string LastCommitTaggedRepository { get; private set; }
        public static string LastCommitTaggedDirtyRepository { get; private set; }
        public static string TagOnBranchRepository { get; private set; }
        public static string TagOnMasterRepository { get; private set; }
        public static string AlphaTagRepository { get; private set; }
        public static string EarlyTagRepository { get; private set; }

        public void SetUp()
        {
            CreateRepositories();
        }

        public void TearDown()
        {
            DeleteSubdirectories(TestResources.FullName);
            Directory.Delete(TestResources.FullName);
        }

        private static readonly Dictionary<string, string> ToRename = new Dictionary<string, string>
        {
            { "dot_git", ".git" }
        };

        protected const string TestRepositoryPath = @"C:\Temp\Repo1";

        protected void CheckoutBranch(string repositoryPath, string branchName)
        {
            using (var repository = new Repository(repositoryPath))
            {
                var branch = repository.Branches.Single(br => br.Name.Equals(branchName));
                branch.Checkout();
            }
        }


        protected void CreateRepositories()
        {
            var source = new DirectoryInfo(@"../../Resources");
            TestResources = new DirectoryInfo(string.Format(@"Resources/{0}", Guid.NewGuid()));

            CopyFilesRecursively(source, TestResources);

            TagFreeRepository = Path.Combine(TestResources.FullName, TagFreeRepositoryName);
            TagFreeDirtyRepository = Path.Combine(TestResources.FullName, TagFreeDirtyRepositoryName);
            LastCommitTaggedRepository = Path.Combine(TestResources.FullName, LastCommitTaggedRepositoryName);
            LastCommitTaggedDirtyRepository = Path.Combine(TestResources.FullName, LastCommitTaggedDirtyRepositoryName);
            TagOnBranchRepository = Path.Combine(TestResources.FullName, TagOnBranchRepositoryName);
            TagOnMasterRepository = Path.Combine(TestResources.FullName, TagOnMasterRepositoryName);
            AlphaTagRepository = Path.Combine(TestResources.FullName, AlphaTagRepositoryName);
            EarlyTagRepository = Path.Combine(TestResources.FullName, EarlyTagRepositoryName);
        }

        private void CopyFilesRecursively(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            foreach (DirectoryInfo dir in sourceDirectory.GetDirectories())
            {
                CopyFilesRecursively(dir, targetDirectory.CreateSubdirectory(Rename(dir.Name)));
            }
            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                file.CopyTo(Path.Combine(targetDirectory.FullName, Rename(file.Name)));
            }
        }

        private string Rename(string name)
        {
            return ToRename.ContainsKey(name) ? ToRename[name] : name;
        }

        public void DeleteSubdirectories(string parentPath)
        {
            string[] dirs = Directory.GetDirectories(parentPath);
            foreach (string dir in dirs)
                DeleteDirectory(dir);
        }

        private void DeleteDirectory(string directoryPath)
        {
            // From libgit2/libgit2sharp tests
            // From http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true/329502#329502

            if (!Directory.Exists(directoryPath))
            {
                Trace.WriteLine(
                    string.Format("Directory '{0}' is missing and can't be removed.",
                        directoryPath));

                return;
            }

            string[] files = Directory.GetFiles(directoryPath);
            string[] dirs = Directory.GetDirectories(directoryPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            File.SetAttributes(directoryPath, FileAttributes.Normal);
            try
            {
                Directory.Delete(directoryPath, false);
            }
            catch (IOException)
            {
                Trace.WriteLine(string.Format("{0}The directory '{1}' could not be deleted!" +
                                                    "{0}Most of the time, this is due to an external process accessing the files in the temporary repositories created during the test runs, and keeping a handle on the directory, thus preventing the deletion of those files." +
                                                    "{0}Known and common causes include:" +
                                                    "{0}- Windows Search Indexer (go to the Indexing Options, in the Windows Control Panel, and exclude the bin folder of LibGit2Sharp.Tests)" +
                                                    "{0}- Antivirus (exclude the bin folder of LibGit2Sharp.Tests from the paths scanned by your real-time antivirus){0}",
                    Environment.NewLine, Path.GetFullPath(directoryPath)));
            }
        }

    }
}

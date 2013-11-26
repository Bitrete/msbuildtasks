using LibGit2Sharp;
using System.Linq;

namespace MSBuild.Community.Tasks.Tests.Git2
{
    public abstract class GitTestBase
    {
        protected const string TestRepositoryPath = @"C:\Temp\Repo1";

        protected void CheckoutBranch(string branchName)
        {
            using (var repository = new Repository(TestRepositoryPath))
            {
                var branch = repository.Branches.Single(br => br.Name.Equals(branchName));
                branch.Checkout();
            }
        }
    }
}

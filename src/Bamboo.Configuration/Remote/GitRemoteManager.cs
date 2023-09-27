using Bamboo.Configuration.Helpers;
using Bamboo.Logging;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bamboo.Configuration.Remote
{
    internal class GitRemoteManager
    {
        private static ILogger _logger = new BambooLogger<GitRemoteManager>();

        public static void Fetch()
        {
            var settings = AppSettingsHelper.GetGitSettings();

            if (!settings.Item1)
                return;

            foreach (var item in settings.Item2)
            {
                try
                {
                    VerifySetting(item);

                    DownloadAndCopy(item);

                    if (item.AutoFetch)
                        ConfigDownloadManager.StartTimerIfNotExist($"{item.RemoteAddress}_{item.Branch}".Trim(), item.AutoFetchInterval, (s, e) => DownloadAndCopy(item));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Git configuration source '{item.RepoName}' fetch error");
                }
            }
        }

        /// <summary>
        /// Default fit config download directory
        /// </summary>
        private const string GitConfigDefaultDownloadDirectory = "BambooConfigDownload";

        private static string GitConfigDownloadDirectoryFullPath = Path.Combine(AppContext.BaseDirectory, GitConfigDefaultDownloadDirectory);

        private static void VerifySetting(GitSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.RepoName))
                throw new ArgumentNullException(nameof(setting.RepoName));

            if (string.IsNullOrWhiteSpace(setting.RemoteAddress))
                throw new ArgumentNullException(nameof(setting.RemoteAddress));

            if (string.IsNullOrWhiteSpace(setting.Branch))
                throw new ArgumentNullException(nameof(setting.Branch));

            //The pulling frequency should not be too high.
            if (setting.AutoFetchInterval < 5000)
            {
                setting.AutoFetchInterval = 5000;
                _logger.LogInformation($"The Git source fetch interval is too short and has been reset to 5s (minimum interval)");
            }
        }

        private static void DownloadAndCopy(GitSetting setting)
        {
            var workSpace = Path.Combine(GitConfigDownloadDirectoryFullPath, setting.RepoName);

            if (Download(workSpace, setting))
            {
                CopyLocker.Copy(() =>
                {
                    Copy(workSpace, ConfigurationConst.ConfigurationBaseFolder);
                });
            }
        }

        private static bool Download(string workSpace, GitSetting setting)
        {
            //Ensure directory exist
            if (!Directory.Exists(workSpace))
                Directory.CreateDirectory(workSpace);

            try
            {
                _logger.LogDebug($"git repo '{setting.RepoName}' pulling...");

                //download files
                if (!Repository.IsValid(workSpace))
                    Repository.Clone(setting.RemoteAddress, workSpace);

                using (var repo = new Repository(workSpace))
                {
                    //fetch --prune
                    repo.Config.Set("fetch.prune", true);
                    Commands.Fetch(repo, "origin", new string[0], null, null);

                    //get branch
                    var branch = repo.Branches.FirstOrDefault(t => t.FriendlyName.Equals(string.Concat("origin/", setting.Branch)));

                    if (branch == null)
                        throw new ArgumentOutOfRangeException(nameof(branch), $"the branch '{setting.Branch}'in configuration is not found");

                    //checkout
                    Commands.Checkout(repo, branch);

                    //Commands.Pull(repo, repo.Config.BuildSignature(DateTimeOffset.Now), new PullOptions { });
                }

                _logger.LogDebug($"git repo '{setting.RepoName}' pulling success.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"git repo '{setting.RepoName}' pull error.");
                return false;
            }

            return true;
        }

        private static void Copy(string workspace, string toFolder)
        {
            if (!Directory.Exists(toFolder))
                Directory.CreateDirectory(toFolder);

            foreach (var item in Directory.GetFiles(workspace))
            {
                File.Copy(item, item.Replace(workspace, toFolder), true);
            }

            foreach (var item in Directory.GetDirectories(workspace))
            {
                if (item.Contains(".git"))
                    continue;

                Copy(item, Path.Combine(toFolder, Path.GetFileName(item)));
            }
        }
    }
}

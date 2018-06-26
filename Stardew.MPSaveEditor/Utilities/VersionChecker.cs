using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace StardewValley.MPSaveEditor.Utilities {
    public class VersionChecker {
        
        private static HttpClient gitClient = new HttpClient();
        
        public String run(String currentVersion) {
            String result;
            currentVersion = currentVersion.Replace("v", "");
            try {
                Stream response = this.getReleases();
                String latestVersion = this.parseResponse(response);
                result = this.compareVersions(currentVersion, latestVersion);
            }
            catch (Exception exception) {
                result = "There was a problem checking the latest release.";
                Console.WriteLine(exception);
            }
            return result;
        }
        
        private Stream getReleases() {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/tfitz237/stardew-mp-save-editor/releases");
            requestMessage.Headers.Add("User-Agent", "sd-mp-save-editor");
            var response = gitClient.SendAsync(requestMessage);
            Stream responseBody;
            if (response.Result.ReasonPhrase == "OK") {
                responseBody = response.Result.Content.ReadAsStreamAsync().Result;
            }
            else {
                throw new Exception("Failed to get release information from Git.");
            }
            return responseBody;
        }

        private String parseResponse(Stream responseBody) {
            String versionStatus = "0.0.0.0";
            Regex regex = new Regex("\"tag_name\":\"([^,]*)\"");
            StreamReader responseReader = new StreamReader(responseBody);
            String responseText = responseReader.ReadToEnd();
            MatchCollection matches = regex.Matches(responseText);
            if (matches.Count == 0) {
                throw  new Exception("Unable to find release version tags, check the Git API.");
            }
            foreach (Match match in matches) {
                String gitRelease = match.Groups[1].Value.Replace("v", "");
                if (Version.TryParse(gitRelease, out Version releaseVersion)) {
                    if (releaseVersion.CompareTo(new Version(versionStatus)) > 0) {
                        versionStatus = gitRelease;
                    }
                }
            }
            return versionStatus;
        }

        private String compareVersions(String currentVersionName, String latestVersionName) {
            String versionStatus;

            Version currentVersion = new Version(currentVersionName);
            Version latestVersion = new Version(latestVersionName);
            if (latestVersion.CompareTo(currentVersion) > 0) {
                versionStatus = String.Format("Version {0} of the Stardew Valley Farmhand Managment System is now available. You can download it from https://github.com/tfitz237/stardew-mp-save-editor/releases.", latestVersionName);
            }
            else if (latestVersion.CompareTo(currentVersion) == 0) {
            versionStatus = "You are running the latest version of the Stardew Valley Farmhand Managment System.";
            }
            else {
                throw new Exception("You are running an unreleased build or there was an error in the version checking process.");
            }
            return versionStatus;
        }
    }
}

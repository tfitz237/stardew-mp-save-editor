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
            var response = this.getReleases();
            var latestVersion = this.parseResponse(response);
            // Need to have a separate path if the version is not valid or a try catch + raise exception from parseResponse.
            if (latestVersion == "FAILED") {
                return "There was a problem checking the latest release. Please goto https://github.com/tfitz237/stardew-mp-save-editor/releases to see if there is a newer version";
            }
            result = this.compareVersions(currentVersion, latestVersion);
            return result;
        }
        
        private Stream getReleases() {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/tfitz237/stardew-mp-save-editor/releases");
            requestMessage.Headers.Add("User-Agent", "sd-mp-save-editor");
            var response = gitClient.SendAsync(requestMessage);
            Stream responseBody = response.Result.Content.ReadAsStreamAsync().Result;
            if (!response.IsCompletedSuccessfully) {
                responseBody = new MemoryStream();
            }
            return responseBody;
        }

        private String parseResponse(Stream responseBody) {
            String versionStatus = "0.0.0.0";
            try {
                Regex regex = new Regex("\"tag_name\":\"([^,]*)\"");
                StreamReader responseReader = new StreamReader(responseBody);
                String responseText = responseReader.ReadToEnd();
                MatchCollection matches = regex.Matches(responseText);
                foreach (Match match in matches) {
                    String gitRelease = match.Groups[1].Value.Replace("v", "");
                    Version releaseVersion = new Version(gitRelease);
                    if (releaseVersion.CompareTo(new Version(versionStatus)) > 0) {
                        versionStatus = gitRelease;
                    }
                }
            }
            catch (SerializationException) {
                versionStatus = "FAILED";
            }
            catch (NullReferenceException) {
                versionStatus = "FAILED";
            }
            return versionStatus;
        }

        private String compareVersions(String currentVersionName, String latestVersionName) {
            String versionStatus;

            var currentVersion = new Version(currentVersionName);
            var latestVersion = new Version(latestVersionName);
            if (latestVersion.CompareTo(currentVersion) > 0) {
                versionStatus = String.Format("Version {0} of the Stardew Valley Multiplayer Save Editor is now available. You can download it from https://github.com/tfitz237/stardew-mp-save-editor/releases.", latestVersionName);
            }
            else {
            versionStatus = "You are running the latest version of the Stardew Valley Multiplayer Save Editor.";
            }
            return versionStatus;
        }
    }
}

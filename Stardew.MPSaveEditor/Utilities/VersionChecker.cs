using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;


namespace StardewValley.MPSaveEditor.Utilities {
    public class VersionChecker {
        
        private static HttpClient gitClient = new HttpClient();

        public String run(String currentVersion) {
            String result;
            currentVersion = currentVersion.Replace("v", "");
            var response = this.getReleases();
            var latestVersion = this.parseResponse(response);
            result = this.compareVersions(currentVersion, latestVersion);
            return result;
        }
        
        private Stream getReleases() {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/tfitz237/stardew-mp-save-editor/releases");
            requestMessage.Headers.Add("User-Agent", "sd-mp-editor");
            var response = gitClient.SendAsync(requestMessage);
            Stream responseBody = response.Result.Content.ReadAsStreamAsync().Result;
            if (!response.IsCompletedSuccessfully) {
                responseBody = new MemoryStream();
            }
            return responseBody;
        }

        private String parseResponse(Stream responseBody) {
            String versionStatus;
            var serializer = new DataContractJsonSerializer(typeof(GitRelease[]));
            try {
                GitRelease[] gitReleases = (GitRelease[]) serializer.ReadObject(responseBody);
                GitRelease release = (GitRelease) gitReleases.GetValue(0);
                versionStatus = release.tagName.Replace("v", "");
            }
            catch (SerializationException) {
                versionStatus = "Request for the latest release information failed.";
            }
            return versionStatus;
        }

        private String compareVersions(String currentVersionName, String latestVersionName) {
            String versionStatus;

            var latestVersion = new Version(latestVersionName);
            var currentVersion = new Version(currentVersionName);
            if (latestVersion.CompareTo(currentVersion) > 0) {
                versionStatus = String.Format("Version {0} of the Stardew Valley Multiplayer Save Editor is now available. You can download it from https://github.com/tfitz237/stardew-mp-save-editor/releases.", latestVersionName);
            }
            else {
            versionStatus = "You are running the latest version of the Stardew Valley Multiplayer Save Editor.";
            }
            return versionStatus;
        }

        // static int Main(string[] args) {
        //     VersionChecker versionChecker = new VersionChecker();
        //     Console.WriteLine(versionChecker.checkVersion());
        //     return 0;
        // }

        [DataContract]
        private class GitRelease {
            [DataMember(Name = "tag_name")]
            public String tagName {get; set;}
        }
    }
}
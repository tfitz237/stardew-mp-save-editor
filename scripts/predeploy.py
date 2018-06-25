import os
import re
import subprocess

from pkg_resources import parse_version


def get_existing_versions():
    git_tags = subprocess.check_output(["git", "tag"]).split()
    return [x for x in git_tags]

def get_new_version(source_path):
    with open(source_path) as source_file:
        stardew_source = source_file.read()
    result = re.search("static String VERSION = \"(.*)\";", stardew_source)
    return result.group(1)


existing_versions = get_existing_versions()
new_version = get_new_version("Stardew.MPSaveEditor/StardewSaveEditor.cs")

if all([parse_version(new_version) > parse_version(existing_version) for existing_version in existing_versions]):
    subprocess.call(["git", "tag", new_version])
    os.environ["TRAVIS_TAG"] = "v" + new_version

    subprocess.call(["git", "config", "--local", "user.name", "\"Travis CI - Deploy\""])
    subprocess.call(["git", "config", "--local", "user.email", "\"travis@travisci.org\""])

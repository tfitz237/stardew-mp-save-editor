import os
import re
import subprocess

from pkg_resources import parse_version


def find_current_version(versions):
    current_version = "0.0.0"
    for version in versions:
        if parse_version(version) > parse_version(current_version):
            current_version = version
    return current_version


git_tags = subprocess.check_output(["git", "tag"]).split()

versions = [x for x in git_tags if x.replace(".", "").isdigit()]
current_version_digits = find_current_version(versions).split(".")
current_version_digits[-1] = str(int(current_version_digits[-1]) + 1)
new_version = ".".join(current_version_digits)
subprocess.call(["git", "tag", new_version])
os.environ["TRAVIS_TAG"] = new_version

subprocess.call(["git", "config", "--local", "user.name", "\"Travis CI - Deploy\""])
subprocess.call(["git", "config", "--local", "user.email", "\"travis@travisci.org\""])
reg='^([0-9])+\.([0-9])+\.([0-9]+)$'

for fn in $(git tag); do
    if [[$fn =~ reg]]
        then
            tag="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.$((${BASH_REMATCH[3]}+1))"
            echo "new git tag: $tag"
            git tag "$tag"
            $TRAVIS_TAG=$tag
    fi
done

git config --local user.name "Travis CI - Deploy"
git config --local user.email "travis@travisci.org"

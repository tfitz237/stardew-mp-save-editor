tag=$(git tag | tail -1)
echo "old tag: $tag"
reg='^([0-9])+\.([0-9])+\.([0-9]+)$'
if [[ tag =~ reg ]]
then 
    tag="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.$((${BASH_REMATCH[3]}+1))"
    echo "new tag: $tag"
    git tag "$tag"
fi
git config --local user.name "Travis CI - Deploy"
git config --local user.email "travis@travisci.org"

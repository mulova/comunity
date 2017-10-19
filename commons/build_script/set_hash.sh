REPO_HASH=$(git rev-parse HEAD)

while [[ $# > 0 ]]
do
key="$1"

case $key in
    -f|--file)
    FILE="$2"
    shift # past argument
    ;;
    *)
            # unknown option
    ;;
esac
shift # past argument or value
done

sed -e "s/REPO[[:space:]]*=.*/REPO=\"$REPO_HASH\";/g" -i "" "$FILE"

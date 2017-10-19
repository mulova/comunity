#!/bin/bash -ex
cd -- "$(dirname "$BASH_SOURCE")"
source defs.py

# set dll name and postbuild commands
CONFIGURATION=Debug

function build
{
    define=$1
    if [ ! -z "$define" ]; then
        python csproj_mod.py "${define}" "False"
    fi
    $MONO $MDTOOL build -t:Build -c:$CONFIGURATION -p:${editor_project_file} ${MONO_SOLUTION}
    ./copy_dll.sh ${define}
}

# build script
build
#python csproj_mod.py "ANDROID" "True"

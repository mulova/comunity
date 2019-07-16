#!/bin/bash -ex
cd -- "$(dirname "$BASH_SOURCE")/.."
source build_script/defs.py

# set dll name and postbuild commands
CONFIGURATION=Debug

function build
{
    python build_script/csproj_mod.py "$1"
    if [ ! -z "$editor_project_file" ]; then
        $MONO $MDTOOL build -t:Build -c:$CONFIGURATION -p:${editor_project_file} ${MONO_SOLUTION}
    else
        $MONO $MDTOOL build -t:Build -c:$CONFIGURATION -p:${runtime_project_file} ${MONO_SOLUTION}
    fi
    build_script/copy_dll.sh $1
}

# build script
build_script/set_hash.sh -f AssemblyInfo.cs
build
python build_script/csproj_mod.py "${define}" "build_script/copy_dll.sh"

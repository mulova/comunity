#!/bin/bash -ex
cd -- "$(dirname "$BASH_SOURCE")"

MONO="/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current/bin/mono"
MDTOOL="/Applications/Unity/MonoDevelop.app/Contents/MacOS/lib/monodevelop/bin/mdtool.exe"
# set dll name and postbuild commands

CONFIGURATION=Debug
PROJECT_NAME=Assembly-CSharp-Editor
SOLUTION=effect.sln

function build
{
    define=$1
    python csproj_mod.py "${define}" "False"
    $MONO $MDTOOL build -t:Build -c:$CONFIGURATION -p:$PROJECT_NAME $SOLUTION
    ./copy_dll.sh ${add}
}

$MONO $MDTOOL build -t:Build -c:$CONFIGURATION -p:$PROJECT_NAME $SOLUTION
build ANDROID
python csproj_mod.py "ANDROID" "True"

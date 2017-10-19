#!/bin/bash -exv
cd -- "$(dirname "$BASH_SOURCE")"
ROOT=../..
LIB_DIR=Lib

source defs.py

if [ ! -z "$1" ]; then
    LIB_DIR=Lib/$1
fi

SRC_DIR=bin/Debug
DST=( "${ROOT}"
)

# copy lib
for p in "${DST[@]}"
do
#  cp -R Assets/Plugins "$p/Assets/"
  # copy runtime libs
  mkdir -p $p/Assets/Plugins/$LIB_DIR
  cp $SRC_DIR/${runtime_project}.dll "$p/Assets/Plugins/$LIB_DIR"
  cp $SRC_DIR/${runtime_project}.dll.mdb "$p/Assets/Plugins/$LIB_DIR"
  # copy editor libs
  mkdir -p $p/Assets/Plugins/Editor/Lib
  cp $SRC_DIR/${editor_project}.dll "$p/Assets/Plugins/Editor/Lib"
  cp $SRC_DIR/${editor_project}.dll.mdb "$p/Assets/Plugins/Editor/Lib"
done


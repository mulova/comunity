#!/bin/bash -exv
cd -- "$(dirname "$BASH_SOURCE")/.."
source build_script/defs.py
source build_script/defs.sh

ROOT=../..
LIB_DIR=lib

SRC_DIR=bin/Debug

# copy lib
for p in "${DST[@]}"
do
#  cp -R Assets/Plugins "$p/Assets/"
  # copy runtime libs
  mkdir -p $p/Assets/Plugins/$LIB_DIR
  cp $SRC_DIR/${runtime_project}.dll "$p/Assets/Plugins/$LIB_DIR"
  cp $SRC_DIR/${runtime_project}.dll.mdb "$p/Assets/Plugins/$LIB_DIR"
  # copy editor libs
  if [ ! -z "$editor_project_file" ]; then
      mkdir -p $p/Assets/Plugins/Editor/Lib
      cp $SRC_DIR/${editor_project}.dll "$p/Assets/Plugins/Editor/Lib"
      cp $SRC_DIR/${editor_project}.dll.mdb "$p/Assets/Plugins/Editor/Lib"
  fi
done


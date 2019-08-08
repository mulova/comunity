#!/bin/bash -exv
cd -- "$(dirname "$BASH_SOURCE")/.."
source build_script/defs.py
source build_script/defs.sh

ROOT=../..
LIB_DIR=platform_libs/$1

SRC_DIR=Temp/bin/Debug

# copy lib
for p in "${DLL_DST[@]}"
do
  cp -R Assets/Plugins "$p/Assets/"
  # copy runtime libs
  mkdir -p $p/$LIB_DIR
  cp $SRC_DIR/${runtime_project}.dll "$p/$LIB_DIR"
  cp $SRC_DIR/${runtime_project}.dll.mdb "$p/$LIB_DIR"
  # copy editor libs
  if [ ! -z "$editor_project_file" ]; then
      mkdir -p $p/$LIB_DIR/Editor
      cp $SRC_DIR/${editor_project}.dll "$p/$LIB_DIR/Editor"
      cp $SRC_DIR/${editor_project}.dll.mdb "$p/$LIB_DIR/Editor"
  fi
done


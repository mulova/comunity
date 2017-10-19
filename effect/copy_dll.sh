#!/bin/bash -exv
cd -- "$(dirname "$BASH_SOURCE")"
ROOT=../..
LIB_DIR=lib

if [ ! -z "$1" ]; then
    LIB_DIR=lib/$1
fi

SRC_DIR=Temp/bin/Debug
DST1=${ROOT}/board/hotel_client/Assets/Plugins/$LIB_DIR
DST1_EDITOR=${ROOT}/board/hotel_client/Assets/Plugins/Editor/Lib
DST2=${ROOT}/iron/Assets/Plugins/$LIB_DIR
DST2_EDITOR=${ROOT}/iron/Assets/Plugins/Editor/Lib

function copy_lib {
  mkdir -p $1
  mkdir -p $2
  cp $SRC_DIR/effect.dll $1/
  cp $SRC_DIR/effect.dll.mdb $1/
  cp $SRC_DIR/effect-Editor.dll $2/
  cp $SRC_DIR/effect-Editor.dll.mdb $2/
}

copy_lib ${DST1} ${DST1_EDITOR}
copy_lib ${DST2} ${DST2_EDITOR}

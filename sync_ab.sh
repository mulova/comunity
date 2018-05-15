#!/bin/bash
DIR="Assets/AssetBundleManager"
SRC="../../learn/AssetBundleDemo/demo/Assets/AssetBundleManager"
LIST=("")

for i in "${LIST[@]}"
do
    rsync -av --delete $SRC/${i} $DIR
done

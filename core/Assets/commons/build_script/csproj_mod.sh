#!/bin/bash
cd -- "$(dirname "$BASH_SOURCE")/.."
source build_script/defs.py
python build_script/csproj_mod.py "$define" "build_script/copy_dll.sh"

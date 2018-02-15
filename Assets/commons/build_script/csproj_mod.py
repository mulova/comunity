#!/usr/bin/python
import sys
import csproj_mod_lib

from defs import *
set_cmd=None

if len(sys.argv) > 1:
    define=sys.argv[1]
if len(sys.argv) > 2:
    set_cmd= sys.argv[2]
csproj_mod_lib.run_mod(set_cmd, define)


set DLL_DIR=Temp\bin\Debug

set LEGIONZ_LIB_DIR=..\..\Assets\Plugins\Lib
copy %DLL_DIR%\core.dll %LEGIONZ_LIB_DIR%\
copy %DLL_DIR%\core.dll.mdb %LEGIONZ_LIB_DIR%\

set LEGIONZ_EDITOR_LIB_DIR=..\..\Assets\Plugins\Editor\Lib
copy %DLL_DIR%\core-Editor.dll %LEGIONZ_EDITOR_LIB_DIR%\
copy %DLL_DIR%\core-Editor.dll.mdb %LEGIONZ_EDITOR_LIB_DIR%\
pause

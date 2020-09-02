setlocal Enabledelayedexpansion
@echo off
echo ================ "生成lua文件" ============
echo.

set PROTO_PATH=.\protocol
set STEP_PROTO_LUA=.\proto2lua

cd %STEP_PROTO_LUA%
dir ..\%PROTO_PATH%\*.proto /b >protolist.txt

for /f "delims=." %%i in (protolist.txt) do (
protoc --proto_path=..\%PROTO_PATH% ..\%PROTO_PATH%\%%i.proto --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=.\
)

cd ..
set LUA_PATH=..\Assets\L_Lua\protocol_generated
echo.
echo============================"拷贝lua文件到工程目录"==============
copy %STEP_PROTO_LUA%\*.lua %LUA_PATH%\*.lua
cd %STEP_PROTO_LUA%
del *.lua

pause
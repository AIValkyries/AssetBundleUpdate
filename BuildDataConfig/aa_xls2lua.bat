@echo off

set SHEET_NAME=%1
set XLS_FILE_PATH=%2

echo.
echo ==========================="%XLS_FILE_PATH%.%SHEET_NAME% to lua or bytes "===========================

set PROTO_2_LUA=.\xls_2_lua
cd %PROTO_2_LUA%

@echo off
echo 尝试删除临时文件
del *.data
del *.proto
del *.protodesc
del *.txt
del *.lua
del *.log
del tnt_deploy_*_pb2.py
del tnt_deploy_*_pb2.pyc

::生成python对应的文件?
python xls_deploy_tool.py %SHEET_NAME% ..\DataConfig\%XLS_FILE_PATH%.xlsx c

dir *.proto /b >>protolist.txt

for /f "delims=." %%i in (protolist.txt) do (
protoc --proto_path=..\%PROTO_2_LUA% ..\%PROTO_2_LUA%\%%i.proto --plugin=protoc-gen-lua="protoc-gen-lua.bat" --lua_out=.\ 
)

cd ..

set ASSET_DATA_PATH=..\Assets\L_Resources\DataConfig
set ASSET_LUA_PATH=..\Assets\L_Lua\protocol_generated

copy %PROTO_2_LUA%\*.data %ASSET_DATA_PATH%\*.bytes
copy %PROTO_2_LUA%\*.lua %ASSET_LUA_PATH%\*.lua

cd %PROTO_2_LUA%

::移除临时文件
@echo off
echo 尝试删除临时文件
del *.data
del *.proto
del *.protodesc
del *.txt
del *.lua
del *.log
del data_config_*_pb2.py
del data_config_*_pb2.pyc

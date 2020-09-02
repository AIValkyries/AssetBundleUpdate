setlocal Enabledelayedexpansion
@echo  off
echo ==================生成cs文件==================
echo.

set PROTO_GEN_EXE=.\proto2cs\ProtoGen\protogen.exe
set PROTO_PATH=.\protocol

:: 解析文本文件 
for /f "delims=." %%i in (cs_proto_list.txt) do (
   set /a i+=1
   set array[!i!]=%%i
)

for /l %%i in (1,1,!i!) do %PROTO_GEN_EXE% -i:%PROTO_PATH%/!array[%%i]!.proto -o:%PROTO_PATH%/!array[%%i]!.cs

set CS_PATH=..\Assets\L_Proto\ProtoModel
echo.
echo =================将cs拷贝到%CS_PATH%工程目录=================

copy %PROTO_PATH%\*.cs %CS_PATH%\*.cs
del %PROTO_PATH%\*.cs

pause
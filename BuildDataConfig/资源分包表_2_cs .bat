set XLS_FILE_NAME=资源分包表
set SHEET_NAME_1=SceneFenBao
set SHEET_NAME_2=NpcFenBao
set SHEET_NAME_3=NPCMissionFenBao
set SHEET_NAME_4=MonsterFenBao
set SHEET_NAME_5=PlayerFenBao
set SHEET_NAME_6=MusicFenBao
set SHEET_NAME_7=UIFenBao
set SHEET_NAME_8=SoundFenBao
set SHEET_NAME_9=SoundMissionFenBao
set SHEET_NAME_10=RestFenBao
set SHEET_NAME_11=LuaFenBao


@echo off

call aa_xls2cs %SHEET_NAME_1% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_2% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_3% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_4% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_5% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_6% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_7% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_8% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_9% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_10% %XLS_FILE_NAME%
call aa_xls2cs %SHEET_NAME_11% %XLS_FILE_NAME%

pause